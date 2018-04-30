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
            if (record is STATRecord) return ((STATRecord)record).MODL.value;
            else if (record is DOORRecord) return ((DOORRecord)record).MODL.value;
            else if (record is MISCRecord) return ((MISCRecord)record).MODL.value;
            else if (record is WEAPRecord) return ((WEAPRecord)record).MODL.value;
            else if (record is CONTRecord) return ((CONTRecord)record).MODL.value;
            else if (record is LIGHRecord) return ((LIGHRecord)record).MODL.value;
            else if (record is ARMORecord) return ((ARMORecord)record).MODL.value;
            else if (record is CLOTRecord) return ((CLOTRecord)record).MODL.value;
            else if (record is REPARecord) return ((REPARecord)record).MODL.value;
            else if (record is ACTIRecord) return ((ACTIRecord)record).MODL.value;
            else if (record is APPARecord) return ((APPARecord)record).MODL.value;
            else if (record is LOCKRecord) return ((LOCKRecord)record).MODL.value;
            else if (record is PROBRecord) return ((PROBRecord)record).MODL.value;
            else if (record is INGRRecord) return ((INGRRecord)record).MODL.value;
            else if (record is BOOKRecord) return ((BOOKRecord)record).MODL.value;
            else if (record is ALCHRecord) return ((ALCHRecord)record).MODL.value;
            else if (record is CREARecord) return ((CREARecord)record).MODL.value;
            else if (record is NPC_Record) { var npc = (NPC_Record)record; return npc.MODL != null ? npc.MODL.value : null; }
            else return null;
        }
    }

    public class Header
    {
        public string type; // 4 bytes
        public uint dataSize;
        public uint flags;
        public bool compressed => (flags & 0x00040000) != 0;
        public uint formId;

        public Header(UnityBinaryReader r, GameId gameId)
        {
            type = r.ReadASCIIString(4);
            dataSize = r.ReadLEUInt32();
            if (gameId == GameId.Morrowind)
                r.ReadLEUInt32();
            flags = r.ReadLEUInt32();
            if (gameId == GameId.Morrowind)
                return;
            formId = r.ReadLEUInt32();
            r.ReadLEUInt32();
            if (gameId == GameId.Oblivion)
                return;
            r.ReadLEUInt32();
        }

        public Record CreateUninitializedRecord()
        {
            var game = TesSettings.Game;
            Record r;
            switch (type)
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
                default: Utils.Warning("Unsupported ESM record type: " + type); r = null; break;
            }
            if (r != null)
                r.header = this;
            return r;
        }
    }

    public class SubRecordHeader
    {
        public string type; // 4 bytes
        public uint dataSize;

        public SubRecordHeader(UnityBinaryReader r, GameId gameId)
        {
            type = r.ReadASCIIString(4);
            if (gameId == GameId.Morrowind) dataSize = r.ReadLEUInt32();
            else dataSize = r.ReadLEUInt16();
        }
    }

    public abstract class Record : IRecord
    {
        public Header header;

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

        public void DeserializeData(UnityBinaryReader r, GameId gameId)
        {
            var dataEndPos = r.BaseStream.Position + header.dataSize;
            while (r.BaseStream.Position < dataEndPos)
            {
                var subRecordStartStreamPosition = r.BaseStream.Position;
                var subRecordHeader = new SubRecordHeader(r, gameId);
                var subRecord = gameId > GameId.Morrowind ? CreateUninitializedSubRecord(subRecordHeader.type, gameId) : CreateUninitializedSubRecord(subRecordHeader.type);
                // Read or skip the record.
                if (subRecord != null)
                {
                    subRecord.header = subRecordHeader;
                    var subRecordDataStreamPosition = r.BaseStream.Position;
                    subRecord.DeserializeData(r, subRecordHeader.dataSize);
                    if (r.BaseStream.Position != (subRecordDataStreamPosition + subRecord.header.dataSize))
                        throw new FormatException("Failed reading " + subRecord.header.type + " subrecord at offset " + subRecordStartStreamPosition);
                }
                else r.BaseStream.Position += subRecordHeader.dataSize;
            }
        }
    }

    public abstract class SubRecord
    {
        public SubRecordHeader header;

        public abstract void DeserializeData(UnityBinaryReader reader, uint dataSize);
    }

    // Common sub-records.
    public class STRVSubRecord : SubRecord
    {
        public string value;

        public override void DeserializeData(UnityBinaryReader r, uint dataSize)
        {
            value = r.ReadPossiblyNullTerminatedASCIIString((int)header.dataSize);
        }
    }

    // variable size
    public class INTVSubRecord : SubRecord
    {
        public long value;

        public override void DeserializeData(UnityBinaryReader r, uint dataSize)
        {
            switch (header.dataSize)
            {
                case 1: value = r.ReadByte(); break;
                case 2: value = r.ReadLEInt16(); break;
                case 4: value = r.ReadLEInt32(); break;
                case 8: value = r.ReadLEInt64(); break;
                default: throw new NotImplementedException("Tried to read an INTV subrecord with an unsupported size (" + header.dataSize.ToString() + ").");
            }
        }
    }
    public class INTVTwoI32SubRecord : SubRecord
    {
        public int value0, value1;

        public override void DeserializeData(UnityBinaryReader r, uint dataSize)
        {
            Debug.Assert(header.dataSize == 8);
            value0 = r.ReadLEInt32();
            value1 = r.ReadLEInt32();
        }
    }
    public class INDXSubRecord : INTVSubRecord { }

    public class FLTVSubRecord : SubRecord
    {
        public float value;

        public override void DeserializeData(UnityBinaryReader r, uint dataSize)
        {
            value = r.ReadLESingle();
        }
    }

    public class ByteSubRecord : SubRecord
    {
        public byte value;

        public override void DeserializeData(UnityBinaryReader r, uint dataSize)
        {
            value = r.ReadByte();
        }
    }
    public class Int32SubRecord : SubRecord
    {
        public int value;

        public override void DeserializeData(UnityBinaryReader r, uint dataSize)
        {
            value = r.ReadLEInt32();
        }
    }
    public class UInt32SubRecord : SubRecord
    {
        public uint value;

        public override void DeserializeData(UnityBinaryReader r, uint dataSize)
        {
            value = r.ReadLEUInt32();
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
