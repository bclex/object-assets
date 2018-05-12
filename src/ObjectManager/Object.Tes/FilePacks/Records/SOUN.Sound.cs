using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class SOUNRecord : Record
    {
        public class DATAField : Field
        {
            public byte Volume;
            public byte MinRange;
            public byte MaxRange;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Volume = r.ReadByte();
                MinRange = r.ReadByte();
                MaxRange = r.ReadByte();
            }
        }

        public STRVField NAME;
        public STRVField FNAM;
        public DATAField DATA;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "DATA": DATA = new DATAField(); return DATA;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}