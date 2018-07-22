using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class TES4Record : Record
    {
        public struct HEDRField
        {
            public float Version;
            public int NumRecords; // Number of records and groups (not including TES4 record itself).
            public uint NextObjectId; // Next available object ID.

            public HEDRField(UnityBinaryReader r, int dataSize)
            {
                Version = r.ReadLESingle();
                NumRecords = r.ReadLEInt32();
                NextObjectId = r.ReadLEUInt32();
            }
        }

        public HEDRField HEDR;
        public STRVField? CNAM; // author (Optional)
        public STRVField? SNAM; // description (Optional)
        public List<STRVField> MASTs; // master
        public List<INTVField> DATAs; // fileSize
        public UNKNField? ONAM; // overrides (Optional)
        public IN32Field INTV; // unknown
        public IN32Field? INCC; // unknown (Optional)

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "HEDR": HEDR = new HEDRField(r, dataSize); return true;
                case "OFST": r.SkipBytes(dataSize); return true;
                case "DELE": r.SkipBytes(dataSize); return true;
                case "CNAM": CNAM = r.ReadSTRV(dataSize); return true;
                case "SNAM": SNAM = r.ReadSTRV(dataSize); return true;
                case "MAST": if (MASTs == null) MASTs = new List<STRVField>(); MASTs.Add(r.ReadSTRV(dataSize)); return true;
                case "DATA": if (DATAs == null) DATAs = new List<INTVField>(); DATAs.Add(r.ReadINTV(dataSize)); return true;
                case "ONAM": ONAM = r.ReadUNKN(dataSize); return true;
                case "INTV": INTV = r.ReadT<IN32Field>(dataSize); return true;
                case "INCC": INCC = r.ReadT<IN32Field>(dataSize); return true;
                default: return false;
            }
        }
    }
}