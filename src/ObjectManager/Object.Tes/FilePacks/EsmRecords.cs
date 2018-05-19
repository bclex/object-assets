using OA.Configuration;
using OA.Core;
using OA.Tes.FilePacks.Records;
using System;
using System.Collections.Generic;

//https://github.com/WrinklyNinja/esplugin/tree/master/src
//http://en.uesp.net/wiki/Tes3Mod:File_Format
//http://en.uesp.net/wiki/Tes4Mod:Mod_File_Format
//http://en.uesp.net/wiki/Tes5Mod:Mod_File_Format
//https://github.com/TES5Edit/TES5Edit/blob/dev/wbDefinitionsTES5.pas 
//http://en.uesp.net/morrow/tech/mw_esm.txt
namespace OA.Tes.FilePacks
{
    [Flags]
    public enum HeaderFlags : uint
    {
        EsmFile = 0x00000001,               // ESM file. (TES4.HEDR record only.)
        Deleted = 0x00000020,               // Deleted
        R00 = 0x00000040,                   // Constant / (REFR) Hidden From Local Map (Needs Confirmation: Related to shields)
        R01 = 0x00000100,                   // Must Update Anims / (REFR) Inaccessible
        R02 = 0x00000200,                   // (REFR) Hidden from local map / (ACHR) Starts dead / (REFR) MotionBlurCastsShadows
        R03 = 0x00000400,                   // Quest item / Persistent reference / (LSCR) Displays in Main Menu
        InitiallyDisabled = 0x00000800,     // Initially disabled
        Ignored = 0x00001000,               // Ignored
        VisibleWhenDistant = 0x00008000,    // Visible when distant
        R04 = 0x00010000,                   // (ACTI) Random Animation Start
        R05 = 0x00020000,                   // (ACTI) Dangerous / Off limits (Interior cell) Dangerous Can't be set withough Ignore Object Interaction
        Compressed = 0x00040000,            // Data is compressed
        CantWait = 0x00080000,              // Can't wait
        // tes5
        R06 = 0x00100000,                   // (ACTI) Ignore Object Interaction Ignore Object Interaction Sets Dangerous Automatically
        IsMarker = 0x00800000,              // Is Marker
        R07 = 0x02000000,                   // (ACTI) Obstacle / (REFR) No AI Acquire
        NavMesh01 = 0x04000000,             // NavMesh Gen - Filter
        NavMesh02 = 0x08000000,             // NavMesh Gen - Bounding Box
        R08 = 0x10000000,                   // (FURN) Must Exit to Talk / (REFR) Reflected By Auto Water
        R09 = 0x20000000,                   // (FURN/IDLM) Child Can Use / (REFR) Don't Havok Settle
        R10 = 0x40000000,                   // NavMesh Gen - Ground / (REFR) NoRespawn
        R11 = 0x80000000,                   // (REFR) MultiBound
    }

    public class Header
    {
        public string Type; // 4 bytes
        public uint DataSize;
        public HeaderFlags Flags;
        public bool Compressed => (Flags & HeaderFlags.Compressed) != 0;
        public uint FormId;
        // group
        public string Label;
        public int GroupType;

        public Header() { }
        public Header(UnityBinaryReader r, GameFormatId formatId)
        {
            Type = r.ReadASCIIString(4);
            if (Type == "GRUP")
            {
                if (formatId == GameFormatId.Tes4) DataSize = r.ReadLEUInt32() - 20;
                else if (formatId == GameFormatId.Tes5) DataSize = r.ReadLEUInt32() - 24;
                Label = r.ReadASCIIString(4);
                if (formatId == GameFormatId.Tes4) GroupType = r.ReadLEInt32();
                else if (formatId == GameFormatId.Tes5) GroupType = r.ReadLEInt32();
                r.ReadLEUInt32(); // stamp | stamp + uknown
                if (formatId == GameFormatId.Tes4)
                    return;
                r.ReadLEUInt32(); // version + uknown
                return;
            }
            DataSize = r.ReadLEUInt32();
            if (formatId == GameFormatId.Tes3)
                r.ReadLEUInt32(); // Unknown
            Flags = (HeaderFlags)r.ReadLEUInt32();
            if (formatId == GameFormatId.Tes3)
                return;
            // tes4
            FormId = r.ReadLEUInt32();
            r.ReadLEUInt32();
            if (formatId == GameFormatId.Tes4)
                return;
            // tes5
            r.ReadLEUInt32();
        }

        struct RecordType
        {
            public Func<byte, Record> Func;
            public bool Load;
        }

