using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class BookHeaderNewPacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly byte Flag0;
        public readonly byte Flag1;
        public readonly short Pages;
        public readonly short AuthorLength;
        public readonly string Author;
        public readonly short TitleLength;
        public readonly string Title;

        public BookHeaderNewPacket(PacketReader reader)
            : base(0xD4, "Book Header (New)")
        {
            Serial = reader.ReadInt32();
            Flag0 = reader.ReadByte();
            Flag1 = reader.ReadByte();
            Pages = reader.ReadInt16();
            TitleLength = reader.ReadInt16();
            Title = reader.ReadString();
            AuthorLength = reader.ReadInt16();
            Author = reader.ReadString();
        }
    }
}
