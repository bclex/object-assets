using OA.Core;
using System.Collections.Generic;
using System.Linq;

namespace OA.Tes.FilePacks.Records
{
    public class SCPTRecord : Record
    {
        // TESX
        public struct CTDAField
        {
            public enum INFOType : byte
            {
                Nothing = 0, Function, Global, Local, Journal, Item, Dead, NotId, NotFaction, NotClass, NotRace, NotCell, NotLocal
            }

            // TES3: 0 = [=], 1 = [!=], 2 = [>], 3 = [>=], 4 = [<], 5 = [<=]
            // TES4: 0 = [=], 2 = [!=], 4 = [>], 6 = [>=], 8 = [<], 10 = [<=]
            public byte CompareOp;
            // (00-71) - sX = Global/Local/Not Local types, JX = Journal type, IX = Item Type, DX = Dead Type, XX = Not ID Type, FX = Not Faction, CX = Not Class, RX = Not Race, LX = Not Cell
            public string FunctionId;
            // TES3
            public byte Index; // (0-5)
            public byte Type;
            // Except for the function type, this is the ID for the global/local/etc. Is not nessecarily NULL terminated.The function type SCVR sub-record has
            public string Name;
            // TES4
            public float ComparisonValue;
            public int Parameter1; // Parameter #1
            public int Parameter2; // Parameter #2

            public CTDAField(UnityBinaryReader r, int dataSize, GameFormatId format)
            {
                if (format == GameFormatId.TES3)
                {
                    Index = r.ReadByte();
                    Type = r.ReadByte();
                    FunctionId = r.ReadASCIIString(2);
                    CompareOp = (byte)(r.ReadByte() << 1);
                    Name = r.ReadASCIIString(dataSize - 5);
                    ComparisonValue = Parameter1 = Parameter2 = 0;
                    return;
                }
                CompareOp = r.ReadByte();
                r.SkipBytes(3); // Unused
                ComparisonValue = r.ReadLESingle();
                FunctionId = r.ReadASCIIString(4);
                Parameter1 = r.ReadLEInt32();
                Parameter2 = r.ReadLEInt32();
                if (dataSize != 24)
                {
                    Index = Type = 0;
                    Name = null;
                    return;
                }
                r.SkipBytes(4); // Unused
            }
        }

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

            public SCHDField(UnityBinaryReader r, int dataSize)
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

            public void SCVRField(UnityBinaryReader r, int dataSize)
            {
                Variables = r.ReadASCIIMultiString(dataSize);
            }
        }

        // TES4
        public struct SCHRField
        {
            public override string ToString() => $"{RefCount}";
            public uint RefCount;
            public uint CompiledSize;
            public uint VariableCount;
            public uint Type; // 0x000 = Object, 0x001 = Quest, 0x100 = Magic Effect

            public SCHRField(UnityBinaryReader r, int dataSize)
            {
                r.SkipBytes(4); // Unused
                RefCount = r.ReadLEUInt32();
                CompiledSize = r.ReadLEUInt32();
                VariableCount = r.ReadLEUInt32();
                Type = r.ReadLEUInt32();
                if (dataSize == 20)
                    return;
                r.SkipBytes(dataSize - 20);
            }
        }

        public class SLSDField
        {
            public override string ToString() => $"{Idx}:{VariableName}";
            public uint Idx;
            public uint Type;
            public string VariableName;

            public SLSDField(UnityBinaryReader r, int dataSize)
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

            public void SCVRField(UnityBinaryReader r, int dataSize)
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

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "SCHD": SCHD = new SCHDField(r, dataSize); return true;
                case "SCVR": if (format != GameFormatId.TES3) ArrayUtils.Last(SLSDs).SCVRField(r, dataSize); else SCHD.SCVRField(r, dataSize); return true;
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