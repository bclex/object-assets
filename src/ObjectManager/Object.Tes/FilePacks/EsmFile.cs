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
        public override string ToString() => $"{Path.GetFileName(FilePath)}";
        UnityBinaryReader _r;
        public string FilePath;
        public GameFormatId FormatId;
        public Dictionary<string, RecordGroup> Groups;
        public Dictionary<Type, List<IRecord>> recordsByType;
        public Dictionary<string, IRecord> objectsByIDString;
        public Dictionary<Vector2i, CELLRecord> exteriorCELLRecordsByIndices;
        public Dictionary<Vector2i, LANDRecord> LANDRecordsByIndices;

        public EsmFile(string filePath, GameId gameId)
        {
            if (filePath == null)
                return;
            FilePath = filePath;
            FormatId = GetFormatId();
            _r = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            var watch = new Stopwatch();
            watch.Start();
            Read(10);
            Utils.Info($"Loading: {watch.Elapsed}");
            //PostProcessRecords();
            watch.Stop();
            GameFormatId GetFormatId()
            {
                switch (gameId)
                {
                    // tes
                    case GameId.Morrowind: return GameFormatId.TES3;
                    case GameId.Oblivion: return GameFormatId.TES4;
                    case GameId.Skyrim:
                    case GameId.SkyrimSE:
                    case GameId.SkyrimVR: return GameFormatId.TES5;
                    // fallout
                    case GameId.Fallout3:
                    case GameId.FalloutNV: return GameFormatId.TES4;
                    case GameId.Fallout4:
                    case GameId.Fallout4VR: return GameFormatId.TES5;
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

        public void Close()
        {
            if (_r != null)
            {
                _r.Close();
                _r = null;
            }
        }

        public List<IRecord> GetRecordsOfType<T>() where T : Record { return recordsByType.TryGetValue(typeof(T), out List<IRecord> records) ? records : null; }

        void Read(byte level)
        {
            var rootHeader = new Header(_r, FormatId);
            if ((FormatId == GameFormatId.TES3 && rootHeader.Type != "TES3") || (FormatId != GameFormatId.TES3 && rootHeader.Type != "TES4"))
                throw new FormatException($"{FilePath} record header {rootHeader.Type} is not valid for this {FormatId}");
            var rootRecord = rootHeader.CreateRecord(rootHeader.Position);
            rootRecord.Read(_r, FilePath, FormatId);
            // morrowind hack
            if (FormatId == GameFormatId.TES3)
            {
                var group = new RecordGroup(_r, FilePath, FormatId, level);
                group.AddHeader(new Header
                {
                    Label = string.Empty,
                    DataSize = (uint)(_r.BaseStream.Length - _r.BaseStream.Position),
                    Position = _r.BaseStream.Position,
                });
                group.Read();
                Groups = group.Records.GroupBy(x => x.Header.Type)
                    .ToDictionary(x => x.Key, x =>
                    {
                        var s = new RecordGroup(_r, FilePath, FormatId, level) { Records = x.ToList() };
                        s.AddHeader(new Header { Label = x.Key });
                        return s;
                    });
                return;
            }
            // read groups
            Groups = new Dictionary<string, RecordGroup>();
            var endPosition = _r.BaseStream.Length;
            while (_r.BaseStream.Position < endPosition)
            {
                var header = new Header(_r, FormatId);
                if (header.Type != "GRUP")
                    throw new InvalidOperationException($"{header.Type} not GRUP");
                if (!Groups.TryGetValue(header.Label, out RecordGroup group))
                {
                    group = new RecordGroup(_r, FilePath, FormatId, level);
                    Groups.Add(header.Label, group);
                }
                group.AddHeader(header);
                _r.BaseStream.Position += header.DataSize;
                //Console.WriteLine($"Read: {group}");
                //if (group.Label != "CELL" && group.Label != "WRLD")
                if (Header.LoadRecord(group.Label, level))
                    group.Read();
            }
        }

        void PostProcessRecords()
        {
            recordsByType = new Dictionary<Type, List<IRecord>>();
            objectsByIDString = new Dictionary<string, IRecord>();
            exteriorCELLRecordsByIndices = new Dictionary<Vector2i, CELLRecord>();
            LANDRecordsByIndices = new Dictionary<Vector2i, LANDRecord>();
            foreach (var record in Groups.Values.SelectMany(x => x.Records))
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