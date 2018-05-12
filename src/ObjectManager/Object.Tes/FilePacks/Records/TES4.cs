using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class TES4Record : Record
    {
        public class HEDRField : Field
        {
            public float Version;
            public int NumRecords;
            public uint NextObjectId;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Version = r.ReadLESingle();
                NumRecords = r.ReadLEInt32();
                NextObjectId = r.ReadLEUInt32();
            }
        }

        public HEDRField HEDR;
        public STRVField CNAM;

        public override Field CreateField(string type) => throw new NotImplementedException();

        public override Field CreateField(string type, GameFormatId gameFormatId)
        {
            switch (type)
            {
                case "HEDR": HEDR = new HEDRField(); return HEDR;
                case "CNAM": CNAM = new STRVField(); return CNAM;
                default: return null;
            }
        }
    }
}