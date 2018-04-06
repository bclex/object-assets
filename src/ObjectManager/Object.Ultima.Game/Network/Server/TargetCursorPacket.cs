using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class TargetCursorPacket : RecvPacket
    {
        public readonly byte CommandType;
        public readonly int CursorID;
        public readonly byte CursorType;
        
        public TargetCursorPacket(PacketReader reader)
            : base(0x6C, "Target Cursor")
        {
            CommandType = reader.ReadByte(); // 0x00 = Select Object; 0x01 = Select X, Y, Z
            CursorID = reader.ReadInt32();
            CursorType = reader.ReadByte(); // 0 - 2 = unknown; 3 = Cancel current targetting RunUO seems to always send 0.
        }
    }
}
