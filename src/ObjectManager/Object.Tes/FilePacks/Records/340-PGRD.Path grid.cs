using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class PGRDRecord : Record
    {
        public override string ToString() => $"PGRD: {EDID.Value}";
        public STRVField EDID { get; set; }
        public UNKNField DATA;
        public UNKNField PGRP;
        public UNKNField PGRC;

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "DATA": DATA = new UNKNField(r, dataSize); return true;
                case "PGRP": PGRP = new UNKNField(r, dataSize); return true;
                case "PGRC": PGRC = new UNKNField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}