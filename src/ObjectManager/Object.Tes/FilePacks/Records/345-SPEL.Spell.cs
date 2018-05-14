using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class SPELRecord : Record
    {
        public struct SPDTField
        {
            public override string ToString() => $"{Type}";
            public uint Type; // 0 = Spell, 1 = Ability, 2 = Blight, 3 = Disease, 4 = Curse, 5 = Power
            public int SpellCost;
            public uint Flags; // 0x0001 = AutoCalc, 0x0002 = PC Start, 0x0004 = Always Succeeds

            public SPDTField(UnityBinaryReader r, uint dataSize)
            {
                Type = r.ReadLEUInt32();
                SpellCost = r.ReadLEInt32();
                Flags = r.ReadLEUInt32();
            }
        }

        public struct ENAMField
        {
            public byte[] Data;

            public ENAMField(UnityBinaryReader r, uint dataSize)
            {
                Data = r.ReadBytes((int)dataSize);
            }
        }

        public override string ToString() => $"SPEL: {EDID.Value}";
        public STRVField EDID { get; set; }
        public STRVField FNAM;
        public SPDTField SPDT;
        public ENAMField ENAM;

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "SPDT": SPDT = new SPDTField(r, dataSize); return true;
                case "ENAM": ENAM = new ENAMField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}