using OA.Ultima.Core.IO;
using OA.Ultima.Core.Network.Compression;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using OA.Core;
using OA.Core.Diagnostics;

namespace OA.Ultima.Core.Network
{
    public sealed class NetworkClient : INetworkClient
    {
        readonly HuffmanDecompression _decompression;
        readonly List<PacketHandler>[] _typedHandlers;
        readonly List<PacketHandler>[][] _extendedTypedHandlers;
        readonly BufferPool _bufferPool = new BufferPool("Network Client - Buffer Pool", 8, 0x10000);
        readonly object _syncRoot = new object();
        readonly PacketChunk _incompleteDecompressionPacket;
        readonly PacketChunk _incompletePacket;
        Queue<QueuedPacket> _queue = new Queue<QueuedPacket>();
        Queue<QueuedPacket> _workingQueue = new Queue<QueuedPacket>();
        Socket _serverSocket;
        IPAddress _serverAddress;
        IPEndPoint _serverEndPoint;
        bool _isDecompressionEnabled;
        bool _isConnected;

        public int ClientAddress
        {
            get
            {
                var localEntry = Dns.GetHostEntry(Dns.GetHostName());
                int address;
                if (localEntry.AddressList.Length > 0)
                {
#pragma warning disable 618
                    address = (int)localEntry.AddressList[0].Address;
#pragma warning restore 618
                }
                else
                    address = 0x100007f;
                return ((((address & 0xff) << 0x18) | ((address & 65280) << 8)) | ((address >> 8) & 65280)) | ((address >> 0x18) & 0xff);
            }
        }

        public IPAddress ServerAddress
        {
            get { return _serverAddress; }
        }

