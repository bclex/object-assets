﻿using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class LEVCRecord : Record
    {
        public override string ToString() => $"LEVC: {EDID.Value}";
        public STRVField EDID { get; set; } // ID
        public IN32Field DATA; // List data - 1 = Calc from all levels <= PC level
        public BYTEField NNAM; // Chance None?
        public IN32Field INDX; // Number of items in list
        public List<STRVField> CNAMs = new List<STRVField>(); // ID string of list item
        public List<IN16Field> INTVs = new List<IN16Field>(); // PC level for previous CNAM
        // The CNAM/INTV can occur many times in pairs

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "DATA": DATA = new IN32Field(r, dataSize); return true;
                case "NNAM": NNAM = new BYTEField(r, dataSize); return true;
                case "INDX": INDX = new IN32Field(r, dataSize); return true;
                case "CNAM": CNAMs.Add(new STRVField(r, dataSize)); return true;
                case "INTV": INTVs.Add(new IN16Field(r, dataSize)); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}