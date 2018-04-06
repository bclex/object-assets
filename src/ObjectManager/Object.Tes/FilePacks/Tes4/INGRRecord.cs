using OA.Core;

namespace OA.Tes.FilePacks.Tes4
{
    public class INGRRecord : Record
    {
        public class IRDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int[] effectID;
            public int[] skillID;
            public int[] attributeID;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                weight = r.ReadLESingle();
                value = r.ReadLEInt32();
                effectID = new int[4];
                for (var i = 0; i < effectID.Length; i++)
                    effectID[i] = r.ReadLEInt32();
                skillID = new int[4];
                for (var i = 0; i < skillID.Length; i++)
                    skillID[i] = r.ReadLEInt32();
                attributeID = new int[4];
                for (var i = 0; i < attributeID.Length; i++)
                    attributeID[i] = r.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public IRDTSubRecord IRDT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "IRDT": IRDT = new IRDTSubRecord(); return IRDT;
                case "ITEX": ITEX = new ITEXSubRecord(); return ITEX;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                default: return null;
            }
        }
    }
}