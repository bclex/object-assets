using System;

namespace OA.Ultima.Core.Network
{
    public abstract class PacketHandler
    {
        public readonly int ID;
        public readonly int Length;
        public readonly Type PacketType;
        public readonly object Client;

        public PacketHandler(int id, int length, Type packetType, object client)
        {
            ID = id;
            Length = length;
            PacketType = packetType;
            Client = client;
        }

        public abstract void Invoke(PacketReader reader);
    }

    public class PacketHandler<T> : PacketHandler where T : IRecvPacket
    {
        readonly Action<T> _handler;

        public PacketHandler(int id, int length, Type packetType, object client, Action<T> handler)
            : base(id, length, packetType, client)
        {
            _handler = handler;
        }

        public override void Invoke(PacketReader reader)
        {
            var packet = (T)Activator.CreateInstance(PacketType, new object[] { reader });
            _handler(packet);
        }
    }
}
