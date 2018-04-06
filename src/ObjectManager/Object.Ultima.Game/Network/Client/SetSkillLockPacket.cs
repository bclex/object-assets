using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class SetSkillLockPacket : SendPacket
    {
        public SetSkillLockPacket(ushort skillIndex, byte lockType)
            : base(0x3A, "Set skill lock")
        {
            Stream.Write(skillIndex);
            Stream.Write(lockType);
        }
    }
}
