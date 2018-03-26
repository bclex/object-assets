using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class PersonalLightLevelPacket : RecvPacket
    {
        readonly Serial _creatureSerial;
        readonly byte _lightLevel;

        public Serial CreatureSerial
        {
            get { return _creatureSerial; }
        }

        public byte LightLevel
        {
            get { return _lightLevel; }
        }

        public PersonalLightLevelPacket(PacketReader reader)
            : base(0x4E, "Personal Light Level")
        {
            _creatureSerial = reader.ReadInt32();
            _lightLevel = reader.ReadByte();
        }
    }
}
