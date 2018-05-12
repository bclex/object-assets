using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class CONTRecord : Record
    {
        public class NPCOField : Field
        {
            public uint ItemCount;
            public string ItemName;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                ItemCount = r.ReadLEUInt32();
                ItemName = r.ReadPossiblyNullTerminatedASCIIString(32);
            }
        }

        public STRVField NAME;
        public STRVField MODL;
        public STRVField FNAM; // container name
        public FLTVField CNDT; // weight
        public UInt32Field FLAG; // flags
        public List<NPCOField> NPCOs = new List<NPCOField>();

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "CNDT": CNDT = new FLTVField(); return CNDT;
                case "FLAG": FLAG = new UInt32Field(); return FLAG;
                case "NPCO": var NPCO = new NPCOField(); NPCOs.Add(NPCO); return NPCO;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}