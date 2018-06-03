//
//  SCPTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SCPTRecord: Record {
    // TESX
    public struct CTDAField
    {
        public enum INFOType : byte
        {
            Nothing = 0,
            Function = 1,
            Global = 2,
            Local = 3,
            Journal = 4,
            Item = 5,
            Dead = 6,
            NotID = 7,
            NotFaction = 8,
            NotClass = 9,
            NotRace = 10,
            NotCell = 11,
            NotLocal = 12,
        }

        public byte CompareOp; // TES3: 0 = [=], 1 = [!=], 2 = [>], 3 = [>=], 4 = [<], 5 = [<=]
                                // TES4: 0 = [=], 2 = [!=], 4 = [>], 6 = [>=], 8 = [<], 10 = [<=]
        public string FunctionId; // (00-71) - sX = Global/Local/Not Local types, JX = Journal type, IX = Item Type, DX = Dead Type, XX = Not ID Type, FX = Not Faction, CX = Not Class, RX = Not Race, LX = Not Cell
                                    // TES3
        public byte Index; // (0-5)
        public byte Type;
        public string Name; // Except for the function type, this is the ID for the global/local/etc. Is not nessecarily NULL terminated.The function type SCVR sub-record has
                            // TES4
        public float ComparisonValue;
        public int Parameter1; // Parameter #1
        public int Parameter2; // Parameter #2

        public CTDAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                Index = r.ReadByte();
                Type = r.ReadByte();
                FunctionId = r.ReadASCIIString(2);
                CompareOp = (byte)(r.ReadByte() * 2);
                Name = r.ReadASCIIString((int)dataSize - 5);
                ComparisonValue = Parameter1 = Parameter2 = 0;
                return;
            }
            CompareOp = r.ReadByte();
            r.skipBytes(3); // Unused
            ComparisonValue = r.ReadLESingle();
            FunctionId = r.ReadASCIIString(4);
            Parameter1 = r.ReadLEInt32();
            Parameter2 = r.ReadLEInt32();
            if (dataSize == 24)
                r.skipBytes(4); // Unused
            Index = Type = 0;
            Name = null;
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
            if (dataSize == 20)
                return;
            r.skipBytes(dataSize - 20);
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

    public var description: String { return "SCPT: \(EDID ?? SCHD)" }
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

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "SCHD": SCHD = SCHDField(r, dataSize)
        case "SCVR": if format != .TES3 { SLSDs.last!.SCVRField(r, dataSize) } else { SCHD.SCVRField(r, dataSize) }
        case "SCDA":
        case "SCDT": SCDA = BYTVField(r, dataSize)
        case "SCTX": SCTX = STRVField(r, dataSize)
        // TES4
        case "SCHR": SCHR = SCHRField(r, dataSize)
        case "SLSD": SLSDs.append(SLSDField(r, dataSize))
        case "SCRO": SCROs.append(FMIDField<Record>(r, dataSize))
        case "SCRV": let idx = r.readLEUInt32(); SCRVs.append(SLSDs.first(where: { $0.idx == idx }))
        default: return false
        }
        return true
    }
}
