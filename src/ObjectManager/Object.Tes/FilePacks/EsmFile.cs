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
            ReadRecords(filePath, gameId);
            PostProcessRecords();
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

        private void ReadRecords(string filePath, GameId gameId, bool loadHeaderOnly = false)
        {
            var r = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            var header = new Header(r, gameId);
            if ((gameId == GameId.Morrowind && header.Type != "TES3") || (gameId != GameId.Morrowind && header.Type != "TES4"))
                throw new FormatException($"{filePath} record header {header.Type} is not valid for this {gameId}");
            var record = header.CreateUninitializedRecord(0);
            record.Read(r, gameId);
            if (loadHeaderOnly)
                return;
            var recordList = new List<Record>();
            while (r.BaseStream.Position < r.BaseStream.Length)
            {
                var position = r.BaseStream.Position;
                header = new Header(r, gameId);
                var recordPosition = r.BaseStream.Position;
                record = header.CreateUninitializedRecord(recordPosition);
                // Read or skip the record.
                if (record != null)
                {
                    record.Read(r, gameId);
                    if (r.BaseStream.Position != recordPosition + record.Header.DataSize)
                        throw new FormatException($"Failed reading {header.Type} record at offset {position} in {filePath}");
                    recordList.Add(record);
                }
                else r.BaseStream.Position += header.DataSize; // Skip the record.
            }
            records = recordList.ToArray();
        }

        private void PostProcessRecords()
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
                    recordsOfSameType = new List<IRecord>();
                    recordsOfSameType.Add(record);
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
                {
                    if (!cell.IsInterior)
                        exteriorCELLRecordsByIndices[cell.GridCoords] = cell;
                }
                // Add the record to LANDRecordsByIndices if applicable.
                if (record is LANDRecord land)
                {
                    LANDRecordsByIndices[land.GridCoords] = land;
                }
            }
        }
    }
}