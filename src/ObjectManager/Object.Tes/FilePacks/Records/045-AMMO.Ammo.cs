using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class AMMORecord : Record, IHaveEDID, IHaveMODL
    {
        public struct DATAField
        {
            public float Speed;
            public uint Flags;
            public uint Value;
            public float Weight;
            public ushort Damage;

            public DATAField(UnityBinaryReader r, int dataSize)
            {
                Speed = r.ReadLESingle();
                Flags = r.ReadLEUInt32();
                Value = r.ReadLEUInt32();
                Weight = r.ReadLESingle();
                Damage = r.ReadLEUInt16();
            }
        }

        public override string ToString() => $"AMMO: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model
        public STRVField FULL; // Item Name
        public FILEField? ICON; // Male Icon (optional)
        public FMIDField<ENCHRecord>? ENAM; // Enchantment ID (optional)
        public IN16Field? ANAM; // Enchantment points (optional)
        public DATAField DATA; // Ammo Data

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                case "FULL": FULL = new STRVField(r, dataSize); return true;
                case "ICON": ICON = new FILEField(r, dataSize); return true;
                case "ENAM": ENAM = new FMIDField<ENCHRecord>(r, dataSize); return true;
                case "ANAM": ANAM = new IN16Field(r, dataSize); return true;
                case "DATA": DATA = new DATAField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}