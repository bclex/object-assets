using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using OA.Configuration;
using OA.Core;
using OA.Tes.FilePacks.Records;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


// TES3
//http://en.uesp.net/wiki/Tes3Mod:File_Format
//https://github.com/TES5Edit/TES5Edit/blob/dev/wbDefinitionsTES3.pas
//http://en.uesp.net/morrow/tech/mw_esm.txt
//https://github.com/mlox/mlox/blob/master/util/tes3cmd/tes3cmd
// TES4
//https://github.com/WrinklyNinja/esplugin/tree/master/src
//http://en.uesp.net/wiki/Tes4Mod:Mod_File_Format
//https://github.com/TES5Edit/TES5Edit/blob/dev/wbDefinitionsTES4.pas 
// TES5
//http://en.uesp.net/wiki/Tes5Mod:Mod_File_Format
//https://github.com/TES5Edit/TES5Edit/blob/dev/wbDefinitionsTES5.pas 

namespace OA.Tes.FilePacks
{
    public enum GameFormatId
    {
        TES3 = 3,
        TES4,
        TES5,
    }

    public class Header
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

        public enum HeaderGroupType : int
        {
            Top = 0,                    // Label: Record type
            WorldChildren,              // Label: Parent (WRLD)
            InteriorCellBlock,          // Label: Block number
            InteriorCellSubBlock,       // Label: Sub-block number
            ExteriorCellBlock,          // Label: Grid Y, X (Note the reverse order)
            ExteriorCellSubBlock,       // Label: Grid Y, X (Note the reverse order)
            CellChildren,               // Label: Parent (CELL)
            TopicChildren,              // Label: Parent (DIAL)
            CellPersistentChilden,      // Label: Parent (CELL)
            CellTemporaryChildren,      // Label: Parent (CELL)
            CellVisibleDistantChildren, // Label: Parent (CELL)
        }

        public override string ToString() => $"{Type}:{GroupType}";
        public Header Parent;
        public string Type; // 4 bytes
        public uint DataSize;
        public HeaderFlags Flags;
        public bool Compressed => (Flags & HeaderFlags.Compressed) != 0;
        public uint FormId;
        public long Position;
        // group
        public byte[] Label;
        public HeaderGroupType GroupType;

        public Header() { }
        public Header(UnityBinaryReader r, GameFormatId format, Header parent)
        {
            Parent = parent;
            Type = r.ReadASCIIString(4);
            if (Type == "GRUP")
            {
                DataSize = (uint)(r.ReadLEUInt32() - (format == GameFormatId.TES4 ? 20 : 24));
                Label = r.ReadBytes(4);
                GroupType = (HeaderGroupType)r.ReadLEInt32();
                r.ReadLEUInt32(); // stamp | stamp + uknown
                if (format != GameFormatId.TES4)
                    r.ReadLEUInt32(); // version + uknown
                Position = r.BaseStream.Position;
                return;
            }
            DataSize = r.ReadLEUInt32();
            if (format == GameFormatId.TES3)
                r.ReadLEUInt32(); // Unknown
            Flags = (HeaderFlags)r.ReadLEUInt32();
            if (format == GameFormatId.TES3)
            {
                Position = r.BaseStream.Position;
                return;
            }
            // tes4
            FormId = r.ReadLEUInt32();
            r.ReadLEUInt32();
            if (format == GameFormatId.TES4)
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
            public Func<Record> F;
            public Func<int, bool> L;
        }

