using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
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
        public Dictionary<string, EsmGroup> Groups;
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

        public class EsmGroup
        {
            public string Label => Headers.First.Value.Label;
            public override string ToString() => Headers.First.Value.Label;
            public LinkedList<Header> Headers = new LinkedList<Header>();
            public List<Record> Records = new List<Record>();
            public List<EsmGroup> Groups;
            readonly UnityBinaryReader _r;
            readonly string _filePath;
            readonly GameFormatId _formatId;
            readonly byte _level;
            int _headerSkip;

            public EsmGroup(UnityBinaryReader r, string filePath, GameFormatId formatId, byte level)
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

            void ReadGroup(Header groupHeader)
            {
                _r.BaseStream.Position = groupHeader.Position;
                var endPosition = groupHeader.Position + groupHeader.DataSize;
                while (_r.BaseStream.Position < endPosition)
                {
                    var header = new Header(_r, _formatId);
                    if (header.Type == "GRUP")
                    {
                        if (Groups == null)
                            Groups = new List<EsmGroup>();
                        var group = new EsmGroup(_r, _filePath, _formatId, _level);
                        group.AddHeader(header);
                        Groups.Add(group);
                        group.Read(); Console.WriteLine($"Read: {groupHeader.Label}/{group}");
                        //_r.BaseStream.Position += header.DataSize;
                        continue;
                    }
                    var record = header.CreateRecord(_r.BaseStream.Position, _level);
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

        void Read(byte level)
        {
            var rootHeader = new Header(_r, FormatId);
            if ((FormatId == GameFormatId.TES3 && rootHeader.Type != "TES3") || (FormatId != GameFormatId.TES3 && rootHeader.Type != "TES4"))
                throw new FormatException($"{FilePath} record header {rootHeader.Type} is not valid for this {FormatId}");
            var rootRecord = rootHeader.CreateRecord(_r.BaseStream.Position, level);
            rootRecord.Read(_r, FilePath, FormatId);
            // morrowind hack
            if (FormatId == GameFormatId.TES3)
            {
                var group = new EsmGroup(_r, FilePath, FormatId, level);
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
                        var s = new EsmGroup(_r, FilePath, FormatId, level) { Records = x.ToList() };
                        s.AddHeader(new Header { Label = x.Key });
                        return s;
                    });
                return;
            }
            // read groups
            Groups = new Dictionary<string, EsmGroup>();
            var endPosition = _r.BaseStream.Length;
            while (_r.BaseStream.Position < endPosition)
            {
                var header = new Header(_r, FormatId);
                if (header.Type != "GRUP")
                    throw new InvalidOperationException($"{header.Type} not GRUP");
                if (!Groups.TryGetValue(header.Label, out EsmGroup group))
                {
                    group = new EsmGroup(_r, FilePath, FormatId, level);
                    Groups.Add(header.Label, group);
                }
                group.AddHeader(header);
                _r.BaseStream.Position += header.DataSize;
                if (group.Label != "CELL" && group.Label != "WRLD")
                {
                    group.Read(); Console.WriteLine($"Read: {group}");
                }
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