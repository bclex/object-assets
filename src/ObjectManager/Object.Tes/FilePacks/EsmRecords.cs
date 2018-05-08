using OA.Tes.FilePacks.Records;
using OA.Core;
using System;
using UnityEngine;

//http://en.uesp.net/wiki/Tes4Mod:Mod_File_Format
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
            else if (record is NPC_Record) { var npc = (NPC_Record)record; return npc.MODL != null ? npc.MODL.Value : null; }
            else return null;
        }
    }

    public class Header
    {
        public string Type; // 4 bytes
        public uint DataSize;
        public uint Flags;
        public bool Compressed => (Flags & 0x00040000) != 0;
        public uint FormId;

        public Header(UnityBinaryReader r, GameId gameId)
        {
            Type = r.ReadASCIIString(4);
            DataSize = r.ReadLEUInt32();
            if (gameId == GameId.Morrowind)
                r.ReadLEUInt32();
            Flags = r.ReadLEUInt32();
            if (gameId == GameId.Morrowind)
                return;
            FormId = r.ReadLEUInt32();
            r.ReadLEUInt32();
            if (gameId == GameId.Oblivion)
                return;
            r.ReadLEUInt32();
        }

        public Record CreateUninitializedRecord(long position)
        {
            var game = TesSettings.Game;
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
                case "CREA": r = game.CreaturesEnabled ? new CREARecord() : null; break;
                case "NPC_": r = game.NpcsEnabled ? new NPC_Record() : null; break;
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

    public class SubRecordHeader
    {
        public string Type; // 4 bytes
        public uint DataSize;

        public SubRecordHeader(UnityBinaryReader r, GameId gameId)
        {
            Type = r.ReadASCIIString(4);
            if (gameId == GameId.Morrowind) DataSize = r.ReadLEUInt32();
            else DataSize = r.ReadLEUInt16();
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
        public abstract SubRecord CreateUninitializedSubRecord(string subRecordName);

        /// <summary>
        /// Return an uninitialized subrecord to deserialize, or null to skip.
        /// </summary>
        /// <returns>Return an uninitialized subrecord to deserialize, or null to skip.</returns>
        public abstract SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId);

        public void Read(UnityBinaryReader r, GameId gameId)
        {
            var dataEndPos = r.BaseStream.Position + Header.DataSize;
            while (r.BaseStream.Position < dataEndPos)
            {
                var subRecordStartStreamPosition = r.BaseStream.Position;
                var subRecordHeader = new SubRecordHeader(r, gameId);
                var subRecord = gameId > GameId.Morrowind ? CreateUninitializedSubRecord(subRecordHeader.Type, gameId) : CreateUninitializedSubRecord(subRecordHeader.Type);
                // Read or skip the record.
                if (subRecord != null)
                {
                    subRecord.Header = subRecordHeader;
                    var subRecordDataStreamPosition = r.BaseStream.Position;
                    subRecord.Read(r, subRecordHeader.DataSize);
                    if (r.BaseStream.Position != subRecordDataStreamPosition + subRecord.Header.DataSize)
                        throw new FormatException($"Failed reading {subRecord.Header.Type} subrecord at offset {subRecordStartStreamPosition}");
                }
                else r.BaseStream.Position += subRecordHeader.DataSize;
            }
        }
    }

    public abstract class SubRecord
    {
        public SubRecordHeader Header;

        public abstract void Read(UnityBinaryReader reader, uint dataSize);
    }

    // Common sub-records.
    public class STRVSubRecord : SubRecord
    {
        public string Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadPossiblyNullTerminatedASCIIString((int)Header.DataSize);
        }
    }

    // variable size
    public class INTVSubRecord : SubRecord
    {
        public long Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            switch (Header.DataSize)
            {
                case 1: Value = r.ReadByte(); break;
                case 2: Value = r.ReadLEInt16(); break;
                case 4: Value = r.ReadLEInt32(); break;
                case 8: Value = r.ReadLEInt64(); break;
                default: throw new NotImplementedException($"Tried to read an INTV subrecord with an unsupported size ({Header.DataSize})");
            }
        }
    }
    public class INTVTwoI32SubRecord : SubRecord
    {
        public int Value0, Value1;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            Debug.Assert(Header.DataSize == 8);
            Value0 = r.ReadLEInt32();
            Value1 = r.ReadLEInt32();
        }
    }
    public class INDXSubRecord : INTVSubRecord { }

    public class FLTVSubRecord : SubRecord
    {
        public float Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLESingle();
        }
    }

    public class ByteSubRecord : SubRecord
    {
        public byte Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadByte();
        }
    }
    public class Int32SubRecord : SubRecord
    {
        public int Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEInt32();
        }
    }
    public class UInt32SubRecord : SubRecord
    {
        public uint Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEUInt32();
        }
    }

    public class NAMESubRecord : STRVSubRecord { }
    public class FNAMSubRecord : STRVSubRecord { }
    public class SNAMSubRecord : STRVSubRecord { }
    public class ANAMSubRecord : STRVSubRecord { }
    public class ITEXSubRecord : STRVSubRecord { }
    public class ENAMSubRecord : STRVSubRecord { }
    public class BNAMSubRecord : STRVSubRecord { }
    public class CNAMSubRecord : STRVSubRecord { }
    public class SCRISubRecord : STRVSubRecord { }
    public class SCPTSubRecord : STRVSubRecord { }
    public class MODLSubRecord : STRVSubRecord { }
    public class TEXTSubRecord : STRVSubRecord { }

    public class INDXBNAMCNAMGroup
    {
        public INDXSubRecord INDX;
        public BNAMSubRecord BNAM;
        public CNAMSubRecord CNAM;
    }
}
