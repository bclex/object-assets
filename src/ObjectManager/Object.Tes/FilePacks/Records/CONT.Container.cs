using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class CONTRecord : Record
    {
        public class CNDTSubRecord : FLTVSubRecord { }
        public class FLAGSubRecord : UInt32SubRecord { }
        public class NPCOSubRecord : SubRecord
        {
            public uint ItemCount;
            public string ItemName;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                ItemCount = r.ReadLEUInt32();
                ItemName = r.ReadPossiblyNullTerminatedASCIIString(32);
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM; // container name
        public CNDTSubRecord CNDT; // weight
        public FLAGSubRecord FLAG; // flags
        public List<NPCOSubRecord> NPCOs = new List<NPCOSubRecord>();

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "CNDT": CNDT = new CNDTSubRecord(); return CNDT;
                case "FLAG": FLAG = new FLAGSubRecord(); return FLAG;
                case "NPCO": var NPCO = new NPCOSubRecord(); NPCOs.Add(NPCO); return NPCO;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}