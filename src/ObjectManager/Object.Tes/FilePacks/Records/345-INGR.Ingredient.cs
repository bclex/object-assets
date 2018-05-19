using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class INGRRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct IRDTField
        {
            public float Weight;
            public int Value;
            public int[] EffectId; // 0 or -1 means no effect
            public int[] SkillId; // only for Skill related effects, 0 or -1 otherwise
            public int[] AttributeId; // only for Attribute related effects, 0 or -1 otherwise

            public IRDTField(UnityBinaryReader r, uint dataSize)
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

        public override string ToString() => $"INGR: {EDID.Value}";
        public STRVField EDID { get; set; } // Item ID
        public FILEField MODL { get; set; } // Model Name
        public STRVField FNAM; // Item Name
        public IRDTField IRDT; // Ingrediant Data
        public FILEField ITEX; // Inventory Icon
        public STRVField SCRI; // Script Name

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "MODL": MODL = new FILEField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "IRDT": IRDT = new IRDTField(r, dataSize); return true;
                    case "ITEX": ITEX = new FILEField(r, dataSize); return true;
                    case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}