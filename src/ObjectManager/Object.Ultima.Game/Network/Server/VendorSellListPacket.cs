using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class VendorSellListPacket : RecvPacket
    {
        public readonly Serial VendorSerial;
        public readonly VendorSellItem[] Items;
        public VendorSellListPacket(PacketReader reader)
            : base(0x9E, "Vendor Sell List")
        {
            VendorSerial = (Serial)reader.ReadInt32();
            var numItems = reader.ReadUInt16();
            Items = new VendorSellItem[numItems];
            for (var i = 0; i < numItems; i++)
                Items[i] = new VendorSellItem(reader);
        }

        public struct VendorSellItem
        {
            public readonly Serial ItemSerial;
            public readonly ushort ItemID;
            public readonly ushort Hue;
            public readonly ushort Amount;
            public readonly ushort Price;
            public readonly string Name;

            public VendorSellItem(PacketReader reader)
            {
                ItemSerial = reader.ReadInt32();
                ItemID = reader.ReadUInt16();
                Hue = reader.ReadUInt16();
                Amount = reader.ReadUInt16();
                Price = reader.ReadUInt16();
                var nameLength = reader.ReadUInt16();
                Name = reader.ReadString(nameLength);
            }
        }
    }
}
