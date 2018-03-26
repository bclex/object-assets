namespace OA.Ultima.Network.Server.GeneralInfo
{
    internal class SpellbookData
    {
        private Serial serial;
        private ushort itemID;
        private ushort spellbookType;
        private ulong spellBitfields;

        public SpellbookData(Serial serial, ushort itemID, ushort spellbookType, ulong spellBitfields)
        {
            this.serial = serial;
            this.itemID = itemID;
            this.spellbookType = spellbookType;
            this.spellBitfields = spellBitfields;
        }
    }
}