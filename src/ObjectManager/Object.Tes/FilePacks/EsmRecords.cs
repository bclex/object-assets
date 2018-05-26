using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using OA.Configuration;
using OA.Core;
using OA.Tes.FilePacks.Records;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public override string ToString() => Type;
        public string Type; // 4 bytes
        public uint DataSize;
        public HeaderFlags Flags;
        public bool Compressed => (Flags & HeaderFlags.Compressed) != 0;
        public uint FormId;
        public long Position;
        // group
        public string Label;
        public int GroupType;

        public Header() { }
        public Header(UnityBinaryReader r, GameFormatId formatId)
        {
            Type = r.ReadASCIIString(4);
            if (Type == "GRUP")
            {
                if (formatId == GameFormatId.TES4) DataSize = r.ReadLEUInt32() - 20;
                else if (formatId == GameFormatId.TES5) DataSize = r.ReadLEUInt32() - 24;
                Label = r.ReadASCIIString(4);
                if (formatId == GameFormatId.TES4) GroupType = r.ReadLEInt32();
                else if (formatId == GameFormatId.TES5) GroupType = r.ReadLEInt32();
                r.ReadLEUInt32(); // stamp | stamp + uknown
                if (formatId == GameFormatId.TES4)
                {
                    Position = r.BaseStream.Position;
                    return;
                }
                r.ReadLEUInt32(); // version + uknown
                Position = r.BaseStream.Position;
                return;
            }
            DataSize = r.ReadLEUInt32();
            if (formatId == GameFormatId.TES3)
                r.ReadLEUInt32(); // Unknown
            Flags = (HeaderFlags)r.ReadLEUInt32();
            if (formatId == GameFormatId.TES3)
            {
                Position = r.BaseStream.Position;
                return;
            }
            // tes4
            FormId = r.ReadLEUInt32();
            r.ReadLEUInt32();
            if (formatId == GameFormatId.TES4)
            {
                Position = r.BaseStream.Position;
                return;
            }
            // tes5
            r.ReadLEUInt32();
            Position = r.BaseStream.Position;
        }

        struct RecordType
        {
            public Func<Record> Func;
            public Func<byte, bool> Load;
        }

        static Dictionary<string, RecordType> Create = new Dictionary<string, RecordType>
        {
            { "TES3", new RecordType { Func = ()=>new TES3Record() }},
            { "TES4", new RecordType { Func = ()=>new TES4Record() }},
            // 0
            { "LTEX", new RecordType { Func = ()=>new LTEXRecord(), Load = x => x > 0 }},
            { "STAT", new RecordType { Func = ()=>new STATRecord(), Load = x => x > 0 }},
            { "CELL", new RecordType { Func = ()=>new CELLRecord(), Load = x => x > 0 }},
            { "LAND", new RecordType { Func = ()=>new LANDRecord(), Load = x => x > 0 }},
            // 1
            { "DOOR", new RecordType { Func = ()=>new DOORRecord(), Load = x => x > 1 }},
            { "MISC", new RecordType { Func = ()=>new MISCRecord(), Load = x => x > 1 }},
            { "WEAP", new RecordType { Func = ()=>new WEAPRecord(), Load = x => x > 1 }},
            { "CONT", new RecordType { Func = ()=>new CONTRecord(), Load = x => x > 1 }},
            { "LIGH", new RecordType { Func = ()=>new LIGHRecord(), Load = x => x > 1 }},
            { "ARMO", new RecordType { Func = ()=>new ARMORecord(), Load = x => x > 1 }},
            { "CLOT", new RecordType { Func = ()=>new CLOTRecord(), Load = x => x > 1 }},
            { "REPA", new RecordType { Func = ()=>new REPARecord(), Load = x => x > 1 }},
            { "ACTI", new RecordType { Func = ()=>new ACTIRecord(), Load = x => x > 1 }},
            { "APPA", new RecordType { Func = ()=>new APPARecord(), Load = x => x > 1 }},
            { "LOCK", new RecordType { Func = ()=>new LOCKRecord(), Load = x => x > 1 }},
            { "PROB", new RecordType { Func = ()=>new PROBRecord(), Load = x => x > 1 }},
            { "INGR", new RecordType { Func = ()=>new INGRRecord(), Load = x => x > 1 }},
            { "BOOK", new RecordType { Func = ()=>new BOOKRecord(), Load = x => x > 1 }},
            { "ALCH", new RecordType { Func = ()=>new ALCHRecord(), Load = x => x > 1 }},
            { "CREA", new RecordType { Func = ()=>new CREARecord(), Load = x => x > 1 && BaseSettings.Game.CreaturesEnabled }},
            { "NPC_", new RecordType { Func = ()=>new NPC_Record(), Load = x => x > 1 && BaseSettings.Game.NpcsEnabled }},
            // 2
            { "GMST", new RecordType { Func = ()=>new GMSTRecord(), Load = x => x > 2 }},
            { "GLOB", new RecordType { Func = ()=>new GLOBRecord(), Load = x => x > 2 }},
            { "SOUN", new RecordType { Func = ()=>new SOUNRecord(), Load = x => x > 2 }},
            { "REGN", new RecordType { Func = ()=>new REGNRecord(), Load = x => x > 2 }},
            // 3
            { "CLAS", new RecordType { Func = ()=>new CLASRecord(), Load = x => x > 3 }},
            { "SPEL", new RecordType { Func = ()=>new SPELRecord(), Load = x => x > 3 }},
            { "BODY", new RecordType { Func = ()=>new BODYRecord(), Load = x => x > 3 }},
            { "PGRD", new RecordType { Func = ()=>new PGRDRecord(), Load = x => x > 3 }},
            { "INFO", new RecordType { Func = ()=>new INFORecord(), Load = x => x > 3 }},
            { "DIAL", new RecordType { Func = ()=>new DIALRecord(), Load = x => x > 3 }},
            { "SNDG", new RecordType { Func = ()=>new SNDGRecord(), Load = x => x > 3 }},
            { "ENCH", new RecordType { Func = ()=>new ENCHRecord(), Load = x => x > 3 }},
            { "SCPT", new RecordType { Func = ()=>new SCPTRecord(), Load = x => x > 3 }},
            { "SKIL", new RecordType { Func = ()=>new SKILRecord(), Load = x => x > 3 }},
            { "RACE", new RecordType { Func = ()=>new RACERecord(), Load = x => x > 3 }},
            { "MGEF", new RecordType { Func = ()=>new MGEFRecord(), Load = x => x > 3 }},
            { "LEVI", new RecordType { Func = ()=>new LEVIRecord(), Load = x => x > 3 }},
            { "LEVC", new RecordType { Func = ()=>new LEVCRecord(), Load = x => x > 3 }},
            { "BSGN", new RecordType { Func = ()=>new BSGNRecord(), Load = x => x > 3 }},
            { "FACT", new RecordType { Func = ()=>new FACTRecord(), Load = x => x > 3 }},
            { "SSCR", new RecordType { Func = ()=>new SSCRRecord(), Load = x => x > 3 }},
            // 4 - Oblivion
            { "ACRE", new RecordType { Func = ()=>new ACRERecord(), Load = x => x > 4 }},
            { "ACHR", new RecordType { Func = ()=>new ACHRRecord(), Load = x => x > 4 }},
            { "AMMO", new RecordType { Func = ()=>new AMMORecord(), Load = x => x > 4 }},
            { "ANIO", new RecordType { Func = ()=>new ANIORecord(), Load = x => x > 4 }},
            { "CLMT", new RecordType { Func = ()=>new CLMTRecord(), Load = x => x > 4 }},
            { "CSTY", new RecordType { Func = ()=>new CSTYRecord(), Load = x => x > 4 }},
            { "EFSH", new RecordType { Func = ()=>new EFSHRecord(), Load = x => x > 4 }},
            { "EYES", new RecordType { Func = ()=>new EYESRecord(), Load = x => x > 4 }},
            { "FLOR", new RecordType { Func = ()=>new FLORRecord(), Load = x => x > 4 }},
            { "FURN", new RecordType { Func = ()=>new FURNRecord(), Load = x => x > 4 }},
            { "GRAS", new RecordType { Func = ()=>new GRASRecord(), Load = x => x > 4 }},
            { "HAIR", new RecordType { Func = ()=>new HAIRRecord(), Load = x => x > 4 }},
            { "IDLE", new RecordType { Func = ()=>new IDLERecord(), Load = x => x > 4 }},
            { "KEYM", new RecordType { Func = ()=>new KEYMRecord(), Load = x => x > 4 }},
            { "LSCR", new RecordType { Func = ()=>new LSCRRecord(), Load = x => x > 4 }},
            { "LVLC", new RecordType { Func = ()=>new LVLCRecord(), Load = x => x > 4 }},
            { "LVLI", new RecordType { Func = ()=>new LVLIRecord(), Load = x => x > 4 }},
            { "LVSP", new RecordType { Func = ()=>new LVSPRecord(), Load = x => x > 4 }},
            { "PACK", new RecordType { Func = ()=>new PACKRecord(), Load = x => x > 4 }},
            { "QUST", new RecordType { Func = ()=>new QUSTRecord(), Load = x => x > 4 }},
            { "REFR", new RecordType { Func = ()=>new REFRRecord(), Load = x => x > 4 }},
            { "ROAD", new RecordType { Func = ()=>new ROADRecord(), Load = x => x > 4 }},
            { "SBSP", new RecordType { Func = ()=>new SBSPRecord(), Load = x => x > 4 }},
            { "SGST", new RecordType { Func = ()=>new SGSTRecord(), Load = x => x > 4 }},
            { "SLGM", new RecordType { Func = ()=>new SLGMRecord(), Load = x => x > 4 }},
            { "TREE", new RecordType { Func = ()=>new TREERecord(), Load = x => x > 4 }},
            { "WATR", new RecordType { Func = ()=>new WATRRecord(), Load = x => x > 4 }},
            { "WRLD", new RecordType { Func = ()=>new WRLDRecord(), Load = x => x > 4 }},
            { "WTHR", new RecordType { Func = ()=>new WTHRRecord(), Load = x => x > 4 }},
            // 5 - Skyrim
            { "AACT", new RecordType { Func = ()=>new AACTRecord(), Load = x => x > 5 }},
            { "ADDN", new RecordType { Func = ()=>new ADDNRecord(), Load = x => x > 5 }},
            { "ARMA", new RecordType { Func = ()=>new ARMARecord(), Load = x => x > 5 }},
            { "ARTO", new RecordType { Func = ()=>new ARTORecord(), Load = x => x > 5 }},
            { "ASPC", new RecordType { Func = ()=>new ASPCRecord(), Load = x => x > 5 }},
            { "ASTP", new RecordType { Func = ()=>new ASTPRecord(), Load = x => x > 5 }},
            { "AVIF", new RecordType { Func = ()=>new AVIFRecord(), Load = x => x > 5 }},
            { "DLBR", new RecordType { Func = ()=>new DLBRRecord(), Load = x => x > 5 }},
            { "DLVW", new RecordType { Func = ()=>new DLVWRecord(), Load = x => x > 5 }},
            { "SNDR", new RecordType { Func = ()=>new SNDRRecord(), Load = x => x > 5 }},
        };

        public static bool LoadRecord(string type, byte level) => Create.TryGetValue(type, out RecordType recordType) ? recordType.Load(level) : false;

        public Record CreateRecord(long position)
        {
            if (Create.TryGetValue(Type, out RecordType recordType))
            {
                var r = recordType.Func();
                if (r != null)
                    r.Header = this;
                return r;
            }
            Utils.Warning($"Unsupported ESM record type: {Type}");
            return null;
        }
    }

    public class RecordGroup
    {
        public string Label => Headers.First.Value.Label;
        public override string ToString() => Headers.First.Value.Label;
        public LinkedList<Header> Headers = new LinkedList<Header>();
        public List<Record> Records = new List<Record>();
        public List<RecordGroup> Groups;
        readonly UnityBinaryReader _r;
        readonly string _filePath;
        readonly GameFormatId _formatId;
        readonly byte _level;
        int _headerSkip;

        public RecordGroup(UnityBinaryReader r, string filePath, GameFormatId formatId, byte level)
        {
            _r = r;
            _filePath = filePath;
            _formatId = formatId;
            _level = level;
        }

        public void AddHeader(Header header)
        {
            Headers.AddLast(header);
        }

        public void Read()
        {
            if (_headerSkip == Headers.Count) return;
            lock (_r)
            {
                if (_headerSkip == Headers.Count) return;
                foreach (var header in Headers.Skip(_headerSkip))
                    ReadGroup(header);
                _headerSkip = Headers.Count;
            }
        }

        void ReadGroup(Header groupHeader, bool readGroup = true)
        {
            _r.BaseStream.Position = groupHeader.Position;
            var endPosition = groupHeader.Position + groupHeader.DataSize;
            while (_r.BaseStream.Position < endPosition)
            {
                var header = new Header(_r, _formatId);
                if (header.Type == "GRUP")
                {
                    if (Groups == null)
                        Groups = new List<RecordGroup>();
                    var group = new RecordGroup(_r, _filePath, _formatId, _level);
                    group.AddHeader(header);
                    Groups.Add(group);
                    Console.WriteLine($"Grup: {groupHeader.Label}/{group}");
                    if (readGroup) group.Read();
                    else _r.BaseStream.Position += header.DataSize;
                    continue;
                }
                var record = header.CreateRecord(_r.BaseStream.Position);
                if (record == null)
                {
                    _r.BaseStream.Position += header.DataSize;
                    continue;
                }
                ReadRecord(record, header.Compressed);
                Records.Add(record);
            }
        }

        void ReadRecord(Record record, bool compressed)
        {
            if (compressed)
            {
                // decompress record
                var newDataSize = _r.ReadLEUInt32();
                var data = _r.ReadBytes((int)record.Header.DataSize - 4);
                var newData = new byte[newDataSize];
                using (var s = new MemoryStream(data))
                using (var gs = new InflaterInputStream(s))
                    gs.Read(newData, 0, newData.Length);
                // read record
                record.Header.Position = 0;
                record.Header.DataSize = newDataSize;
                using (var s = new MemoryStream(newData))
                using (var r = new UnityBinaryReader(s))
                    record.Read(r, _filePath, _formatId);
            }
            else record.Read(_r, _filePath, _formatId);
        }
    }

    public abstract class Record : IRecord
    {
        internal Header Header;

        /// <summary>
        /// Return an uninitialized subrecord to deserialize, or null to skip.
        /// </summary>
        /// <returns>Return an uninitialized subrecord to deserialize, or null to skip.</returns>
        public abstract bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize);

        public void Read(UnityBinaryReader r, string filePath, GameFormatId formatId)
        {
            var startPosition = r.BaseStream.Position;
            var endPosition = r.BaseStream.Position + Header.DataSize;
            while (r.BaseStream.Position < endPosition)
            {
                var header = new FieldHeader(r, formatId);
                if (header.Type == "XXXX")
                {
                    if (header.DataSize != 4)
                        throw new InvalidOperationException();
                    header.DataSize = (uint)r.ReadLEInt32();
                    continue;
                }
                else if (Header.Type == "WRLD" && header.Type == "OFST")
                {
                    r.BaseStream.Position = endPosition;
                    continue;
                }
                var position = r.BaseStream.Position;
                if (!CreateField(r, formatId, header.Type, header.DataSize))
                {
                    Utils.Warning($"Unsupported ESM record type: {Header.Type}:{header.Type}");
                    r.BaseStream.Position += header.DataSize;
                    continue;
                }
                // check full read
                if (r.BaseStream.Position != position + header.DataSize)
                    throw new FormatException($"Failed reading {Header.Type}:{header.Type} field data at offset {position} in {filePath} of {r.BaseStream.Position - position - header.DataSize}");
            }
            // check full read
            if (r.BaseStream.Position != endPosition)
                throw new FormatException($"Failed reading {Header.Type} record data at offset {startPosition} in {filePath}");
        }
    }

    public class FieldHeader
    {
        public override string ToString() => Type;
        public string Type; // 4 bytes
        public uint DataSize;

        public FieldHeader(UnityBinaryReader r, GameFormatId formatId)
        {
            Type = r.ReadASCIIString(4);
            DataSize = formatId == GameFormatId.TES3 ? r.ReadLEUInt32() : r.ReadLEUInt16();
        }
    }
}
