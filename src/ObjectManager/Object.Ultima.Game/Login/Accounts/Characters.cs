using OA.Ultima.Network.Server;

namespace OA.Ultima.Login.Accounts
{
    public static class Characters
    {
        static CharacterListEntry[] _characters;
        public static CharacterListEntry[] List { get { return _characters; } }
        public static int Length { get { return _characters.Length; } }

        static CharacterCityListPacket.StartingLocation[] _locations;
        public static CharacterCityListPacket.StartingLocation[] StartingLocations { get { return _locations; } }

        static int _updateValue;
        public static int UpdateValue { get { return _updateValue; } }

        public static int FirstEmptySlot
        {
            get
            {
                for (var i = 0; i < _characters.Length; i++)
                    if (_characters[i].Name == string.Empty)
                        return i;
                return -1;
            }
        }

        public static void SetCharacterList(CharacterListEntry[] list)
        {
            _characters = list;
            _updateValue++;
        }

        public static void SetStartingLocations(CharacterCityListPacket.StartingLocation[] list)
        {
            _locations = list;
            _updateValue++;
        }
    }
}
