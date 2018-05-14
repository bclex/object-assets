using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class SKILRecord : Record
    {
        public struct SKDTField
        {
            public int Attribute;
            public int Specialization; // 0 = Combat, 1 = Magic, 2 = Stealth
            public float[] UseValue; // The use types for each skill are hard-coded.

            public SKDTField(UnityBinaryReader r, uint dataSize)
            {
                Attribute = r.ReadLEInt32();
                Specialization = r.ReadLEInt32();
                UseValue = new float[4];
                for (var i = 0; i < UseValue.Length; i++)
                    UseValue[i] = r.ReadLESingle();
            }
        }

        public override string ToString() => $"SKIL: {INDX.Value}";
        public INTVField INDX; // Skill ID
        public SKDTField SKDT; // Skill Data
        public STRVField DESC; // Skill description

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "INDX": INDX = new INTVField(r, dataSize); return true;
                case "SKDT": SKDT = new SKDTField(r, dataSize); return true;
                case "DESC": DESC = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}