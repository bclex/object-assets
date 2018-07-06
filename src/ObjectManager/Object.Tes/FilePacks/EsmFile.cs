using OA.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace OA.Tes.FilePacks
{
    public partial class EsmFile : IDisposable
    {
        const int recordHeaderSizeInBytes = 16;
        public override string ToString() => $"{Path.GetFileName(FilePath)}";
        UnityBinaryReader _r;
        public string FilePath;
        public GameFormatId Format;
        public Dictionary<string, RecordGroup> Groups;

        public EsmFile(string filePath, GameId gameId)
        {
            if (filePath == null)
                return;
            FilePath = filePath;
            Format = GetFormatId();
            _r = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            var watch = new Stopwatch();
            watch.Start();
            Read(1);
            Utils.Info($"Loading: {watch.Elapsed}");
            Process();
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

        void Read(int recordLevel)
        {
            var rootHeader = new Header(_r, Format, null);
            if ((Format == GameFormatId.TES3 && rootHeader.Type != "TES3") || (Format != GameFormatId.TES3 && rootHeader.Type != "TES4"))
                throw new FormatException($"{FilePath} record header {rootHeader.Type} is not valid for this {Format}");
            var rootRecord = rootHeader.CreateRecord(rootHeader.Position, recordLevel);
            rootRecord.Read(_r, FilePath, Format);
            // morrowind hack
            if (Format == GameFormatId.TES3)
            {
                var group = new RecordGroup(_r, FilePath, Format, recordLevel);
                group.AddHeader(new Header
                {
                    Label = null,
                    DataSize = (uint)(_r.BaseStream.Length - _r.BaseStream.Position),
                    Position = _r.BaseStream.Position,
                });
                group.Load();
                Groups = group.Records.GroupBy(x => x.Header.Type)
                    .ToDictionary(x => x.Key, x =>
                    {
                        var s = new RecordGroup(_r, FilePath, Format, recordLevel) { Records = x.ToList() };
                        s.AddHeader(new Header { Label = Encoding.ASCII.GetBytes(x.Key) });
                        return s;
                    });
                return;
            }
            // read groups
            Groups = new Dictionary<string, RecordGroup>();
            var endPosition = _r.BaseStream.Length;
            while (_r.BaseStream.Position < endPosition)
            {
                var header = new Header(_r, Format, null);
                if (header.Type != "GRUP")
                    throw new InvalidOperationException($"{header.Type} not GRUP");
                var nextPosition = _r.BaseStream.Position + header.DataSize;
                var label = Encoding.ASCII.GetString(header.Label);
                if (!Groups.TryGetValue(label, out RecordGroup group))
                {
                    group = new RecordGroup(_r, FilePath, Format, recordLevel);
                    Groups.Add(label, group);
                }
                group.AddHeader(header);
                _r.BaseStream.Position = nextPosition;
            }
        }
    }
}