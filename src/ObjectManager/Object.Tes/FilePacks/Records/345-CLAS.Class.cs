using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class CLASRecord : Record
    {
        public struct DATAField
        {
            public DATAField(UnityBinaryReader r, uint dataSize)
            {
                r.ReadBytes((int)dataSize);
            }
        }

        public override string ToString() => $"CLAS: {EDID.Value}";
        public STRVField EDID { get; set; } // editorID*
        public STRVField FULL; // Name*
        public STRVField DESC; // Description*
        public STRVField? ICON; // Description
        public DATAField DATA; // Data*

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FNAM": FULL = new STRVField(r, dataSize); return true;
                case "CLDT": r.ReadBytes((int)dataSize); return true;
                case "DESC": DESC = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "FULL": FULL = new STRVField(r, dataSize); return true;
                case "DESC": DESC = new STRVField(r, dataSize); return true;
                case "ICON": ICON = new STRVField(r, dataSize); return true;
                case "DATA": DATA = new DATAField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}
