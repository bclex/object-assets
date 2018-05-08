using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    // TODO: implement MAST and DATA subrecords
    public class TES4Record : Record
    {
        public class HEDRSubRecord : SubRecord
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

        public HEDRSubRecord HEDR;
        public CNAMSubRecord CNAM;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName) => throw new NotImplementedException();

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId)
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