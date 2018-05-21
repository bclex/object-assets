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
        public ILookup<string, EsmGroup> Groups;
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
            public override string ToString() => Header.Label;
            public List<Record> Records;
            public List<EsmGroup> Groups;
            public Header Header;
            public long Position;
            readonly UnityBinaryReader _r;
            readonly string _filePath;
            readonly GameFormatId _formatId;
            readonly byte _level;

            public EsmGroup(UnityBinaryReader r, string filePath, GameFormatId formatId, byte level)
            {
                _r = r;
                _filePath = filePath;
                _formatId = formatId;
                _level = level;
            }

            public void Read()
            {
                if (Records != null) return;
                lock (_r)
                {
                    if (Records != null) return;
                    Records = ReadGroup();
                }
            }

            public List<Record> ReadGroup()
            {
                var records = new List<Record>();
                _r.BaseStream.Position = Position;
                var endPosition = Position + Header.DataSize;
                while (_r.BaseStream.Position < endPosition)
                {
                    var header = new Header(_r, _formatId);
                    if (header.Type == "GRUP")
                    {
                        if (Groups == null)
                            Groups = new List<EsmGroup>();
                        var group = new EsmGroup(_r, _filePath, _formatId, _level)
                        {
                            Position = _r.BaseStream.Position,
                            Header = header,
                        };
                        Groups.Add(group);
                        group.Read();
                        Console.WriteLine($"Read: {Header.Label}/{group.Header.Label}");
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
                    records.Add(record);
                }
                return records;
            }

            void ReadRecord(Record record, bool compressed)
            {
                if (compressed)
                {
                    // decompress record
                    var newDataSize = _r.ReadLEUInt32();
                    var data = _r.ReadBytes((int)record.Header.DataSize);
                    var newData = new byte[newDataSize];
                    using (var s = new MemoryStream(data))
                    using (var gs = new InflaterInputStream(s))
                        gs.Read(newData, 0, newData.Length);
                    // read record
                    record.Position = 0;
                    record.Header.DataSize = newDataSize;
                    using (var s = new MemoryStream(newData))
                    using (var r = new UnityBinaryReader(s))
                        record.Read(r, _filePath, _formatId);
                    return;
                }
                record.Read(_r, _filePath, _formatId);
            }
        }

        void Read(byte level)
        {
            var rootHeader = new Header(_r, FormatId);
            if ((FormatId == GameFormatId.Tes3 && rootHeader.Type != "TES3") || (FormatId != GameFormatId.Tes3 && rootHeader.Type != "TES4"))
                throw new FormatException($"{FilePath} record header {rootHeader.Type} is not valid for this {FormatId}");
            var rootRecord = rootHeader.CreateRecord(_r.BaseStream.Position, level);
            rootRecord.Read(_r, FilePath, FormatId);
            var groups = new List<EsmGroup>();
            // morrowind hack
            if (FormatId == GameFormatId.Tes3)
            {
                var group = new EsmGroup(_r, FilePath, FormatId, level)
                {
                    Position = _r.BaseStream.Position,
                    Header = new Header { Label = string.Empty, DataSize = (uint)(_r.BaseStream.Length - _r.BaseStream.Position) },
                };
                groups.Add(group);
                Console.WriteLine($"Read: {group.Header.Label}");
                group.Read();
                return;
            }
            // read groups
            var endPosition = _r.BaseStream.Length;
            while (_r.BaseStream.Position < endPosition)
            {
                var header = new Header(_r, FormatId);
                if (header.Type != "GRUP")
                    throw new InvalidOperationException();
                var group = new EsmGroup(_r, FilePath, FormatId, level)
                {
                    Position = _r.BaseStream.Position,
                    Header = header,
                };
                groups.Add(group);
                _r.BaseStream.Position += header.DataSize;
                Console.WriteLine($"Read: {group.Header.Label}");
                group.Read();
            }
            Groups = groups.ToLookup(x => x.Header.Label);
        }

        void PostProcessRecords()
        {
            recordsByType = new Dictionary<Type, List<IRecord>>();
            objectsByIDString = new Dictionary<string, IRecord>();
            exteriorCELLRecordsByIndices = new Dictionary<Vector2i, CELLRecord>();
            LANDRecordsByIndices = new Dictionary<Vector2i, LANDRecord>();
            foreach (var record in Groups.SelectMany(x => x.SelectMany(y => y.Records)))
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