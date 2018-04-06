using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Data;
using OA.Ultima.Login.Data;
using System;

namespace OA.Ultima.Network.Client
{
    public class CreateCharacterPacket : SendPacket
    {
        internal CreateCharacterPacket(CreateCharacterData data, short locationIndex, short slotNumber, int clientIp)
            : base(0x00, "Create Character", 104)
        {
            var str = (byte)MathHelper.Clamp(data.Attributes[0], 10, 60);
            var dex = (byte)MathHelper.Clamp(data.Attributes[1], 10, 60);
            var intel = (byte)MathHelper.Clamp(data.Attributes[2], 10, 60);
            if (str + dex + intel != 80)
                throw new Exception("Unable to create character with a combined stat total not equal to 80.");
            Stream.Write(0xedededed);
            Stream.Write(0xffffffff);
            Stream.Write((byte)0);
            Stream.WriteAsciiFixed(data.Name, 30);
            Stream.WriteAsciiFixed(string.Empty, 30);
            Stream.Write((byte)((int)(Genders)data.Gender + (int)(Races)0));
            Stream.Write((byte)str);
            Stream.Write((byte)dex);
            Stream.Write((byte)intel);
            Stream.Write((byte)data.SkillIndexes[0]);
            Stream.Write((byte)data.SkillValues[0]);
            Stream.Write((byte)data.SkillIndexes[1]);
            Stream.Write((byte)data.SkillValues[1]);
            Stream.Write((byte)data.SkillIndexes[2]);
            Stream.Write((byte)data.SkillValues[2]);
            Stream.Write((short)data.SkinHue);
            Stream.Write((short)data.HairStyleID);
            Stream.Write((short)data.HairHue);
            Stream.Write((short)data.FacialHairStyleID);
            Stream.Write((short)data.FacialHairHue);
            Stream.Write((short)locationIndex);
            Stream.Write((short)slotNumber);
            Stream.Write((short)0);
            Stream.Write(clientIp);
            Stream.Write((short)data.ShirtColor);
            Stream.Write((short)data.PantsColor);
        }
    }
}
