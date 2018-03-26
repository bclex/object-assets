using OA.Ultima.Core.Network;

namespace OA.Ultima.Login.Accounts
{
    public class CharacterListEntry
    {
        readonly string name;
        readonly string password;

        public string Name
        {
            get { return name; }
        }

        public string Password
        {
            get { return password; }
        }

        public CharacterListEntry(PacketReader reader)
        {
            name = reader.ReadString(30);
            password = reader.ReadString(30);
        }
    }
}
