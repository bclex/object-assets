using OA.Core;

namespace OA.Bae.FilePacks
{
    // TODO: implement MAST and DATA subrecords
    public class TES4Record : Record
    {
        public class HEDRSubRecord : SubRecord
        {
            public float version;
            public int numRecords;
            public uint nextObjectId;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                version = r.ReadLESingle();
                numRecords = r.ReadLEInt32();
                nextObjectId = r.ReadLEUInt32();
            }
        }

        public HEDRSubRecord HEDR;
        public CNAMSubRecord CNAM;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "HEDR": HEDR = new HEDRSubRecord(); return HEDR;
                case "CNAM": CNAM = new CNAMSubRecord(); return CNAM;
                default: return null;
            }
        }
    }
}