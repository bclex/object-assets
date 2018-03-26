using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class RequestSkillUsePacket : SendPacket
    {
        public RequestSkillUsePacket(int id)
            : base(0x12, "Request Skill Use")
        {
            Stream.Write((byte)0x24);
            Stream.WriteAsciiNull(string.Format("{0} 0", id));
        }
    }
}
