using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class CLOTRecord : Record, IHaveEDID, IHaveMODL
    {
        // TESX
        public struct DATAField
        {
            public enum CLOTType
            {
                Pants = 0, Shoes, Shirt, Belt, Robe, R_Glove, L_Glove, Skirt, Ring, Amulet
            }

            public int Value;
            public float Weight;
            //
            public int Type;
            public short EnchantPts;

            public DATAField(UnityBinaryReader r, int dataSize, GameFormatId format)
            {
                if (format == GameFormatId.TES3)
                {
                    Type = r.ReadLEInt32();
                    Weight = r.ReadLESingle();
                    Value = r.ReadLEInt16();
                    EnchantPts = r.ReadLEInt16();
                    return;
                }
                Value = r.ReadLEInt32();
                Weight = r.ReadLESingle();
                Type = 0;
                EnchantPts = 0;
            }
        }

        public class INDXFieldGroup
        {
            public override string ToString() => $"{INDX.Value}: {BNAM.Value}";
            public INTVField INDX;
            public STRVField BNAM;
            public STRVField CNAM;
        }

        public override string ToString() => $"CLOT: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model Name
        public STRVField FULL; // Item Name
        public DATAField DATA; // Clothing Data
        public FILEField ICON; // Male Icon
        public STRVField ENAM; // Enchantment Name
        public FMIDField<SCPTRecord> SCRI; // Script Name
        // TES3
        public List<INDXFieldGroup> INDXs = new List<INDXFieldGroup>(); // Body Part Index (Moved to Race)
        // TES4
        public UI32Field BMDT; // Clothing Flags
        public MODLGroup MOD2; // Male world model (optional)
        public MODLGroup MOD3; // Female biped (optional)
        public MODLGroup MOD4; // Female world model (optional)
        public FILEField? ICO2; // Female icon (optional)
        public IN16Field? ANAM; // Enchantment points (optional)

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                case "FULL":
                case "FNAM": FULL = new STRVField(r, dataSize); return true;
                case "DATA":
                case "CTDT": DATA = new DATAField(r, dataSize, format); return true;
                case "ICON":
                case "ITEX": ICON = new FILEField(r, dataSize); return true;
                case "INDX": INDXs.Add(new INDXFieldGroup { INDX = new INTVField(r, dataSize) }); return true;
                case "BNAM": INDXs.Last().BNAM = new STRVField(r, dataSize); return true;
                case "CNAM": INDXs.Last().CNAM = new STRVField(r, dataSize); return true;
                case "ENAM": ENAM = new STRVField(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                case "BMDT": BMDT = new UI32Field(r, dataSize); return true;
                case "MOD2": MOD2 = new MODLGroup(r, dataSize); return true;
                case "MO2B": MOD2.MODBField(r, dataSize); return true;
                case "MO2T": MOD2.MODTField(r, dataSize); return true;
                case "MOD3": MOD3 = new MODLGroup(r, dataSize); return true;
                case "MO3B": MOD3.MODBField(r, dataSize); return true;
                case "MO3T": MOD3.MODTField(r, dataSize); return true;
                case "MOD4": MOD4 = new MODLGroup(r, dataSize); return true;
                case "MO4B": MOD4.MODBField(r, dataSize); return true;
                case "MO4T": MOD4.MODTField(r, dataSize); return true;
                case "ICO2": ICO2 = new FILEField(r, dataSize); return true;
                case "ANAM": ANAM = new IN16Field(r, dataSize); return true;
                default: return false;
            }
        }
    }
}