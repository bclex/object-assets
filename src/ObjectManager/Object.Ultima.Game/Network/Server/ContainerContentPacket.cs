using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Data;
using System.Collections.Generic;

namespace OA.Ultima.Network.Server
{
    public class ContainerContentPacket : RecvPacket
    {
        ItemInContainer[] _items;

        public ItemInContainer[] Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public ContainerContentPacket(PacketReader reader)
            : base(0x3C, "Container ContentPacket")
        {
            var itemCount = reader.ReadUInt16();
            var items = new List<ItemInContainer>(itemCount);
            var PacketIsPre6017 = (reader.Buffer.Length == 5 + (19 * itemCount));
            for (var i = 0; i < itemCount; i++)
            {
                var serial = reader.ReadInt32();
                var iItemID = reader.ReadUInt16();
                var iUnknown = reader.ReadByte(); // signed, itemID offset. always 0 in RunUO.
                var iAmount = reader.ReadUInt16();
                var iX = reader.ReadInt16();
                var iY = reader.ReadInt16();
                var iGridLocation = 0;
                if (!PacketIsPre6017)
                    iGridLocation = reader.ReadByte(); // always 0 in RunUO.
                var iContainerSerial = reader.ReadInt32();
                var iHue = reader.ReadUInt16();
                items.Add(new ItemInContainer(serial, iItemID, iAmount, iX, iY, iGridLocation, iContainerSerial, iHue));
            }
            _items = items.ToArray();
        }

        public bool AllItemsInSameContainer
        {
            get
            {
                if (Items.Length == 0)
                    return true;
                var containerSerial = Items[0].ContainerSerial;
                for (var i = 1; i < Items.Length; i++)
                    if (Items[i].ContainerSerial != containerSerial)
                        return false;
                return true;
            }
        }
    }
}
