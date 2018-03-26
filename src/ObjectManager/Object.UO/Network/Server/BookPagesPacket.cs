using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.World.Entities.Items;

namespace OA.Ultima.Network.Server
{
    public class BookPagesPacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly int PageCount;
        public readonly BaseBook.BookPageInfo[] Pages;

        public BookPagesPacket(PacketReader reader)
            : base(0x66, "Book Pages")
        {
            Serial = reader.ReadInt32();
            PageCount = reader.ReadInt16();
            Pages = new BaseBook.BookPageInfo[PageCount];
            for (var i = 0; i < PageCount; ++i)
            {
                var page = reader.ReadInt16();
                var length = reader.ReadInt16();
                var lines = new string[length];
                for (var j = 0; j < length; j++)
                    lines[j] = reader.ReadString();
                Pages[i] = new BaseBook.BookPageInfo(lines);
            }
        }
    }
}
