using OA.Core;

namespace OA.Bae.FilePacks
{
    // TODO: implement MAST and DATA subrecords
    public class TES3Record : Record
    {
        public class HEDRSubRecord : SubRecord
        {
            public float version;
            public uint fileType;
            public string companyName; // 32 bytes
            public string fileDescription; // 256 bytes
            public uint numRecords;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                version = r.ReadLESingle();
                fileType = r.ReadLEUInt32();
                companyName = r.ReadASCIIString(32);
                fileDescription = r.ReadASCIIString(256);
                numRecords = r.ReadLEUInt32();
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
    }
}