        static Dictionary<string, RecordType> Create = new Dictionary<string, RecordType>
        {
            { "TES3", new RecordType { Func = x => new TES3Record() }},
            { "TES4", new RecordType { Func = x => new TES4Record() }},
            // 0
            { "LTEX", new RecordType { Func = x => x > 0 ? new LTEXRecord() : null, Load = true }},
            { "STAT", new RecordType { Func = x => x > 0 ? new STATRecord() : null }},
            { "CELL", new RecordType { Func = x => x > 0 ? new CELLRecord() : null }},
            { "LAND", new RecordType { Func = x => x > 0 ? new LANDRecord() : null }},
            // 1
            { "DOOR", new RecordType { Func = x => x > 1 ? new DOORRecord() : null }},
            { "MISC", new RecordType { Func = x => x > 1 ? new MISCRecord() : null }},
            { "WEAP", new RecordType { Func = x => x > 1 ? new WEAPRecord() : null }},
            { "CONT", new RecordType { Func = x => x > 1 ? new CONTRecord() : null }},
            { "LIGH", new RecordType { Func = x => x > 1 ? new LIGHRecord() : null }},
            { "ARMO", new RecordType { Func = x => x > 1 ? new ARMORecord() : null }},
            { "CLOT", new RecordType { Func = x => x > 1 ? new CLOTRecord() : null }},
            { "REPA", new RecordType { Func = x => x > 1 ? new REPARecord() : null }},
            { "ACTI", new RecordType { Func = x => x > 1 ? new ACTIRecord() : null }},
            { "APPA", new RecordType { Func = x => x > 1 ? new APPARecord() : null }},
            { "LOCK", new RecordType { Func = x => x > 1 ? new LOCKRecord() : null }},
            { "PROB", new RecordType { Func = x => x > 1 ? new PROBRecord() : null }},
            { "INGR", new RecordType { Func = x => x > 1 ? new INGRRecord() : null }},
            { "BOOK", new RecordType { Func = x => x > 1 ? new BOOKRecord() : null }},
            { "ALCH", new RecordType { Func = x => x > 1 ? new ALCHRecord() : null }},
            { "CREA", new RecordType { Func = x => x > 1 && BaseSettings.Game.CreaturesEnabled ? new CREARecord() : null } },
            { "NPC_", new RecordType { Func = x => x > 1 && BaseSettings.Game.NpcsEnabled ? new NPC_Record() : null } },
            // 2
            { "GMST", new RecordType { Func = x => x > 2 ? new GMSTRecord() : null } },
            { "GLOB", new RecordType { Func = x => x > 2 ? new GLOBRecord() : null }},
            { "SOUN", new RecordType { Func = x => x > 2 ? new SOUNRecord() : null }},
            { "REGN", new RecordType { Func = x => x > 2 ? new REGNRecord() : null }},
            // 3
            { "CLAS", new RecordType { Func = x => x > 3 ? new CLASRecord() : null }},
            { "SPEL", new RecordType { Func = x => x > 3 ? new SPELRecord() : null }},
            { "BODY", new RecordType { Func = x => x > 3 ? new BODYRecord() : null }},
            { "PGRD", new RecordType { Func = x => x > 3 ? new PGRDRecord() : null }},
            { "INFO", new RecordType { Func = x => x > 3 ? new INFORecord() : null }},
            { "DIAL", new RecordType { Func = x => x > 3 ? new DIALRecord() : null }},
            { "SNDG", new RecordType { Func = x => x > 3 ? new SNDGRecord() : null }},
            { "ENCH", new RecordType { Func = x => x > 3 ? new ENCHRecord() : null }},
            { "SCPT", new RecordType { Func = x => x > 3 ? new SCPTRecord() : null }},
            { "SKIL", new RecordType { Func = x => x > 3 ? new SKILRecord() : null }},
            { "RACE", new RecordType { Func = x => x > 3 ? new RACERecord() : null }},
            { "MGEF", new RecordType { Func = x => x > 3 ? new MGEFRecord() : null }},
            { "LEVI", new RecordType { Func = x => x > 3 ? new LEVIRecord() : null }},
            { "LEVC", new RecordType { Func = x => x > 3 ? new LEVCRecord() : null }},
            { "BSGN", new RecordType { Func = x => x > 3 ? new BSGNRecord() : null }},
            { "FACT", new RecordType { Func = x => x > 3 ? new FACTRecord() : null }},
            // 4 - Oblivion
            { "ACRE", new RecordType { Func = x => x > 4 ? new ACRERecord() : null }}, //*
            { "ACHR", new RecordType { Func = x => x > 4 ? new ACHRRecord() : null }},
            { "AMMO", new RecordType { Func = x => x > 4 ? new AMMORecord() : null }},
            { "ANIO", new RecordType { Func = x => x > 4 ? new ANIORecord() : null }},
            { "CLMT", new RecordType { Func = x => x > 4 ? new CLMTRecord() : null }},
            { "CSTY", new RecordType { Func = x => x > 4 ? new CSTYRecord() : null }},
            { "EFSH", new RecordType { Func = x => x > 4 ? new EFSHRecord() : null }},
            { "EYES", new RecordType { Func = x => x > 4 ? new EYESRecord() : null }},
            { "FLOR", new RecordType { Func = x => x > 4 ? new FLORRecord() : null }},
            { "FURN", new RecordType { Func = x => x > 4 ? new FURNRecord() : null }},
            { "GRAS", new RecordType { Func = x => x > 4 ? new GRASRecord() : null }},
            { "HAIR", new RecordType { Func = x => x > 4 ? new HAIRRecord() : null }}, //*
            { "IDLE", new RecordType { Func = x => x > 4 ? new IDLERecord() : null }},
            { "KEYM", new RecordType { Func = x => x > 4 ? new KEYMRecord() : null }}, //*
            { "LSCR", new RecordType { Func = x => x > 4 ? new LSCRRecord() : null }}, //?
            { "LVLC", new RecordType { Func = x => x > 4 ? new LVLCRecord() : null }},
            { "LVLI", new RecordType { Func = x => x > 4 ? new LVLIRecord() : null }},
            { "LVSP", new RecordType { Func = x => x > 4 ? new LVSPRecord() : null }},
            { "PACK", new RecordType { Func = x => x > 4 ? new PACKRecord() : null }},
            { "QUST", new RecordType { Func = x => x > 4 ? new QUSTRecord() : null }},
            { "REFR", new RecordType { Func = x => x > 4 ? new REFRRecord() : null }},
            { "ROAD", new RecordType { Func = x => x > 4 ? new ROADRecord() : null }}, //*
            { "SBSP", new RecordType { Func = x => x > 4 ? new SBSPRecord() : null }}, //*
            { "SGST", new RecordType { Func = x => x > 4 ? new SGSTRecord() : null }}, //*
            { "SLGM", new RecordType { Func = x => x > 4 ? new SLGMRecord() : null }},
            { "TREE", new RecordType { Func = x => x > 4 ? new TREERecord() : null }},
            { "WATR", new RecordType { Func = x => x > 4 ? new WATRRecord() : null }},
            { "WRLD", new RecordType { Func = x => x > 4 ? new WRLDRecord() : null }},
            { "WTHR", new RecordType { Func = x => x > 4 ? new WTHRRecord() : null }},
            // 5 - Skyrim
            { "AACT", new RecordType { Func = x => x > 5 ? new AACTRecord() : null }},
            { "ADDN", new RecordType { Func = x => x > 5 ? new ADDNRecord() : null }},
            { "ARMA", new RecordType { Func = x => x > 5 ? new ARMARecord() : null }},
            { "ARTO", new RecordType { Func = x => x > 5 ? new ARTORecord() : null }},
            { "ASPC", new RecordType { Func = x => x > 5 ? new ASPCRecord() : null }},
            { "ASTP", new RecordType { Func = x => x > 5 ? new ASTPRecord() : null }},
            { "AVIF", new RecordType { Func = x => x > 5 ? new AVIFRecord() : null }},
            { "DLBR", new RecordType { Func = x => x > 5 ? new DLBRRecord() : null }},
            { "DLVW", new RecordType { Func = x => x > 5 ? new DLVWRecord() : null }},
            { "SNDR", new RecordType { Func = x => x > 5 ? new SNDRRecord() : null }},
        };

