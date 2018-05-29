using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class SBSPRecord : Record
    {
        public struct DNAMField
        {
            public float X; // X dimension
            public float Y; // Y dimension
            public float Z; // Z dimension

            public DNAMField(UnityBinaryReader r, int dataSize)
            {
                X = r.ReadLESingle();
                Y = r.ReadLESingle();
                Z = r.ReadLESingle();
            }
        }

        public override string ToString() => $"SBSP: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public DNAMField DNAM;

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "DNAM": DNAM = new DNAMField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}