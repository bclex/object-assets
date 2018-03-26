using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class BookHeaderNewChangePacket : SendPacket
    {
        public BookHeaderNewChangePacket(Serial serial, string title, string author)
            : base(0xD4, "Book Header Change (New)")
        {
            Stream.Write(serial);
            Stream.Write((byte)0); // Flag 1 = 0 
            Stream.Write((byte)0); // Flag 2 = 0
            Stream.Write((short)0); // Number of pages = 0
            Stream.Write((short)title.Length);
            Stream.WriteUTF8Fixed(title, title.Length);
            Stream.Write((short)author.Length);
            Stream.WriteUTF8Fixed(author, author.Length);
        }
    }
}
