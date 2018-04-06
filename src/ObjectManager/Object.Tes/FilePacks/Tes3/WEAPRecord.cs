using OA.Core;

namespace OA.Tes.FilePacks.Tes3
{
    public class WEAPRecord : Record
    {
        public class WPDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public short type;
            public short health;
            public float speed;
            public float reach;
            public short enchantPts;
            public byte chopMin;
            public byte chopMax;
            public byte slashMin;
            public byte slashMax;
            public byte thrustMin;
            public byte thrustMax;
            public int flags;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                weight = r.ReadLESingle();
                value = r.ReadLEInt32();
                type = r.ReadLEInt16();
                health = r.ReadLEInt16();
                speed = r.ReadLESingle();
                reach = r.ReadLESingle();
                enchantPts = r.ReadLEInt16();
                chopMin = r.ReadByte();
                chopMax = r.ReadByte();
                slashMin = r.ReadByte();
                slashMax = r.ReadByte();
                thrustMin = r.ReadByte();
                thrustMax = r.ReadByte();
                flags = r.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public WPDTSubRecord WPDT;
        public ITEXSubRecord ITEX;
        public ENAMSubRecord ENAM;
        public SCRISubRecord SCRI;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "WPDT": WPDT = new WPDTSubRecord(); return WPDT;
                case "ITEX": ITEX = new ITEXSubRecord(); return ITEX;
                case "ENAM": ENAM = new ENAMSubRecord(); return ENAM;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                default: return null;
            }
        }
    }
}