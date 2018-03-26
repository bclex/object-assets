using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Login.Accounts;

namespace OA.Ultima.Network.Server
{
    public class CharacterCityListPacket : RecvPacket
    {
        readonly StartingLocation[] _locations;
        readonly CharacterListEntry[] _characters;

        public StartingLocation[] Locations
        {
            get { return _locations; }
        }

        public CharacterListEntry[] Characters
        {
            get { return _characters; }
        }

        public CharacterCityListPacket(PacketReader reader)
            : base(0xA9, "Char/City List")
        {
            var characterCount = reader.ReadByte();
            _characters = new CharacterListEntry[characterCount];
            for (var i = 0; i < characterCount; i++)
                _characters[i] = new CharacterListEntry(reader);
            var locationCount = reader.ReadByte();
            _locations = new StartingLocation[locationCount];
            for (var i = 0; i < locationCount; i++)
                _locations[i] = new StartingLocation(reader);
        }

        public class StartingLocation
        {
            readonly byte index;
            readonly string cityName;
            readonly string areaOfCityOrTown;

            public byte Index
            {
                get { return index; }
            }

            public string CityName
            {
                get { return cityName; }
            }

            public string AreaOfCityOrTown
            {
                get { return areaOfCityOrTown; }
            }

            public StartingLocation(PacketReader reader)
            {
                index = reader.ReadByte();
                cityName = reader.ReadString(31);
                areaOfCityOrTown = reader.ReadString(31);
            }
        }
    }
}
