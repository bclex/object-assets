using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.World.Entities.Mobiles;

namespace OA.Ultima.Network.Server
{
    public class MobileUpdatePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _body;
        readonly short _x;
        readonly short _y;
        readonly short _z;
        readonly byte _direction;
        readonly ushort _hue;
        readonly MobileFlags _flags;

        public Serial Serial
        {
            get { return _serial; }
        }

        public short BodyID
        {
            get { return _body; } 
        }

        public short X 
        {
            get { return _x; }
        }

        public short Y 
        {
            get { return _y; }
        }

        public short Z 
        {
            get { return _z; }
        }

        public byte Direction
        {
            get { return _direction; } 
        }

        public ushort Hue         
        { 
            get { return _hue; }
        }

        public MobileFlags Flags
        {
            get { return _flags; }
        } 

        public MobileUpdatePacket(PacketReader reader)
            : base(0x20, "Mobile Update")
        {
            _serial = reader.ReadInt32();
            _body = reader.ReadInt16();
            reader.ReadByte(); // Always 0
            _hue = reader.ReadUInt16(); // Skin hue
            _flags = new MobileFlags((MobileFlag)reader.ReadByte());
            _x = reader.ReadInt16();
            _y = reader.ReadInt16();
            reader.ReadInt16(); // Always 0
            _direction = reader.ReadByte();
            _z = reader.ReadSByte();
        }
    }
}
