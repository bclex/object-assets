using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class TES3Record : Record
    {
        public struct HEDRField
        {
            public float Version;
            public uint FileType;
            public string CompanyName;
            public string FileDescription;
            public uint NumRecords;

            public HEDRField(UnityBinaryReader r, uint dataSize)
            {
                Version = r.ReadLESingle();
                FileType = r.ReadLEUInt32();
                CompanyName = r.ReadASCIIString(32, ASCIIFormat.ZeroPadded);
                FileDescription = r.ReadASCIIString(256, ASCIIFormat.ZeroPadded);
                NumRecords = r.ReadLEUInt32();
            }
        }

        public HEDRField HEDR;
        public List<STRVField> MASTs;
        public List<INTVField> DATAs;

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "HEDR": HEDR = new HEDRField(r, dataSize); return true;
                case "MAST": if (MASTs == null) MASTs = new List<STRVField>(); MASTs.Add(new STRVField(r, dataSize)); return true;
                case "DATA": if (DATAs == null) DATAs = new List<INTVField>(); DATAs.Add(new INTVField(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}