        static Dictionary<string, RecordType> CreateMap = new Dictionary<string, RecordType>
        {
            { "TES3", new RecordType { F = ()=>new TES3Record(), L = x => true }},
            { "TES4", new RecordType { F = ()=>new TES4Record(), L = x => true }},
            // 0
            { "LTEX", new RecordType { F = ()=>new LTEXRecord(), L = x => x > 0 }},
            { "STAT", new RecordType { F = ()=>new STATRecord(), L = x => x > 0 }},
            { "CELL", new RecordType { F = ()=>new CELLRecord(), L = x => x > 0 }},
            { "LAND", new RecordType { F = ()=>new LANDRecord(), L = x => x > 0 }},
            // 1
            { "DOOR", new RecordType { F = ()=>new DOORRecord(), L = x => x > 1 }},
            { "MISC", new RecordType { F = ()=>new MISCRecord(), L = x => x > 1 }},
            { "WEAP", new RecordType { F = ()=>new WEAPRecord(), L = x => x > 1 }},
            { "CONT", new RecordType { F = ()=>new CONTRecord(), L = x => x > 1 }},
            { "LIGH", new RecordType { F = ()=>new LIGHRecord(), L = x => x > 1 }},
            { "ARMO", new RecordType { F = ()=>new ARMORecord(), L = x => x > 1 }},
            { "CLOT", new RecordType { F = ()=>new CLOTRecord(), L = x => x > 1 }},
            { "REPA", new RecordType { F = ()=>new REPARecord(), L = x => x > 1 }},
            { "ACTI", new RecordType { F = ()=>new ACTIRecord(), L = x => x > 1 }},
            { "APPA", new RecordType { F = ()=>new APPARecord(), L = x => x > 1 }},
            { "LOCK", new RecordType { F = ()=>new LOCKRecord(), L = x => x > 1 }},
            { "PROB", new RecordType { F = ()=>new PROBRecord(), L = x => x > 1 }},
            { "INGR", new RecordType { F = ()=>new INGRRecord(), L = x => x > 1 }},
            { "BOOK", new RecordType { F = ()=>new BOOKRecord(), L = x => x > 1 }},
            { "ALCH", new RecordType { F = ()=>new ALCHRecord(), L = x => x > 1 }},
            { "CREA", new RecordType { F = ()=>new CREARecord(), L = x => x > 1 && BaseSettings.Game.CreaturesEnabled }},
            { "NPC_", new RecordType { F = ()=>new NPC_Record(), L = x => x > 1 && BaseSettings.Game.NpcsEnabled }},
            // 2
            { "GMST", new RecordType { F = ()=>new GMSTRecord(), L = x => x > 2 }},
            { "GLOB", new RecordType { F = ()=>new GLOBRecord(), L = x => x > 2 }},
            { "SOUN", new RecordType { F = ()=>new SOUNRecord(), L = x => x > 2 }},
            { "REGN", new RecordType { F = ()=>new REGNRecord(), L = x => x > 2 }},
            // 3
            { "CLAS", new RecordType { F = ()=>new CLASRecord(), L = x => x > 3 }},
            { "SPEL", new RecordType { F = ()=>new SPELRecord(), L = x => x > 3 }},
            { "BODY", new RecordType { F = ()=>new BODYRecord(), L = x => x > 3 }},
            { "PGRD", new RecordType { F = ()=>new PGRDRecord(), L = x => x > 3 }},
            { "INFO", new RecordType { F = ()=>new INFORecord(), L = x => x > 3 }},
            { "DIAL", new RecordType { F = ()=>new DIALRecord(), L = x => x > 3 }},
            { "SNDG", new RecordType { F = ()=>new SNDGRecord(), L = x => x > 3 }},
            { "ENCH", new RecordType { F = ()=>new ENCHRecord(), L = x => x > 3 }},
            { "SCPT", new RecordType { F = ()=>new SCPTRecord(), L = x => x > 3 }},
            { "SKIL", new RecordType { F = ()=>new SKILRecord(), L = x => x > 3 }},
            { "RACE", new RecordType { F = ()=>new RACERecord(), L = x => x > 3 }},
            { "MGEF", new RecordType { F = ()=>new MGEFRecord(), L = x => x > 3 }},
            { "LEVI", new RecordType { F = ()=>new LEVIRecord(), L = x => x > 3 }},
            { "LEVC", new RecordType { F = ()=>new LEVCRecord(), L = x => x > 3 }},
            { "BSGN", new RecordType { F = ()=>new BSGNRecord(), L = x => x > 3 }},
            { "FACT", new RecordType { F = ()=>new FACTRecord(), L = x => x > 3 }},
            { "SSCR", new RecordType { F = ()=>new SSCRRecord(), L = x => x > 3 }},
            // 4 - Oblivion
            { "WRLD", new RecordType { F = ()=>new WRLDRecord(), L = x => x > 0 }},
            { "ACRE", new RecordType { F = ()=>new ACRERecord(), L = x => x > 1 }},
            { "ACHR", new RecordType { F = ()=>new ACHRRecord(), L = x => x > 1 }},
            { "REFR", new RecordType { F = ()=>new REFRRecord(), L = x => x > 1 }},
            //
            { "AMMO", new RecordType { F = ()=>new AMMORecord(), L = x => x > 4 }},
            { "ANIO", new RecordType { F = ()=>new ANIORecord(), L = x => x > 4 }},
            { "CLMT", new RecordType { F = ()=>new CLMTRecord(), L = x => x > 4 }},
            { "CSTY", new RecordType { F = ()=>new CSTYRecord(), L = x => x > 4 }},
            { "EFSH", new RecordType { F = ()=>new EFSHRecord(), L = x => x > 4 }},
            { "EYES", new RecordType { F = ()=>new EYESRecord(), L = x => x > 4 }},
            { "FLOR", new RecordType { F = ()=>new FLORRecord(), L = x => x > 4 }},
            { "FURN", new RecordType { F = ()=>new FURNRecord(), L = x => x > 4 }},
            { "GRAS", new RecordType { F = ()=>new GRASRecord(), L = x => x > 4 }},
            { "HAIR", new RecordType { F = ()=>new HAIRRecord(), L = x => x > 4 }},
            { "IDLE", new RecordType { F = ()=>new IDLERecord(), L = x => x > 4 }},
            { "KEYM", new RecordType { F = ()=>new KEYMRecord(), L = x => x > 4 }},
            { "LSCR", new RecordType { F = ()=>new LSCRRecord(), L = x => x > 4 }},
            { "LVLC", new RecordType { F = ()=>new LVLCRecord(), L = x => x > 4 }},
            { "LVLI", new RecordType { F = ()=>new LVLIRecord(), L = x => x > 4 }},
            { "LVSP", new RecordType { F = ()=>new LVSPRecord(), L = x => x > 4 }},
            { "PACK", new RecordType { F = ()=>new PACKRecord(), L = x => x > 4 }},
            { "QUST", new RecordType { F = ()=>new QUSTRecord(), L = x => x > 4 }},
            { "ROAD", new RecordType { F = ()=>new ROADRecord(), L = x => x > 4 }},
            { "SBSP", new RecordType { F = ()=>new SBSPRecord(), L = x => x > 4 }},
            { "SGST", new RecordType { F = ()=>new SGSTRecord(), L = x => x > 4 }},
            { "SLGM", new RecordType { F = ()=>new SLGMRecord(), L = x => x > 4 }},
            { "TREE", new RecordType { F = ()=>new TREERecord(), L = x => x > 4 }},
            { "WATR", new RecordType { F = ()=>new WATRRecord(), L = x => x > 4 }},
            { "WTHR", new RecordType { F = ()=>new WTHRRecord(), L = x => x > 4 }},
            // 5 - Skyrim
            { "AACT", new RecordType { F = ()=>new AACTRecord(), L = x => x > 5 }},
            { "ADDN", new RecordType { F = ()=>new ADDNRecord(), L = x => x > 5 }},
            { "ARMA", new RecordType { F = ()=>new ARMARecord(), L = x => x > 5 }},
            { "ARTO", new RecordType { F = ()=>new ARTORecord(), L = x => x > 5 }},
            { "ASPC", new RecordType { F = ()=>new ASPCRecord(), L = x => x > 5 }},
            { "ASTP", new RecordType { F = ()=>new ASTPRecord(), L = x => x > 5 }},
            { "AVIF", new RecordType { F = ()=>new AVIFRecord(), L = x => x > 5 }},
            { "DLBR", new RecordType { F = ()=>new DLBRRecord(), L = x => x > 5 }},
            { "DLVW", new RecordType { F = ()=>new DLVWRecord(), L = x => x > 5 }},
            { "SNDR", new RecordType { F = ()=>new SNDRRecord(), L = x => x > 5 }},
        };

