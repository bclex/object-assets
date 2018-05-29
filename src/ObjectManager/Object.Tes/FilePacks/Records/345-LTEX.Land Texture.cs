using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class LTEXRecord : Record, IHaveEDID
    {
        public struct HNAMField
        {
            public byte MaterialType;
            public byte Friction;
            public byte Restitution;
           
            public HNAMField(UnityBinaryReader r, int dataSize)
            {
                MaterialType = r.ReadByte();
                Friction = r.ReadByte();
                Restitution = r.ReadByte();
            }
        }

        public override string ToString() => $"LTEX: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public FILEField ICON; // Texture
        // TES3
        public INTVField INTV;
        // TES4
        public HNAMField HNAM; // Havok data
        public BYTEField SNAM; // Texture specular exponent
        public List<FMIDField<GRASRecord>> GNAMs = new List<FMIDField<GRASRecord>>(); // Potential grass

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "INTV": INTV = new INTVField(r, dataSize); return true;
                case "ICON":
                case "DATA": ICON = new FILEField(r, dataSize); return true;
                // TES4
                case "HNAM": HNAM = new HNAMField(r, dataSize); return true;
                case "SNAM": SNAM = new BYTEField(r, dataSize); return true;
                case "GNAM": GNAMs.Add(new FMIDField<GRASRecord>(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}