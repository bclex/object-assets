using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class MISCRecord : Record
    {
        public class MCDTSubRecord : SubRecord
        {
            public float Weight;
            public uint Value;
            public uint Unknown;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEUInt32();
                Unknown = r.ReadLEUInt32();
            }
        }

        public NAMESubRecord NAME; // door ID
        public MODLSubRecord MODL; // model filename
        public FNAMSubRecord FNAM; // item name
        public MCDTSubRecord MCDT; // misc data
        public ITEXSubRecord ITEX; // inventory icon filename
        public ENAMSubRecord ENAM; // enchantment ID string
        public SCRISubRecord SCRI; // script ID string

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "MCDT": MCDT = new MCDTSubRecord(); return MCDT;
                case "ITEX": ITEX = new ITEXSubRecord(); return ITEX;
                case "ENAM": ENAM = new ENAMSubRecord(); return ENAM;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}