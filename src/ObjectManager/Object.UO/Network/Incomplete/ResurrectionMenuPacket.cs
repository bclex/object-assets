using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class ResurrectionMenuPacket : RecvPacket
    {
        public readonly byte ResurrectionAction;

        public ResurrectionMenuPacket(PacketReader reader)
            : base(0x2C, "Resurrection Menu")
        {
            ResurrectionAction = reader.ReadByte();
            // 0: Server sent
            // 1: Resurrect
            // 2: Ghost
            // The only use on OSI for this packet is now sending "2C02" for the "You Are Dead" screen upon character death.
        }
    }
}
