using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class APPARecord : Record, IHaveEDID, IHaveMODL
    {
        // TESX
        public struct DATAField
        {
            public byte Type; // 0 = Mortar and Pestle, 1 = Albemic, 2 = Calcinator, 3 = Retort
            public float Quality;
            public float Weight;
            public int Value;

            public DATAField(UnityBinaryReader r, int dataSize, GameFormatId formatId)
            {
                if (formatId == GameFormatId.TES3)
                {
                    Type = (byte)r.ReadLEInt32();
                    Quality = r.ReadLESingle();
                    Weight = r.ReadLESingle();
                    Value = r.ReadLEInt32();
                    return;
                }
                Type = r.ReadByte();
                Value = r.ReadLEInt32();
                Weight = r.ReadLESingle();
                Quality = r.ReadLESingle();
            }
        }

        public override string ToString() => $"APPA: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model Name
        public STRVField FULL; // Item Name
        public DATAField DATA; // Alchemy Data
        public FILEField ICON; // Inventory Icon
        public FMIDField<SCPTRecord> SCRI; // Script Name

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
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
                case "AADT": DATA = new DATAField(r, dataSize, formatId); return true;
                case "ICON":
                case "ITEX": ICON = new FILEField(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                default: return false;
            }
        }
    }
}