        public Record CreateRecord(long position, int recordLevel)
        {
            if (!CreateMap.TryGetValue(Type, out RecordType recordType))
            {
                Utils.Warning($"Unsupported ESM record type: {Type}");
                return null;
            }
            if (!recordType.L(recordLevel))
                return null;
            var record = recordType.F();
            record.Header = this;
            return record;
        }
    }

    public partial class RecordGroup
    {
        public byte[] Label => Headers.First.Value.Label;
        public override string ToString() => Headers.First.Value.ToString();
        public LinkedList<Header> Headers = new LinkedList<Header>();
        public List<Record> Records = new List<Record>();
        public List<RecordGroup> Groups;
        public Dictionary<byte[], RecordGroup[]> GroupsByLabel;
        readonly UnityBinaryReader _r;
        readonly string _filePath;
        readonly GameFormatId _formatId;
        readonly int _recordLevel;
        int _headerSkip;

        public RecordGroup(UnityBinaryReader r, string filePath, GameFormatId format, int recordLevel)
        {
            _r = r;
            _filePath = filePath;
            _formatId = format;
            _recordLevel = recordLevel;
        }

        public void AddHeader(Header header)
        {
            //Console.WriteLine($"Read: {header.Label}");
            Headers.AddLast(header);
            if (header.Label != null && header.GroupType == Header.HeaderGroupType.Top)
                switch (Encoding.ASCII.GetString(header.Label))
                {
                    case "CELL": case "WRLD": Load(); break; // "DIAL"
                }
        }

        public List<Record> Load(bool loadAll = false)
        {
            if (_headerSkip == Headers.Count) return Records;
            lock (_r)
            {
                if (_headerSkip == Headers.Count) return Records;
                foreach (var header in Headers.Skip(_headerSkip))
                    ReadGroup(header, loadAll);
                _headerSkip = Headers.Count;
                return Records;
            }
        }

        static int _cellsLoaded = 0;
        void ReadGroup(Header header, bool loadAll)
        {
            _r.BaseStream.Position = header.Position;
            var endPosition = header.Position + header.DataSize;
            while (_r.BaseStream.Position < endPosition)
            {
                var recordHeader = new Header(_r, _formatId, header);
                if (recordHeader.Type == "GRUP")
                {
                    var group = ReadGRUP(header, recordHeader);
                    if (loadAll)
                        group.Load(loadAll);
                    continue;
                }
                // HACK to limit cells loading
                if (recordHeader.Type == "CELL" && _cellsLoaded > int.MaxValue)
                {
                    _r.BaseStream.Position += recordHeader.DataSize;
                    continue;
                }
                var record = recordHeader.CreateRecord(_r.BaseStream.Position, _recordLevel);
                if (record == null)
                {
                    _r.BaseStream.Position += recordHeader.DataSize;
                    continue;
                }
                ReadRecord(record, recordHeader.Compressed);
                Records.Add(record);
                if (recordHeader.Type == "CELL") { _cellsLoaded++; }
            }
            GroupsByLabel = Groups?.GroupBy(x => x.Label, ByteArrayComparer.Default).ToDictionary(x => x.Key, x => x.ToArray(), ByteArrayComparer.Default);
        }

        RecordGroup ReadGRUP(Header header, Header recordHeader)
        {
            var nextPosition = _r.BaseStream.Position + recordHeader.DataSize;
            if (Groups == null)
                Groups = new List<RecordGroup>();
            var group = new RecordGroup(_r, _filePath, _formatId, _recordLevel);
            group.AddHeader(recordHeader);
            Groups.Add(group);
            _r.BaseStream.Position = nextPosition;
            // print header path
            var headerPath = string.Join("/", GetHeaderPath(new List<string>(), header).ToArray());
            Console.WriteLine($"Grup: {headerPath} {header.GroupType}");
            return group;
        }

        static List<string> GetHeaderPath(List<string> b, Header header)
        {
            if (header.Parent != null) GetHeaderPath(b, header.Parent);
            b.Add(header.GroupType != Header.HeaderGroupType.Top ? BitConverter.ToString(header.Label).Replace("-", string.Empty) : Encoding.ASCII.GetString(header.Label));
            return b;
        }

        void ReadRecord(Record record, bool compressed)
        {
            //Console.WriteLine($"Recd: {record.Header.Type}");
            if (!compressed)
            {
                record.Read(_r, _filePath, _formatId);
                return;
            }
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
    }

    public abstract class Record : IRecord
    {
        internal Header Header;
        public uint Id => Header.FormId;

        /// <summary>
        /// Return an uninitialized subrecord to deserialize, or null to skip.
        /// </summary>
        /// <returns>Return an uninitialized subrecord to deserialize, or null to skip.</returns>
        public abstract bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize);

        public void Read(UnityBinaryReader r, string filePath, GameFormatId format)
        {
            var startPosition = r.BaseStream.Position;
            var endPosition = startPosition + Header.DataSize;
            while (r.BaseStream.Position < endPosition)
            {
                var fieldHeader = new FieldHeader(r, format);
                if (fieldHeader.Type == "XXXX")
                {
                    if (fieldHeader.DataSize != 4)
                        throw new InvalidOperationException();
                    fieldHeader.DataSize = (int)r.ReadLEUInt32();
                    continue;
                }
                else if (fieldHeader.Type == "OFST" && Header.Type == "WRLD")
                {
                    r.BaseStream.Position = endPosition;
                    continue;
                }
                var position = r.BaseStream.Position;
                if (!CreateField(r, format, fieldHeader.Type, fieldHeader.DataSize))
                {
                    Utils.Warning($"Unsupported ESM record type: {Header.Type}:{fieldHeader.Type}");
                    r.BaseStream.Position += fieldHeader.DataSize;
                    continue;
                }
                // check full read
                if (r.BaseStream.Position != position + fieldHeader.DataSize)
                    throw new FormatException($"Failed reading {Header.Type}:{fieldHeader.Type} field data at offset {position} in {filePath} of {r.BaseStream.Position - position - fieldHeader.DataSize}");
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
        public int DataSize;

        public FieldHeader(UnityBinaryReader r, GameFormatId format)
        {
            Type = r.ReadASCIIString(4);
            DataSize = (int)(format == GameFormatId.TES3 ? r.ReadLEUInt32() : r.ReadLEUInt16());
        }
    }
}
