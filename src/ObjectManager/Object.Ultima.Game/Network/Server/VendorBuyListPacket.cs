using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using System.Collections.Generic;

namespace OA.Ultima.Network.Server
{
    public class VendorBuyListPacket : RecvPacket
    {
        public Serial VendorPackSerial { get; private set; }
        public List<VendorBuyItem> Items { get; private set; }

        public VendorBuyListPacket(PacketReader reader)
            : base(0x74, "Open Buy Window")
        {
            VendorPackSerial = reader.ReadInt32();
            var count = reader.ReadByte();
            Items = new List<VendorBuyItem>();
            for (var i = 0; i < count; i++)
            {
                var price = reader.ReadInt32();
                var descriptionLegnth = reader.ReadByte();
                var description = reader.ReadString(descriptionLegnth);
                Items.Add(new VendorBuyItem(price, description));
            }
        }

        public class VendorBuyItem
        {
            public readonly int Price;
            public readonly string Description;

            public VendorBuyItem(int price, string description)
            {
                Price = price;
                Description = description;
            }
        }
    }
}
