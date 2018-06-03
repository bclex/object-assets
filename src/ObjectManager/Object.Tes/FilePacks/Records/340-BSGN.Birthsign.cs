using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class BSGNRecord : Record
    {
        public override string ToString() => $"BSGN: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public STRVField FULL; // Sign name
        public FILEField ICON; // Texture
        public STRVField DESC; // Description
        public List<STRVField> NPCSs = new List<STRVField>(); // TES3: Spell/ability
        public List<FMIDField<Record>> SPLOs = new List<FMIDField<Record>>(); // TES4: (points to a SPEL or LVSP record)

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FULL":
                case "FNAM": FULL = new STRVField(r, dataSize); return true;
                case "ICON":
                case "TNAM": ICON = new FILEField(r, dataSize); return true;
                case "DESC": DESC = new STRVField(r, dataSize); return true;
                case "SPLO": if (SPLOs == null) SPLOs = new List<FMIDField<Record>>(); SPLOs.Add(new FMIDField<Record>(r, dataSize)); return true;
                case "NPCS": if (NPCSs == null) NPCSs = new List<STRVField>(); NPCSs.Add(new STRVField(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}