        public bool IsDecompressionEnabled
        {
            get { return _isDecompressionEnabled; }
            set { _isDecompressionEnabled = value; }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        public NetworkClient()
        {
            _decompression = new HuffmanDecompression();
            _isDecompressionEnabled = false;
            _incompletePacket = new PacketChunk(_bufferPool.AcquireBuffer());
            _incompleteDecompressionPacket = new PacketChunk(_bufferPool.AcquireBuffer());
            _typedHandlers = new List<PacketHandler>[0x100];
            _extendedTypedHandlers = new List<PacketHandler>[0x100][];
            for (var i = 0; i < _typedHandlers.Length; i++)
                _typedHandlers[i] = new List<PacketHandler>();
        }

        public void Register<T>(object client, int id, int length, Action<T> onReceive) where T : IRecvPacket
        {
            var type = typeof(T);
            var ctors = type.GetConstructors();
            bool valid = false;
            for (var i = 0; i < ctors.Length && !valid; i++)
            {
                var parameters = ctors[i].GetParameters();
                valid = (parameters.Length == 1 && parameters[0].ParameterType == typeof(PacketReader));
            }
            if (!valid)
                throw new NetworkException($"Unable to register packet type {type} without a public constructor with a {typeof(PacketReader)} parameter");
            if (id > byte.MaxValue)
                throw new NetworkException($"Unable to register packet id 0x{id:X4} because it is greater than 0xff");
            var handler = new PacketHandler<T>(id, length, type, client, onReceive);
            if (_typedHandlers[id].Any())
            {
                var requiredLength = _typedHandlers[id][0].Length;
                Guard.Requires(requiredLength == length, "Invalid packet length.  All packet handlers for 0x{0:X2} must specify a length of {1}.", id, requiredLength);
            }
            _typedHandlers[id].Add(handler);
        }

        public void Unregister(object client)
        {
            for (var id = 0; id < byte.MaxValue; id++)
                if (_typedHandlers[id] != null)
                    for (var i = 0; i < _typedHandlers[id].Count; i++)
                    {
                        var handler = _typedHandlers[id][i] as PacketHandler;
                        if (handler.Client == client)
                        {
                            _typedHandlers[id].RemoveAt(i);
                            i--;
                        }
                    }
        }

        public void Unregister(object client, int id)
        {
            for (var i = 0; i < _typedHandlers[id].Count; i++)
            {
                var handler = _typedHandlers[id][i] as PacketHandler;
                if (handler.Client == client)
                {
                    _typedHandlers[id].RemoveAt(i);
                    break;
                }
            }
        }

        public void RegisterExtended<T>(object client, int extendedId, int subId, int length, Action<T> onReceive) where T : IRecvPacket
        {
            var type = typeof(T);
            var ctors = type.GetConstructors();
            var valid = false;
            for (var i = 0; i < ctors.Length && !valid; i++)
            {
                var parameters = ctors[i].GetParameters();
                valid = (parameters.Length == 1 && parameters[0].ParameterType == typeof(PacketReader));
            }
            if (!valid)
                throw new NetworkException(string.Format("Unable to register packet type {0} without a public constructor with a {1} parameter", type, typeof(PacketReader)));
            if (extendedId > byte.MaxValue)
                throw new NetworkException(string.Format("Unable to register packet extendedId {0:X2} because it is greater than byte.MaxValue", extendedId));
            if (subId > byte.MaxValue)
                throw new NetworkException(string.Format("Unable to register packet subId {0:X2} because it is greater than byte.MaxValue", subId));
            if (_extendedTypedHandlers[extendedId] == null)
            {
                _extendedTypedHandlers[extendedId] = new List<PacketHandler>[0x100];
                for (var i = 0; i < _extendedTypedHandlers[extendedId].Length; i++)
                    _extendedTypedHandlers[extendedId][i] = new List<PacketHandler>();
            }
            Utils.Log($"Registering Extended Command: id: 0x{extendedId:X2} subCommand: 0x{subId:X2} Length: {length}");
            var handler = new PacketHandler<T>(subId, length, type, client, onReceive);
            _extendedTypedHandlers[extendedId][subId].Add(handler);
        }

        public bool Connect(string ipAddressOrHostName, int port)
        {
            lock (_syncRoot)
            {
                _workingQueue.Clear();
            }
            _incompletePacket.Clear();
            _incompleteDecompressionPacket.Clear();
            if (IsConnected)
                Disconnect();
            var success = true;
            try
            {
                if (!IPAddress.TryParse(ipAddressOrHostName, out _serverAddress))
                {
                    var ipAddresses = Dns.GetHostAddresses(ipAddressOrHostName);
                    if (ipAddresses.Length == 0)
                        throw new NetworkException("Host address was unreachable or invalid, unable to obtain an ip address.");
                    // On Vista and later, the first ip address is an empty one '::1'. This makes sure we choose the first valid ip address.
                    foreach (var address in ipAddresses)
                    {
                        if (address.ToString().Length <= 7)
                            continue;
                        _serverAddress = address;
                        break;
                    }
                }
                _serverEndPoint = new IPEndPoint(_serverAddress, port);
                Utils.Log($"Connecting to {_serverAddress}:{port}...");
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Connect(_serverEndPoint);
                if (_serverSocket.Connected)
                {
                    Utils.Log("Connected.");
                    var state = new SocketState(_serverSocket, _bufferPool.AcquireBuffer());
                    _serverSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
                }

            }
            catch { success = false; }
            _isConnected = success;
            return success;
        }

        public void Disconnect()
        {
            if (_serverSocket != null)
            {
                try
                {
                    _serverSocket.Shutdown(SocketShutdown.Both);
                    _serverSocket.Close();
                }
                catch { } // empty catch.
                _serverSocket = null;
                _serverEndPoint = null;
                _isDecompressionEnabled = false;
                Utils.Log("Disconnected.");
                _isConnected = false;
            }
        }

        public bool Send(ISendPacket packet)
        {
            var buffer = packet.Compile();
            if (IsConnected)
            {
                var success = Send(buffer, 0, packet.Length, packet.Name);
                if (!success)
                    Disconnect();
                return success;
            }
            return false;
        }

        public bool Send(byte[] buffer, int offset, int length, string name)
        {
            var success = true;
            if (buffer == null || buffer.Length == 0)
                throw new NetworkException("Unable to send, buffer was null or empty");
            LogPacket(buffer, length, false);
            try
            {
                lock (_serverSocket)
                {
                    _serverSocket.Send(buffer, offset, length, SocketFlags.None);
                }
            }
            catch (Exception e)
            {
                Utils.Log(e.ToString());
                success = false;
            }
            return success;
        }

        void OnReceive(IAsyncResult result)
        {
            var state = result.AsyncState as SocketState;
            if (state == null)
            {
                Utils.Warning("Socket state was null.");
                return;
            }
            try
            {
                var socket = state.Socket;
                if (socket.Connected == false)
                {
                    Disconnect();
                    return;
                }
                var length = socket.EndReceive(result);
                if (length > 0)
                {
                    var buffer = state.Buffer;
                    if (_isDecompressionEnabled)
                        DecompressBuffer(ref buffer, ref length);
                    if (_incompletePacket.Length > 0)
                    {
                        _incompletePacket.Prepend(buffer, length);
                        _incompletePacket.Clear();
                    }
                    var offset = 0;
                    ProcessBuffer(buffer, ref offset, length);
                    // Not all the data was processed, due to an incomplete packet
                    if (offset < length)
                        _incompletePacket.Write(buffer, offset, length - offset);
                }
                if (_serverSocket != null)
                    _serverSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
            }
            catch (Exception e)
            {
                Utils.Exception(e);
                Disconnect();
            }
        }

        void DecompressBuffer(ref byte[] buffer, ref int length)
        {
            var source = _bufferPool.AcquireBuffer();
            var incompleteLength = _incompleteDecompressionPacket.Length;
            var sourceLength = incompleteLength + length;
            if (incompleteLength > 0)
            {
                _incompleteDecompressionPacket.Prepend(source, 0);
                _incompleteDecompressionPacket.Clear();
            }
            Buffer.BlockCopy(buffer, 0, source, incompleteLength, length);
            int outSize;
            var processedOffset = 0;
            var sourceOffset = 0;
            var offset = 0;
            while (_decompression.DecompressChunk(ref source, ref sourceOffset, sourceLength, ref buffer, offset, out outSize))
            {
                processedOffset = sourceOffset;
                offset += outSize;
            }
            length = offset;
            // We've run out of data to parse, or the packet was incomplete. If the packet was incomplete, we should save what's left for the next socket receive event.
            if (processedOffset >= sourceLength)
            {
                _bufferPool.ReleaseBuffer(source);
                return;
            }
            _incompleteDecompressionPacket.Write(source, processedOffset, sourceLength - processedOffset);
        }

        void ProcessBuffer(byte[] buffer, ref int offset, int length)
        {
            var index = offset;
            while (index < length)
            {
                int realLength;
                PacketHandler packetHandler;
                var packetHandlers = GetHandlers(buffer[index], buffer[index + 1]);
                // Ensure we have the proper handler for the size of this packet
                if (!GetPacketSizeAndHandler(packetHandlers, buffer, index, out realLength, out packetHandler))
                {
                    var formatBuffer = _bufferPool.AcquireBuffer();
                    Buffer.BlockCopy(buffer, index, formatBuffer, 0, length - index);
                    Utils.Warning($"Unhandled packet with id: 0x{buffer[index]:x2}, possible subid: 0x{buffer[index + 1]:x2}{Environment.NewLine}{Utility.FormatBuffer(formatBuffer, length - index)}");
                    index += length - index;
                    break;
                }
                // Entire packet exist, so we can process it
                if ((length - index) >= realLength)
                {
                    // TODO: Move this to a buffer pool, need to investigate max byte[].length and pool size
                    var packetBuffer = new byte[realLength];
                    Buffer.BlockCopy(buffer, index, packetBuffer, 0, realLength);
                    AddPacket(packetHandler, packetBuffer, realLength);
                    index += realLength;
                }
                // Need more data
                else break;
            }
            offset = index;
        }

        void AddPacket(PacketHandler packetHandler, byte[] packetBuffer, int realLength)
        {
            lock (_syncRoot)
            {
                _workingQueue.Enqueue(new QueuedPacket(packetHandler, packetBuffer, realLength));
            }
        }

        public void Slice()
        {
            if (!IsConnected)
                return;
            lock (_syncRoot)
            {
                var temp = _workingQueue;
                _workingQueue = _queue;
                _queue = temp;
            }
            while (_queue.Count > 0)
            {
                var packet = _queue.Dequeue();
                LogPacket(packet.PacketBuffer, packet.RealLength);
                InvokeHandler(packet.PacketHandler, packet.PacketBuffer, packet.RealLength);
            }
        }

        /// <summary>
        /// Determines the correct packet size of the packet, and if there is a packetHandler that will handle this packet.
        /// </summary>
        /// <param name="packetHandlers">List of possible packet handlers for this packet. A packet handler with length of -1 must be first, if any.</param>
        /// <param name="realLength">The real length of the packet.</param>
        /// <returns>True if there is a packetHandler that will handle this packet.</returns>
        bool GetPacketSizeAndHandler(List<PacketHandler> packetHandlers, byte[] buffer, int offset, out int realLength, out PacketHandler packetHandler)
        {
            realLength = 0;
            packetHandler = null;
            if (packetHandlers.Count == 0)
                return false;
            foreach (var ph in packetHandlers)
            {
                if (ph.Length == -1)
                {
                    realLength = buffer[offset + 2] | (buffer[offset + 1] << 8);
                    packetHandler = ph;
                    return true;
                }
                realLength = ph.Length;
                packetHandler = ph;
                return true;
            }
            return false;
        }

        void LogPacket(byte[] buffer, int length, bool servertoclient = true)
        {
            if (UltimaSettings.Debug.LogPackets)
            {
                Utils.Log(servertoclient ? "Server - > Client" : "Client - > Server");
                Utils.Log($"Id: 0x{buffer[0]:X2} Length: {length}");
                Utils.Log($"{Utility.FormatBuffer(buffer, length)}{Environment.NewLine}");
            }
        }

        void InvokeHandler(PacketHandler packetHandler, byte[] buffer, int length)
        {
            if (packetHandler == null)
                return;
            var reader = PacketReader.CreateInstance(buffer, length, packetHandler.Length != -1);
            packetHandler.Invoke(reader);
        }

        List<PacketHandler> GetHandlers(byte cmd, byte subcommand)
        {
            var packetHandlers = new List<PacketHandler>();
            packetHandlers.AddRange(_typedHandlers[cmd]);
            if (_extendedTypedHandlers[cmd] != null)
                packetHandlers.AddRange(_extendedTypedHandlers[cmd][subcommand]);
            return packetHandlers;
        }
    }
}
