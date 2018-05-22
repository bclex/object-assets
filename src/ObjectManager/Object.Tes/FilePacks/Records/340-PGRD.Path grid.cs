using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class PGRDRecord : Record
    {
        public override string ToString() => $"PGRD: {EDID.Value}";
        public STRVField EDID { get; set; }
        public UNKNField DATA;
        public UNKNField PGRP;
        public UNKNField PGRC;
        public UNKNField PGAG;
        public UNKNField PGRR;
        public UNKNField PGRL;
        public UNKNField PGRI;

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "DATA": DATA = new UNKNField(r, dataSize); return true;
                case "PGRP": PGRP = new UNKNField(r, dataSize); return true;
                case "PGRC": PGRC = new UNKNField(r, dataSize); return true;
                // TODO
                case "PGAG": PGAG = new UNKNField(r, dataSize); return true;
                case "PGRR": PGAG = new UNKNField(r, dataSize); return true;
                case "PGRL": PGRL = new UNKNField(r, dataSize); return true;
                case "PGRI": PGRI = new UNKNField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}