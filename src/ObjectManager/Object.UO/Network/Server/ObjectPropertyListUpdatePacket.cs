using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class ObjectPropertyListUpdatePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly int _revisionHash;

        public Serial Serial
        {
            get { return _serial; }
        }

        public int RevisionHash 
        {
            get { return _revisionHash; }
        }

        public ObjectPropertyListUpdatePacket(PacketReader reader)
            : base(0xDC, "Object Property List Update")
        {
            _serial = reader.ReadInt32();
            _revisionHash = reader.ReadInt32();
        }
    }
}
