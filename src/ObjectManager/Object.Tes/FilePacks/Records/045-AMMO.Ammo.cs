using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class AMMORecord : Record
    {
        public override string ToString() => $"AMMO: {EDID.Value}";
        public STRVField EDID; // editorID
        // TODO

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize) => throw new NotImplementedException();
        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}