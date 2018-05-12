using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    // TODO: implement MAST and DATA subrecords
    public class TES3Record : Record
    {
        public class HEDRField : Field
        {
            public float Version;
            public uint FileType;
            public string CompanyName; // 32 bytes
            public string FileDescription; // 256 bytes
            public uint NumRecords;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Version = r.ReadLESingle();
                FileType = r.ReadLEUInt32();
                CompanyName = r.ReadASCIIString(32);
                FileDescription = r.ReadASCIIString(256);
                NumRecords = r.ReadLEUInt32();
            }
        }

        /*public class MASTField : Field
        {
            public override void Read(UnityBinaryReader r, uint dataSize) { }
        }
        public class DATAField : Field
        {
            public override void Read(UnityBinaryReader r, uint dataSize) { }
        }*/

        public HEDRField HEDR;
        //public MASTField[] MASTSs;
        //public DATAField[] DATAs;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "HEDR": HEDR = new HEDRField(); return HEDR;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}