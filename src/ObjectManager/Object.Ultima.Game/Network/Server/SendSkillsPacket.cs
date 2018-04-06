using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class SendSkillsPacket : RecvPacket
    {
        public readonly int PacketType;
        public readonly SendSkillsPacket_SkillEntry[] Skills;

        public SendSkillsPacket(PacketReader reader)
            : base(0x3A, "Send Skills List")
        {
            PacketType = reader.ReadByte();
            var hasSkillCap = (PacketType == 0x02 || PacketType == 0xDF);
            var numSkills = (reader.Size - reader.Index - (PacketType == 0x00 ? 2 : 0)) / (hasSkillCap ? 9 : 7);
            Skills = new SendSkillsPacket_SkillEntry[numSkills];
            for (var i = 0; i < numSkills; i++)
            {
                Skills[i] = new SendSkillsPacket_SkillEntry(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadByte());
                if (hasSkillCap)
                    Skills[i].SetSkillCap(reader.ReadUInt16());
            }
            // 0x00: Full List of skills
            // 0xFF: Single skill update
            // 0x02: Full List of skills with skill cap for each skill
            // 0xDF: Single skill update with skill cap for skill
        }
    }

    public class SendSkillsPacket_SkillEntry
    {
        public int SkillID;
        public float SkillValue;
        public float SkillValueUnmodified;
        public byte SkillLock;
        public float SkillCap;

        public SendSkillsPacket_SkillEntry(ushort id, ushort value, ushort unmodified, byte lockvalue)
        {
            SkillID = id;
            SkillValue = (float)value / 10f;
            SkillValueUnmodified = (float)unmodified / 10f;
            SkillLock = lockvalue;
            SkillCap = 100.0f;
        }

        public void SetSkillCap(ushort value)
        {
            SkillCap = (float)value / 10f;
        }
    }
}
