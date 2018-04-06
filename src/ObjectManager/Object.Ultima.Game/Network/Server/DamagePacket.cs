using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class DamagePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _damage;

        public Serial Serial
        {
            get { return _serial; }
        } 

        public short Damage
        {
            get { return _damage; }
        } 
        
        public DamagePacket(PacketReader reader)
            : base(0x0B, "Damage")
        {
            _serial = reader.ReadInt32();
            _damage = reader.ReadInt16();
        }
    }
}
