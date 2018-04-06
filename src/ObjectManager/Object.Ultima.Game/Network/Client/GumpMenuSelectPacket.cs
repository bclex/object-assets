using OA.Ultima.Core.Network.Packets;
using System;

namespace OA.Ultima.Network.Client
{
    public class GumpMenuSelectPacket : SendPacket
    {
        public GumpMenuSelectPacket(int id, int gumpId, int buttonId, int[] switchIds, Tuple<short, string>[] textEntries)
            : base(0xB1, "Gump Menu Select")
        {
            Stream.Write((uint)id);
            Stream.Write((uint)gumpId);
            Stream.Write((uint)buttonId);
            if (switchIds == null)
                Stream.Write((uint)0);
            else
            {
                Stream.Write((uint)switchIds.Length);
                for (var i = 0; i < switchIds.Length; i++)
                    Stream.Write((uint)switchIds[i]);
            }
            if (textEntries == null)
                Stream.Write((uint)0);
            else
            {
                Stream.Write((uint)textEntries.Length);
                for (var i = 0; i < textEntries.Length; i++)
                {
                    var length = textEntries[i].Item2.Length * 2;
                    Stream.Write((ushort)textEntries[i].Item1);
                    Stream.Write((ushort)length);
                    Stream.WriteBigUniFixed(textEntries[i].Item2, textEntries[i].Item2.Length);
                }
            }
        }
    }
}
