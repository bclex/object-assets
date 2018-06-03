using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class INGRRecord : Record, IHaveEDID, IHaveMODL
    {
        // TES3
        public struct IRDTField
        {
            public float Weight;
            public int Value;
            public int[] EffectId; // 0 or -1 means no effect
            public int[] SkillId; // only for Skill related effects, 0 or -1 otherwise
            public int[] AttributeId; // only for Attribute related effects, 0 or -1 otherwise

            public IRDTField(UnityBinaryReader r, int dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                EffectId = new int[4];
                for (var i = 0; i < EffectId.Length; i++)
                    EffectId[i] = r.ReadLEInt32();
                SkillId = new int[4];
                for (var i = 0; i < SkillId.Length; i++)
                    SkillId[i] = r.ReadLEInt32();
                AttributeId = new int[4];
                for (var i = 0; i < AttributeId.Length; i++)
                    AttributeId[i] = r.ReadLEInt32();
            }
        }

        // TES4
        public class DATAField
        {
            public float Weight;
            public int Value;
            public uint Flags;

            public DATAField(UnityBinaryReader r, int dataSize)
            {
                Weight = r.ReadLESingle();
            }

            public void ENITField(UnityBinaryReader r, int dataSize)
            {
                Value = r.ReadLEInt32();
                Flags = r.ReadLEUInt32();
            }
        }

        public override string ToString() => $"INGR: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model Name
        public STRVField FULL; // Item Name
        public IRDTField IRDT; // Ingrediant Data //: TES3
        public DATAField DATA; // Ingrediant Data //: TES4
        public FILEField ICON; // Inventory Icon
        public FMIDField<SCPTRecord> SCRI; // Script Name
        // TES4
        public List<ENCHRecord.EFITField> EFITs = new List<ENCHRecord.EFITField>(); // Effect Data
        public List<ENCHRecord.SCITField> SCITs = new List<ENCHRecord.SCITField>(); // Script effect data

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
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
                case "DATA": DATA = new DATAField(r, dataSize); return true;
                case "IRDT": IRDT = new IRDTField(r, dataSize); return true;
                case "ICON":
                case "ITEX": ICON = new FILEField(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                    //
                case "ENIT": DATA.ENITField(r, dataSize); return true;
                case "EFID": r.SkipBytes(dataSize); return true;
                case "EFIT": EFITs.Add(new ENCHRecord.EFITField(r, dataSize, format)); return true;
                case "SCIT": SCITs.Add(new ENCHRecord.SCITField(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}