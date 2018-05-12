using OA.Tes.FilePacks.Records;
using OA.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// https://github.com/WrinklyNinja/esplugin/tree/master/src
// http://en.uesp.net/wiki/Tes4Mod:Mod_File_Format/Vs_Morrowind
namespace OA.Tes.FilePacks
{
    public partial class EsmFile : IDisposable
    {
        const int recordHeaderSizeInBytes = 16;
        public Record[] records;
        public Dictionary<Type, List<IRecord>> recordsByType;
        public Dictionary<string, IRecord> objectsByIDString;
        public Dictionary<Vector2i, CELLRecord> exteriorCELLRecordsByIndices;
        public Dictionary<Vector2i, LANDRecord> LANDRecordsByIndices;

        public EsmFile(string filePath, GameId gameId)
        {
            Read(filePath, GetFormatId());
            PostProcessRecords();
            GameFormatId GetFormatId()
            {
                switch (gameId)
                {
                    // tes
                    case GameId.Morrowind: return GameFormatId.Tes3;
                    case GameId.Oblivion: return GameFormatId.Tes4;
                    case GameId.Skyrim:
                    case GameId.SkyrimSE:
                    case GameId.SkyrimVR: return GameFormatId.Tes5;
                    // fallout
                    case GameId.Fallout3:
                    case GameId.FalloutNV: return GameFormatId.Tes4;
                    case GameId.Fallout4:
                    case GameId.Fallout4VR: return GameFormatId.Tes5;
                    default: throw new InvalidOperationException();
                }
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        ~EsmFile()
        {
            Close();
        }

        public void Close() { }

        public List<IRecord> GetRecordsOfType<T>() where T : Record { return recordsByType.TryGetValue(typeof(T), out List<IRecord> records) ? records : null; }

        void Read(string filePath, GameFormatId formatId)
        {
            var r = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            var header = new Header(r, formatId);
            if ((formatId == GameFormatId.Tes3 && header.Type != "TES3") || (formatId != GameFormatId.Tes3 && header.Type != "TES4"))
                throw new FormatException($"{filePath} record header {header.Type} is not valid for this {formatId}");
            var rootRecord = header.CreateRecord(r.BaseStream.Position);
            rootRecord.Read(r, filePath, formatId);
            // read stream
            var recordList = new List<Record>();
            var endPosition = r.BaseStream.Length;
            while (r.BaseStream.Position < endPosition)
            {
                header = new Header(r, formatId);
                if (header.Type == "GRUP")
                {
                    var groupRecords = ReadGroup(r, header, filePath, formatId);
                }
                else
                {
                    var record = header.CreateRecord(r.BaseStream.Position);
                    // skip the record if null
                    if (record == null)
                    {
                        r.BaseStream.Position += header.DataSize;
                        continue;
                    }
                    record.Read(r, filePath, formatId);
                    //recordList.Add(record);
                }
            }
            records = recordList.ToArray();
        }

        Record[] ReadGroup(UnityBinaryReader r, Header groupHeader, string filePath, GameFormatId formatId)
        {
            var recordList = new List<Record>();
            var endPosition = r.BaseStream.Position + groupHeader.DataSize;
            while (r.BaseStream.Position < endPosition)
            {
                var header = new Header(r, formatId);
                var record = header.CreateRecord(r.BaseStream.Position);
                if (record == null)
                {
                    // Skip the record.
                    r.BaseStream.Position += header.DataSize;
                    continue;
                }
                record.Read(r, filePath, formatId);
                recordList.Add(record);
            }
            return recordList.ToArray();
        }

        void PostProcessRecords()
        {
            recordsByType = new Dictionary<Type, List<IRecord>>();
            objectsByIDString = new Dictionary<string, IRecord>();
            exteriorCELLRecordsByIndices = new Dictionary<Vector2i, CELLRecord>();
            LANDRecordsByIndices = new Dictionary<Vector2i, LANDRecord>();
            foreach (var record in records)
            {
                if (record == null)
                    continue;
                // Add the record to the list for it's type.
                var recordType = record.GetType();
                if (recordsByType.TryGetValue(recordType, out List<IRecord> recordsOfSameType))
                    recordsOfSameType.Add(record);
                else
                {
                    recordsOfSameType = new List<IRecord> { record };
                    recordsByType.Add(recordType, recordsOfSameType);
                }
                // Add the record to the object dictionary if applicable.
                if (record is GMSTRecord) objectsByIDString.Add(((GMSTRecord)record).NAME.Value, record);
                else if (record is GLOBRecord) objectsByIDString.Add(((GLOBRecord)record).NAME.Value, record);
                else if (record is SOUNRecord) objectsByIDString.Add(((SOUNRecord)record).NAME.Value, record);
                else if (record is REGNRecord) objectsByIDString.Add(((REGNRecord)record).NAME.Value, record);
                else if (record is LTEXRecord) objectsByIDString.Add(((LTEXRecord)record).NAME.Value, record);
                else if (record is STATRecord) objectsByIDString.Add(((STATRecord)record).NAME.Value, record);
                else if (record is DOORRecord) objectsByIDString.Add(((DOORRecord)record).NAME.Value, record);
                else if (record is MISCRecord) objectsByIDString.Add(((MISCRecord)record).NAME.Value, record);
                else if (record is WEAPRecord) objectsByIDString.Add(((WEAPRecord)record).NAME.Value, record);
                else if (record is CONTRecord) objectsByIDString.Add(((CONTRecord)record).NAME.Value, record);
                else if (record is LIGHRecord) objectsByIDString.Add(((LIGHRecord)record).NAME.Value, record);
                else if (record is ARMORecord) objectsByIDString.Add(((ARMORecord)record).NAME.Value, record);
                else if (record is CLOTRecord) objectsByIDString.Add(((CLOTRecord)record).NAME.Value, record);
                else if (record is REPARecord) objectsByIDString.Add(((REPARecord)record).NAME.Value, record);
                else if (record is ACTIRecord) objectsByIDString.Add(((ACTIRecord)record).NAME.Value, record);
                else if (record is APPARecord) objectsByIDString.Add(((APPARecord)record).NAME.Value, record);
                else if (record is LOCKRecord) objectsByIDString.Add(((LOCKRecord)record).NAME.Value, record);
                else if (record is PROBRecord) objectsByIDString.Add(((PROBRecord)record).NAME.Value, record);
                else if (record is INGRRecord) objectsByIDString.Add(((INGRRecord)record).NAME.Value, record);
                else if (record is BOOKRecord) objectsByIDString.Add(((BOOKRecord)record).NAME.Value, record);
                else if (record is ALCHRecord) objectsByIDString.Add(((ALCHRecord)record).NAME.Value, record);
                else if (record is CREARecord) objectsByIDString.Add(((CREARecord)record).NAME.Value, record);
                else if (record is NPC_Record) objectsByIDString.Add(((NPC_Record)record).NAME.Value, record);
                // Add the record to exteriorCELLRecordsByIndices if applicable.
                if (record is CELLRecord cell)
                    if (!cell.IsInterior)
                        exteriorCELLRecordsByIndices[cell.GridCoords] = cell;
                // Add the record to LANDRecordsByIndices if applicable.
                if (record is LANDRecord land)
                    LANDRecordsByIndices[land.GridCoords] = land;
            }
        }
    }
}