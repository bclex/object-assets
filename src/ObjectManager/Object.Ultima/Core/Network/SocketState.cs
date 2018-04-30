using System.Net.Sockets;

namespace OA.Ultima.Core.Network
{
    public class SocketState
    {
        readonly Socket _socket;
        byte[] _buffer;
        int _dataLength;

        public Socket Socket
        {
            get { return _socket; }
        }

        public byte[] Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        public int DataLength
        {
            get { return _dataLength; }
            set { _dataLength = value; }
        }

        public SocketState(Socket socket, byte[] buffer)
        {
            _socket = socket;
            _buffer = buffer;
            _dataLength = 0;
        }
    }
}
