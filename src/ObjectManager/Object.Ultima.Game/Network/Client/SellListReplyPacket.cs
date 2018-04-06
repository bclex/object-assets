using OA.Ultima.Core.Network.Packets;
using System;

namespace OA.Ultima.Network.Client
{
    public class SellListReplyPacket : SendPacket
    {
        public SellListReplyPacket(Serial vendorSerial, Tuple<int, short>[] items)
            : base(0x9F, "Sell List Reply")
        {
            Stream.Write(vendorSerial);
            Stream.Write((short)items.Length);
            for (var i = 0; i < items.Length; i++)
            {
                Stream.Write(items[i].Item1);
                Stream.Write((short)items[i].Item2);
            }
        }
    }
}
