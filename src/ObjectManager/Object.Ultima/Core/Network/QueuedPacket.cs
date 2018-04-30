namespace OA.Ultima.Core.Network
{
    class QueuedPacket
    {
        public PacketHandler PacketHandler;
        public byte[] PacketBuffer;
        public int RealLength;

        public QueuedPacket(PacketHandler packetHandler, byte[] packetBuffer, int realLength)
        {
            PacketHandler = packetHandler;
            PacketBuffer = packetBuffer;
            RealLength = realLength;
        }
    }
}
