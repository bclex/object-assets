using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class TREERecord : Record
    {
        public struct SNAMField
        {
            public int[] Values;

            public SNAMField(UnityBinaryReader r, int dataSize)
            {
                Values = new int[dataSize >> 2];
                for (var i = 0; i < Values.Length; i++)
                    Values[i] = r.ReadLEInt32();
            }
        }

        public struct CNAMField
        {
            public float LeafCurvature;
            public float MinimumLeafAngle;
            public float MaximumLeafAngle;
            public float BranchDimmingValue;
            public float LeafDimmingValue;
            public int ShadowRadius;
            public float RockSpeed;
            public float RustleSpeed;

            public CNAMField(UnityBinaryReader r, int dataSize)
            {
                LeafCurvature = r.ReadLESingle();
                MinimumLeafAngle = r.ReadLESingle();
                MaximumLeafAngle = r.ReadLESingle();
                BranchDimmingValue = r.ReadLESingle();
                LeafDimmingValue = r.ReadLESingle();
                ShadowRadius = r.ReadLEInt32();
                RockSpeed = r.ReadLESingle();
                RustleSpeed = r.ReadLESingle();
            }
        }

        public struct BNAMField
        {
            public float Width;
            public float Height;

            public BNAMField(UnityBinaryReader r, int dataSize)
            {
                Width = r.ReadLESingle();
                Height = r.ReadLESingle();
            }
        }

        public override string ToString() => $"TREE: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL; // Model
        public FILEField ICON; // Leaf Texture
        public SNAMField SNAM; // SpeedTree Seeds, array of ints
        public CNAMField CNAM; // Tree Parameters
        public BNAMField BNAM; // Billboard Dimensions

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                case "ICON": ICON = new FILEField(r, dataSize); return true;
                case "SNAM": SNAM = new SNAMField(r, dataSize); return true;
                case "CNAM": CNAM = new CNAMField(r, dataSize); return true;
                case "BNAM": BNAM = new BNAMField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}