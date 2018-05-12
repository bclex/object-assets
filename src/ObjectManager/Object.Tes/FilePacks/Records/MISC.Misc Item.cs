using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class MISCRecord : Record
    {
        public class MCDTField : Field
        {
            public float Weight;
            public uint Value;
            public uint Unknown;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEUInt32();
                Unknown = r.ReadLEUInt32();
            }
        }

        public STRVField NAME; // door ID
        public STRVField MODL; // model filename
        public STRVField FNAM; // item name
        public MCDTField MCDT; // misc data
        public STRVField ITEX; // inventory icon filename
        public STRVField ENAM; // enchantment ID string
        public STRVField SCRI; // script ID string

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "MCDT": MCDT = new MCDTField(); return MCDT;
                case "ITEX": ITEX = new STRVField(); return ITEX;
                case "ENAM": ENAM = new STRVField(); return ENAM;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}