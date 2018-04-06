using OA.Core;

namespace OA.Tes.FilePacks.Tes3
{
    public class ALCHRecord : Record
    {
        public class ALDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int autoCalc;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                weight = r.ReadLESingle();
                value = r.ReadLEInt32();
                autoCalc = r.ReadLEInt32();
            }
        }

        public class ENAMSubRecord : SubRecord
        {
            public short effectID;
            public byte skillID;
            public byte attributeID;
            public int unknown1;
            public int unknown2;
            public int duration;
            public int magnitude;
            public int unknown4;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                effectID = r.ReadLEInt16();
                skillID = r.ReadByte();
                attributeID = r.ReadByte();
                unknown1 = r.ReadLEInt32();
                unknown2 = r.ReadLEInt32();
                duration = r.ReadLEInt32();
                magnitude = r.ReadLEInt32();
                unknown4 = r.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public ALDTSubRecord ALDT;
        public ENAMSubRecord ENAM;
        public TEXTSubRecord TEXT;
        public SCRISubRecord SCRI;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "ALDT": ALDT = new ALDTSubRecord(); return ALDT;
                case "ENAM": ENAM = new ENAMSubRecord(); return ENAM;
                case "TEXT": TEXT = new TEXTSubRecord(); return TEXT;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                default: return null;
            }
        }
    }
}