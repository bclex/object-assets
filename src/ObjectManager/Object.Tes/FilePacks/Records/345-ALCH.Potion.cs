using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class ALCHRecord : Record, IHaveEDID, IHaveMODL
    {
        // TESX
        public class DATAField
        {
            public float Weight;
            public int Value;
            public int Flags; //: AutoCalc

            public DATAField(UnityBinaryReader r, int dataSize, GameFormatId formatId)
            {
                Weight = r.ReadLESingle();
                if (formatId == GameFormatId.TES3)
                {
                    Value = r.ReadLEInt32();
                    Flags = r.ReadLEInt32();
                }
            }

            public void ENITField(UnityBinaryReader r, int dataSize)
            {
                Value = r.ReadLEInt32();
                Flags = r.ReadByte();
                r.ReadBytes(3); // Unknown
            }
        }

        // TES3
        public struct ENAMField
        {
            public short EffectId;
            public byte SkillId; // for skill related effects, -1/0 otherwise
            public byte AttributeId; // for attribute related effects, -1/0 otherwise
            public int Unknown1;
            public int Unknown2;
            public int Duration;
            public int Magnitude;
            public int Unknown4;

            public ENAMField(UnityBinaryReader r, int dataSize)
            {
                EffectId = r.ReadLEInt16();
                SkillId = r.ReadByte();
                AttributeId = r.ReadByte();
                Unknown1 = r.ReadLEInt32();
                Unknown2 = r.ReadLEInt32();
                Duration = r.ReadLEInt32();
                Magnitude = r.ReadLEInt32();
                Unknown4 = r.ReadLEInt32();
            }
        }

        public override string ToString() => $"ALCH: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model
        public STRVField FULL; // Item Name
        public DATAField DATA; // Alchemy Data
        public ENAMField? ENAM; // Enchantment
        public FILEField ICON; // Icon
        public FMIDField<SCPTRecord>? SCRI; // Script (optional)
        // TES4
        public List<ENCHRecord.EFITField> EFITs = new List<ENCHRecord.EFITField>(); // Effect Data
        public List<ENCHRecord.SCITField> SCITs = new List<ENCHRecord.SCITField>(); // Script Effect Data

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                case "FULL": if (SCITs.Count == 0) FULL = new STRVField(r, dataSize); else ArrayUtils.Last(SCITs).FULLField(r, dataSize); return true;
                case "FNAM": FULL = new STRVField(r, dataSize); return true;
                case "DATA":
                case "ALDT": DATA = new DATAField(r, dataSize, formatId); return true;
                case "ENAM": ENAM = new ENAMField(r, dataSize); return true;
                case "ICON":
                case "TEXT": ICON = new FILEField(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                //
                case "ENIT": DATA.ENITField(r, dataSize); return true;
                case "EFID": r.ReadBytes((int)dataSize); return true;
                case "EFIT": EFITs.Add(new ENCHRecord.EFITField(r, dataSize, formatId)); return true;
                case "SCIT": SCITs.Add(new ENCHRecord.SCITField(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}