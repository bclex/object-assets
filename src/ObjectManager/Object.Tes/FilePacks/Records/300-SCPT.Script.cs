using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class SCPTRecord : Record
    {
        public struct SCHDField
        {
            public string Name;
            public int NumShorts;
            public int NumLongs;
            public int NumFloats;
            public int ScriptDataSize;
            public int LocalVarSize;

            public SCHDField(UnityBinaryReader r, uint dataSize)
            {
                Name = r.ReadASCIIString(32, ASCIIFormat.ZeroPadded);
                NumShorts = r.ReadLEInt32();
                NumLongs = r.ReadLEInt32();
                NumFloats = r.ReadLEInt32();
                ScriptDataSize = r.ReadLEInt32();
                LocalVarSize = r.ReadLEInt32();
            }
        }

        public struct SCVRField
        {
            public string[] Variables;

            public SCVRField(UnityBinaryReader r, uint dataSize)
            {
                Variables = r.ReadASCIIMultiString((int)dataSize);
            }
        }

        public struct SCDTField
        {
            public byte[] Data;

            public SCDTField(UnityBinaryReader r, uint dataSize)
            {
                Data = r.ReadBytes((int)dataSize);
            }
        }

        public override string ToString() => $"SCPT: {SCHD.Name}";
        public SCHDField SCHD { get; set; }
        public SCVRField SCVR { get; set; }
        public SCDTField SCDT { get; set; }
        public STRVField SCTX { get; set; }

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "SCHD": SCHD = new SCHDField(r, dataSize); return true;
                case "SCVR": SCVR = new SCVRField(r, dataSize); return true;
                case "SCDT": SCDT = new SCDTField(r, dataSize); return true;
                case "SCTX": SCTX = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}