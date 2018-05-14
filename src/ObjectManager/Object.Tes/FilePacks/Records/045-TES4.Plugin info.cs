using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class TES4Record : Record
    {
        public struct HEDRField
        {
            public float Version;
            public int NumRecords;
            public uint NextObjectId;

            public HEDRField(UnityBinaryReader r, uint dataSize)
            {
                Version = r.ReadLESingle();
                NumRecords = r.ReadLEInt32();
                NextObjectId = r.ReadLEUInt32();
            }
        }

        public HEDRField HEDR;
        public STRVField CNAM;

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize) => throw new NotImplementedException();

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "HEDR": HEDR = new HEDRField(r, dataSize); return true;
                case "CNAM": CNAM = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}