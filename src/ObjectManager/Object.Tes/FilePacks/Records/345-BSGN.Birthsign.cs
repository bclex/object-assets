using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class BSGNRecord : Record
    {
        public override string ToString() => $"BSGN: {EDID.Value}";
        public STRVField EDID { get; set; } // Sign ID
        public STRVField FNAM; // Sign name
        public FILEField TNAM; // Texture
        public STRVField DESC; // Description
        public List<STRVField> NPCSs = new List<STRVField>(); // Spell/ability

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "TNAM": TNAM = new FILEField(r, dataSize); return true;
                case "DESC": DESC = new STRVField(r, dataSize); return true;
                case "NPCS": NPCSs.Add(new STRVField(r, dataSize)); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}