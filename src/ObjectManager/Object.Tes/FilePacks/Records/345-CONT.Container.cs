using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class CONTRecord : Record, IHaveEDID, IHaveMODL
    {
        public override string ToString() => $"CONT: {EDID.Value}";
        public STRVField EDID { get; set; } // ID
        public FILEField MODL { get; set; } // NIF Model
        public STRVField FNAM; // Container name
        public FLTVField CNDT; // weight
        public UI32Field FLAG; // flags 0x0001 = Organic, 0x0002 = Respawns, organic only, 0x0008 = Default, unknown
        public List<NPCOField> NPCOs = new List<NPCOField>();
        public STRVField? SCRI;

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "MODL": MODL = new FILEField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "CNDT": CNDT = new FLTVField(r, dataSize); return true;
                    case "FLAG": FLAG = new UI32Field(r, dataSize); return true;
                    case "NPCO": NPCOs.Add(new NPCOField(r, dataSize)); return true;
                    case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}