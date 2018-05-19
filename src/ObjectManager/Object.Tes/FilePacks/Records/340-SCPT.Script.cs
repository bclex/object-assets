using OA.Core;
using System;
using System.Linq;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class SCPTRecord : Record
    {
        // TES3
        public class SCHDField
        {
            public override string ToString() => $"{Name}";
            public string Name;
            public int NumShorts;
            public int NumLongs;
            public int NumFloats;
            public int ScriptDataSize;
            public int LocalVarSize;
            public string[] Variables;

            public SCHDField(UnityBinaryReader r, uint dataSize)
            {
                Name = r.ReadASCIIString(32, ASCIIFormat.ZeroPadded);
                NumShorts = r.ReadLEInt32();
                NumLongs = r.ReadLEInt32();
                NumFloats = r.ReadLEInt32();
                ScriptDataSize = r.ReadLEInt32();
                LocalVarSize = r.ReadLEInt32();
                // SCVRField
                Variables = null;
            }

            public void SCVRField(UnityBinaryReader r, uint dataSize)
            {
                Variables = r.ReadASCIIMultiString((int)dataSize);
            }
        }

        //public struct SCVRField
        //{
        //    public override string ToString() => $"{string.Join(",", Values)}";
        //    public string[] Values;

        //    public SCVRField(UnityBinaryReader r, uint dataSize)
        //    {
        //        Values = r.ReadASCIIMultiString((int)dataSize);
        //    }
        //}

        // TES4
        public struct SCHRField
        {
            public override string ToString() => $"{RefCount}";
            public uint RefCount;
            public uint CompiledSize;
            public uint VariableCount;
            public uint Type; // 0x000 = Object, 0x001 = Quest, 0x100 = Magic Effect

            public SCHRField(UnityBinaryReader r, uint dataSize)
            {
                r.ReadLEInt32(); // Unused
                RefCount = r.ReadLEUInt32();
                CompiledSize = r.ReadLEUInt32();
                VariableCount = r.ReadLEUInt32();
                Type = r.ReadLEUInt32();
            }
        }

        public class SLSDField
        {
            public override string ToString() => $"{Idx}:{VariableName}";
            public uint Idx;
            public uint Type;
            public string VariableName;

            public SLSDField(UnityBinaryReader r, uint dataSize)
            {
                Idx = r.ReadLEUInt32();
                r.ReadLEUInt32(); // Unknown
                r.ReadLEUInt32(); // Unknown
                r.ReadLEUInt32(); // Unknown
                Type = r.ReadLEUInt32();
                r.ReadLEUInt32(); // Unknown
                // SCVRField
                VariableName = null;
            }

            public void SCVRField(UnityBinaryReader r, uint dataSize)
            {
                VariableName = r.ReadASCIIString((int)dataSize, ASCIIFormat.PossiblyNullTerminated);
            }
        }

        public override string ToString() => $"SCPT: {EDID.Value ?? SCHD.Name}";
        public STRVField EDID { get; set; } // Editor ID
        public BYTVField SCDA; // Compiled Script
        public STRVField SCTX; // Script Source
        // TES3
        public SCHDField SCHD; // Script Data
        // TES4
        public SCHRField SCHR; // Script Data
        public List<SLSDField> SLSDs = new List<SLSDField>(); // Variable data
        public List<SLSDField> SCRVs = new List<SLSDField>(); // Ref variable data (one for each ref declared)
        public List<FMIDField<Record>> SCROs = new List<FMIDField<Record>>(); // Global variable reference

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "SCHD": SCHD = new SCHDField(r, dataSize); return true;
                case "SCVR": if (formatId != GameFormatId.Tes3) ArrayUtils.Last(SLSDs).SCVRField(r, dataSize); else SCHD.SCVRField(r, dataSize); return true;
                case "SCDA":
                case "SCDT": SCDA = new BYTVField(r, dataSize); return true;
                case "SCTX": SCTX = new STRVField(r, dataSize); return true;
                // TES4
                case "SCHR": SCHR = new SCHRField(r, dataSize); return true;
                case "SLSD": SLSDs.Add(new SLSDField(r, dataSize)); return true;
                case "SCRO": SCROs.Add(new FMIDField<Record>(r, dataSize)); return true;
                case "SCRV": var idx = r.ReadLEUInt32(); SCRVs.Add(SLSDs.Single(x => x.Idx == idx)); return true;
                default: return false;
            }
        }
    }
}