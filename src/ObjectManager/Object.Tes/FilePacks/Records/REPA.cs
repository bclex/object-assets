﻿using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class REPARecord : Record
    {
        public class RIDTSubRecord : SubRecord
        {
            public float Weight;
            public int Value;
            public int Uses;
            public float Quality;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Uses = r.ReadLEInt32();
                Quality = r.ReadLESingle();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public RIDTSubRecord RIDT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "RIDT": RIDT = new RIDTSubRecord(); return RIDT;
                case "ITEX": ITEX = new ITEXSubRecord(); return ITEX;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}