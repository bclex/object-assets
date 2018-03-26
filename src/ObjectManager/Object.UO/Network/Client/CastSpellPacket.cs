using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class CastSpellPacket : SendPacket
    {
        public CastSpellPacket(int spellIndex)
            : base(0xBF, "Cast Spell")
        {
            Stream.Write((short)0x001C); // subcommand 0x1C - cast spell
            Stream.Write((short)0x0002); // unknown - always 2 in legacy client.
            Stream.Write((short)spellIndex);
        }
    }
}
