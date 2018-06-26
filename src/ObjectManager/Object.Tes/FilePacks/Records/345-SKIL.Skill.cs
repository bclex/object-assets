using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class SKILRecord : Record
    {
        // TESX
        public struct DATAField
        {
            public int Action;
            public int Attribute;
            public uint Specialization; // 0 = Combat, 1 = Magic, 2 = Stealth
            public float[] UseValue; // The use types for each skill are hard-coded.

            public DATAField(UnityBinaryReader r, int dataSize, GameFormatId format)
            {
                Action = format == GameFormatId.TES3 ? 0 : r.ReadLEInt32();
                Attribute = r.ReadLEInt32();
                Specialization = r.ReadLEUInt32();
                UseValue = new float[format == GameFormatId.TES3 ? 4 : 2];
                for (var i = 0; i < UseValue.Length; i++) UseValue[i] = r.ReadLESingle();
            }
        }

        public override string ToString() => $"SKIL: {INDX.Value}:{EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public IN32Field INDX; // Skill ID
        public DATAField DATA; // Skill Data
        public STRVField DESC; // Skill description
        // TES4
        public FILEField ICON; // Icon
        public STRVField ANAM; // Apprentice Text
        public STRVField JNAM; // Journeyman Text
        public STRVField ENAM; // Expert Text
        public STRVField MNAM; // Master Text

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "INDX": INDX = new IN32Field(r, dataSize); return true;
                case "DATA":
                case "SKDT": DATA = new DATAField(r, dataSize, format); return true;
                case "DESC": DESC = new STRVField(r, dataSize); return true;
                case "ICON": ICON = new FILEField(r, dataSize); return true;
                case "ANAM": ANAM = new STRVField(r, dataSize); return true;
                case "JNAM": JNAM = new STRVField(r, dataSize); return true;
                case "ENAM": ENAM = new STRVField(r, dataSize); return true;
                case "MNAM": MNAM = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}