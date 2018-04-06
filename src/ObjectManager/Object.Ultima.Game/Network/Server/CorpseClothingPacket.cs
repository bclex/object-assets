using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using System.Collections.Generic;

namespace OA.Ultima.Network.Server
{
    public class CorpseClothingPacket : RecvPacket
    {
        public readonly Serial CorpseSerial;
        public readonly List<CorpseItem> Items = new List<CorpseItem>();
        public CorpseClothingPacket(PacketReader reader)
            : base(0x89, "Corpse Clothing")
        {
            CorpseSerial = reader.ReadInt32(); // BYTE[4] corpseID
            var hasItems = true;
            while (hasItems)
            {
                var layer = reader.ReadByte();
                if (layer == 0x00)
                    hasItems = false;
                else
                {
                    var itemSerial = reader.ReadInt32();
                    Items.Add(new CorpseItem(layer, itemSerial));
                }
            }
        }

        public struct CorpseItem
        {
            public int Layer;
            public Serial Serial;

            public CorpseItem(int layer, Serial serial)
            {
                Layer = layer;
                Serial = serial;
            }
        }
    }
}
