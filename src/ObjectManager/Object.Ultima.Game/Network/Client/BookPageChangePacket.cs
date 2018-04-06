using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class BookPageChangePacket : SendPacket
    {
        public BookPageChangePacket(Serial serial, int page, string[] lines)
            : base(0x66, "Book Page Change")
        {
            Stream.Write(serial);
            Stream.Write((short)1); // Page count always 1
            Stream.Write((short)(page + 1)); // Page number
            Stream.Write((short)lines.Length); // Number of lines
            // Send each line of the page
            for (var i = 0; i < lines.Length; i++)
                Stream.WriteUTF8Null(lines[i]);
        }
    }
}
