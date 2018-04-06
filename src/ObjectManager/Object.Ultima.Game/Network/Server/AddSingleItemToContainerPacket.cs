using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class AddSingleItemToContainerPacket : RecvPacket
    {
        public int ItemId { get; private set; }
        public int Amount { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int GridLocation { get; private set; }
        public Serial ContainerSerial { get; private set; }
        public int Hue { get; private set; }
        public Serial Serial { get; private set; }

        public AddSingleItemToContainerPacket(PacketReader reader)
            : base(0x25, "Add Single Item")
        {
            Serial = reader.ReadInt32();
            ItemId = reader.ReadUInt16();
            reader.ReadByte(); // unknown 
            Amount = reader.ReadUInt16();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            if (reader.Buffer.Length == 21) GridLocation = reader.ReadByte(); // always 0 in RunUO.
            else GridLocation = 0;
            ContainerSerial = (Serial)reader.ReadInt32();
            Hue = reader.ReadUInt16();
        }
    }
}
