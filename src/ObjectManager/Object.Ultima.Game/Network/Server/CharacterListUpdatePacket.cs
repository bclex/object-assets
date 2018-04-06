using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Login.Accounts;

namespace OA.Ultima.Network.Server
{
    public class CharacterListUpdatePacket : RecvPacket
    {
        readonly CharacterListEntry[] _characters;
        public CharacterListEntry[] Characters
        {
            get { return _characters; }
        }

        public CharacterListUpdatePacket(PacketReader reader)
            : base(0x86, "Character List Update")
        {
            // Documented at http://docs.polserver.com/packets/index.php?Packet=0xA8
            var characterCount = reader.ReadByte();
            _characters = new CharacterListEntry[characterCount];
            for (var i = 0; i < characterCount; i++)
                _characters[i] = new CharacterListEntry(reader);
        }
    }
}
