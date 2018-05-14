using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class MGEFRecord : Record
    {
        public struct MEDTField
        {
            public int SpellSchool; // 0 = Alteration, 1 = Conjuration, 2 = Destruction, 3 = Illusion, 4 = Mysticism, 5 = Restoration
            public float BaseCost;
            public int Flags; // 0x0200 = Spellmaking, 0x0400 = Enchanting, 0x0800 = Negative
            public int Red;
            public int Blue;
            public int Green;
            public float SpeedX;
            public float SizeX;
            public float SizeCap;

            public MEDTField(UnityBinaryReader r, uint dataSize)
            {
                SpellSchool = r.ReadLEInt32();
                BaseCost = r.ReadLESingle();
                Flags = r.ReadLEInt32();
                Red = r.ReadLEInt32();
                Blue = r.ReadLEInt32();
                Green = r.ReadLEInt32();
                SpeedX = r.ReadLESingle();
                SizeX = r.ReadLESingle();
                SizeCap = r.ReadLESingle();
            }
        }

        public override string ToString() => $"MGEF: {INDX.Value}";
        public INTVField INDX; // The Effect ID (0 to 137)
        public MEDTField MEDT; // Effect Data
        public STRVField ITEX; // Effect Icon
        public STRVField PTEX; // Particle texture
        public STRVField CVFX; // Casting visual
        public STRVField BVFX; // Bolt visual
        public STRVField HVFX; // Hit visual
        public STRVField AVFX; // Area visual
        public STRVField DESC; // Description
        public STRVField? CSND; // Cast sound (optional)
        public STRVField? BSND; // Bolt sound (optional)
        public STRVField? HSND; // Hit sound (optional)
        public STRVField? ASND; // Area sound (optional)

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "INDX": INDX = new INTVField(r, dataSize); return true;
                case "MEDT": MEDT = new MEDTField(r, dataSize); return true;
                case "ITEX": ITEX = new STRVField(r, dataSize); return true;
                case "PTEX": PTEX = new STRVField(r, dataSize); return true;
                case "CVFX": CVFX = new STRVField(r, dataSize); return true;
                case "BVFX": BVFX = new STRVField(r, dataSize); return true;
                case "HVFX": HVFX = new STRVField(r, dataSize); return true;
                case "AVFX": AVFX = new STRVField(r, dataSize); return true;
                case "DESC": DESC = new STRVField(r, dataSize); return true;
                case "CSND": CSND = new STRVField(r, dataSize); return true;
                case "BSND": BSND = new STRVField(r, dataSize); return true;
                case "HSND": HSND = new STRVField(r, dataSize); return true;
                case "ASND": ASND = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}