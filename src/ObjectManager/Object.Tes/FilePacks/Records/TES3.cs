using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    // TODO: implement MAST and DATA subrecords
    public class TES3Record : Record
    {
        public class HEDRSubRecord : SubRecord
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

        /*public class MASTSubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader r) { }
        }
        public class DATASubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader r) { }
        }*/

        public HEDRSubRecord HEDR;
        //public MASTSubRecord[] MASTSs;
        //public DATASubRecord[] DATAs;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "HEDR": HEDR = new HEDRSubRecord(); return HEDR;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}