using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class MovementRejectPacket : RecvPacket
    {
        readonly byte _sequence;
        readonly short _x;
        readonly short _y;
        readonly byte _direction;
        readonly sbyte _z;

        public byte Sequence 
        {
            get { return _sequence; } 
        }
        
        public short X
        {
            get { return _x; }
        }

        public short Y 
        {
            get { return _y; }
        }
        
        public byte Direction
        {
            get { return _direction; }
        }
        
        public sbyte Z 
        {
            get { return _z; } 
        }

        public MovementRejectPacket(PacketReader reader)
            : base(0x21, "Move Request Rejected")
        {
            _sequence = reader.ReadByte(); // (matches sent sequence)
            _x = reader.ReadInt16();
            _y = reader.ReadInt16();
            _direction = reader.ReadByte();
            _z = reader.ReadSByte();
        }
    }
}
