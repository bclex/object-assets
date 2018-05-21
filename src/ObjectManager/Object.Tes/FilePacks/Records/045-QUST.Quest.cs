﻿using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class QUSTRecord : Record
    {
        public override string ToString() => $"QUST: {EDID.Value}";
        public STRVField EDID;
        // TODO

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