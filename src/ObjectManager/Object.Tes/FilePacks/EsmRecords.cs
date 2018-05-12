using OA.Core;
using OA.Tes.FilePacks.Records;
using System;

//http://en.uesp.net/wiki/Tes3Mod:File_Format
//http://en.uesp.net/wiki/Tes4Mod:Mod_File_Format
//http://en.uesp.net/wiki/Tes5Mod:Mod_File_Format
//https://github.com/TES5Edit/TES5Edit/blob/dev/wbDefinitionsTES5.pas 
namespace OA.Tes.FilePacks
{
    public static class RecordUtils
    {
        public static string GetModelFileName(IRecord record)
        {
            if (record is STATRecord) return ((STATRecord)record).MODL.Value;
            else if (record is DOORRecord) return ((DOORRecord)record).MODL.Value;
            else if (record is MISCRecord) return ((MISCRecord)record).MODL.Value;
            else if (record is WEAPRecord) return ((WEAPRecord)record).MODL.Value;
            else if (record is CONTRecord) return ((CONTRecord)record).MODL.Value;
            else if (record is LIGHRecord) return ((LIGHRecord)record).MODL.Value;
            else if (record is ARMORecord) return ((ARMORecord)record).MODL.Value;
            else if (record is CLOTRecord) return ((CLOTRecord)record).MODL.Value;
            else if (record is REPARecord) return ((REPARecord)record).MODL.Value;
            else if (record is ACTIRecord) return ((ACTIRecord)record).MODL.Value;
            else if (record is APPARecord) return ((APPARecord)record).MODL.Value;
            else if (record is LOCKRecord) return ((LOCKRecord)record).MODL.Value;
            else if (record is PROBRecord) return ((PROBRecord)record).MODL.Value;
            else if (record is INGRRecord) return ((INGRRecord)record).MODL.Value;
            else if (record is BOOKRecord) return ((BOOKRecord)record).MODL.Value;
            else if (record is ALCHRecord) return ((ALCHRecord)record).MODL.Value;
            else if (record is CREARecord) return ((CREARecord)record).MODL.Value;
            else if (record is NPC_Record npc) { return npc.MODL != null ? npc.MODL.Value : null; }
            else return null;
        }
    }

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

        public Header(UnityBinaryReader r, GameFormatId formatId)
        {
            Type = r.ReadASCIIString(4);
            if (Type == "GRUP")
            {
                if (formatId == GameFormatId.Tes4) DataSize = (uint)r.ReadLEUInt64() - 20;
                else if (formatId == GameFormatId.Tes5) DataSize = r.ReadLEUInt32() - 24;
                Label = r.ReadASCIIString(4);
                if (formatId == GameFormatId.Tes4) GroupType = (int)r.ReadLEInt64();
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

        public Record CreateRecord(long position)
        {
            Record r;
            switch (Type)
            {
                case "TES3": r = new TES3Record(); break;
                case "TES4": r = new TES4Record(); break;
                case "GMST": r = new GMSTRecord(); break;
                case "GLOB": r = new GLOBRecord(); break;
                case "SOUN": r = new SOUNRecord(); break;
                case "REGN": r = new REGNRecord(); break;
                case "LTEX": r = new LTEXRecord(); break;
                case "STAT": r = new STATRecord(); break;
                case "DOOR": r = new DOORRecord(); break;
                case "MISC": r = new MISCRecord(); break;
                case "WEAP": r = new WEAPRecord(); break;
                case "CONT": r = new CONTRecord(); break;
                case "LIGH": r = new LIGHRecord(); break;
                case "ARMO": r = new ARMORecord(); break;
                case "CLOT": r = new CLOTRecord(); break;
                case "REPA": r = new REPARecord(); break;
                case "ACTI": r = new ACTIRecord(); break;
                case "APPA": r = new APPARecord(); break;
                case "LOCK": r = new LOCKRecord(); break;
                case "PROB": r = new PROBRecord(); break;
                case "INGR": r = new INGRRecord(); break;
                case "BOOK": r = new BOOKRecord(); break;
                case "ALCH": r = new ALCHRecord(); break;
                case "CELL": r = new CELLRecord(); break;
                case "LAND": r = new LANDRecord(); break;
                case "CREA": r = TesSettings.Game.CreaturesEnabled ? new CREARecord() : null; break;
                case "NPC_": r = TesSettings.Game.NpcsEnabled ? new NPC_Record() : null; break;
                //
                case "CLAS":
                case "SPEL":
                case "BODY":
                case "PGRD":
                case "INFO":
                case "DIAL":
                case "SNDG":
                case "ENCH":
                case "SCPT":
                case "SKIL":
                case "RACE":
                case "MGEF":
                case "LEVI":
                case "LEVC":
                case "BSGN":
                case "FACT": r = null; break;
                default: Utils.Warning($"Unsupported ESM record type: {Type}"); r = null; break;
            }
            if (r != null)
            {
                r.Position = position;
                r.Header = this;
            }
            return r;
        }
    }


    public abstract class Record : IRecord
    {
        public long Position;
        public Header Header;

        /// <summary>
        /// Return an uninitialized subrecord to deserialize, or null to skip.
        /// </summary>
        /// <returns>Return an uninitialized subrecord to deserialize, or null to skip.</returns>
        public abstract Field CreateField(string type);

        /// <summary>
        /// Return an uninitialized subrecord to deserialize, or null to skip.
        /// </summary>
        /// <returns>Return an uninitialized subrecord to deserialize, or null to skip.</returns>
        public abstract Field CreateField(string type, GameFormatId formatId);

        public void Read(UnityBinaryReader r, string filePath, GameFormatId formatId)
        {
            var endPosition = r.BaseStream.Position + Header.DataSize;
            while (r.BaseStream.Position < endPosition)
            {
                var header = new FieldHeader(r, formatId);
                var field = formatId != GameFormatId.Tes3 ? CreateField(header.Type, formatId) : CreateField(header.Type);
                // skip the record if null
                if (field == null)
                {
                    r.BaseStream.Position += header.DataSize;
                    continue;
                }
                var position = r.BaseStream.Position;
                field.Read(r, header.DataSize);
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

    public abstract class Field
    {
        //public FieldHeader Header;

        public abstract void Read(UnityBinaryReader reader, uint dataSize);
    }
}
