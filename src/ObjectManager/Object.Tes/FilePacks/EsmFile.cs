using OA.Core;
using OA.Tes.FilePacks.Records;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OA.Tes.FilePacks
{
    public partial class EsmFile : IDisposable
    {
        const int recordHeaderSizeInBytes = 16;
        public Dictionary<string, Record[]> records = new Dictionary<string, Record[]>();
        public Dictionary<Type, List<IRecord>> recordsByType;
        public Dictionary<string, IRecord> objectsByIDString;
        public Dictionary<Vector2i, CELLRecord> exteriorCELLRecordsByIndices;
        public Dictionary<Vector2i, LANDRecord> LANDRecordsByIndices;

        public EsmFile(string filePath, GameId gameId)
        {
            var watch = new Stopwatch();
            watch.Start();
            Read(filePath, GetFormatId(), 5);
            Utils.Info($"Loading: {watch.Elapsed}");
            PostProcessRecords();
            watch.Stop();
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

        void Read(string filePath, GameFormatId formatId, byte level)
        {
            var r = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            var header = new Header(r, formatId);
            if ((formatId == GameFormatId.Tes3 && header.Type != "TES3") || (formatId != GameFormatId.Tes3 && header.Type != "TES4"))
                throw new FormatException($"{filePath} record header {header.Type} is not valid for this {formatId}");
            var rootRecord = header.CreateRecord(r.BaseStream.Position, level);
            rootRecord.Read(r, filePath, formatId);
            // read stream
            var recordList = new List<Record>();
            var endPosition = r.BaseStream.Length;
            while (r.BaseStream.Position < endPosition)
            {
                header = new Header(r, formatId);
                if (header.Type == "GRUP")
                    records.Add(header.Label, ReadGroup(r, header, filePath, formatId, level));
                else
                {
                    var record = header.CreateRecord(r.BaseStream.Position, level);
                    if (record == null)
                    {
                        r.BaseStream.Position += header.DataSize;
                        continue;
                    }
                    record.Read(r, filePath, formatId);
                    recordList.Add(record);
                }
            }
            if (recordList.Count > 0)
                records.Add(string.Empty, recordList.ToArray());
        }

        Record[] ReadGroup(UnityBinaryReader r, Header groupHeader, string filePath, GameFormatId formatId, byte level)
        {
            var recordList = new List<Record>();
            var endPosition = r.BaseStream.Position + groupHeader.DataSize;
            while (r.BaseStream.Position < endPosition)
            {
                var header = new Header(r, formatId);
                var record = header.CreateRecord(r.BaseStream.Position, level);
                if (record == null)
                {
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
            foreach (var record in records.SelectMany(x => x.Value))
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
                if (record is IHaveEDID edid) objectsByIDString.Add(edid.EDID.Value, record);
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