        public Record CreateRecord(long position, byte level)
        {
            if (Create.TryGetValue(Type, out RecordType recordType))
            {
                var r = recordType.Func(level);
                if (r != null)
                {
                    r.Position = position;
                    r.Header = this;
                }
                return r;
            }
            Utils.Warning($"Unsupported ESM record type: {Type}");
            return null;
        }
    }

    public abstract class Record : IRecord
    {
        internal long Position;
        internal Header Header;

        /// <summary>
        /// Return an uninitialized subrecord to deserialize, or null to skip.
        /// </summary>
        /// <returns>Return an uninitialized subrecord to deserialize, or null to skip.</returns>
        public abstract bool CreateField(UnityBinaryReader r, string type, uint dataSize);

        /// <summary>
        /// Return an uninitialized subrecord to deserialize, or null to skip.
        /// </summary>
        /// <returns>Return an uninitialized subrecord to deserialize, or null to skip.</returns>
        public abstract bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize);

        public void Read(UnityBinaryReader r, string filePath, GameFormatId formatId)
        {
            var endPosition = r.BaseStream.Position + Header.DataSize;
            while (r.BaseStream.Position < endPosition)
            {
                var header = new FieldHeader(r, formatId);
                var position = r.BaseStream.Position;
                var skipField = formatId != GameFormatId.Tes3 ? !CreateField(r, formatId, header.Type, header.DataSize) : !CreateField(r, header.Type, header.DataSize);
                if (skipField)
                {
                    Utils.Warning($"Unsupported ESM record type: {Header.Type} {header.Type}");
                    r.BaseStream.Position += header.DataSize;
                    continue;
                }
                // check full read
                if (r.BaseStream.Position != position + header.DataSize)
                    throw new FormatException($"Failed reading {header.Type} field data at offset {position} in {filePath}");
            }
            // check full read
            if (r.BaseStream.Position != Position + Header.DataSize)
                throw new FormatException($"Failed reading {Header.Type} record data at offset {Position} in {filePath}");
        }
    }

    public class FieldHeader
    {
        public string Type; // 4 bytes
        public uint DataSize;

        public FieldHeader(UnityBinaryReader r, GameFormatId formatId)
        {
            Type = r.ReadASCIIString(4);
            if (formatId == GameFormatId.Tes3) DataSize = r.ReadLEUInt32();
            else DataSize = r.ReadLEUInt16();
        }
    }
}
