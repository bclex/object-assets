using OA.Core.Diagnostics;
using OA.Ultima.Data;
using OA.Ultima.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace OA.Ultima.Resources
{
    public static class SoundData
    {
        static AFileIndex _index;
        //static Stream _stream;
        static Dictionary<int, int> _translations;
        static bool _filesPrepared;

        public static bool TryGetSoundData(int soundID, out byte[] data, out string name)
        {
            // Sounds.mul is exclusively locked by the legacy client, so we need to make sure this file is available
            // before attempting to play any sounds.
            if (!_filesPrepared)
                setupFiles();
            data = null;
            name = null;
            if (!_filesPrepared || soundID < 0) return false;
            else
            {
                var r = _index.Seek(soundID, out int length, out int extra, out bool patched);
                var streamStart = (int)r.Position;
                var offset = (int)r.Position;
                if (offset < 0 || length <= 0)
                {
                    if (!_translations.TryGetValue(soundID, out soundID))
                        return false;
                    r = _index.Seek(soundID, out length, out extra, out patched);
                    streamStart = (int)r.Position;
                    offset = (int)r.Position;
                }
                if (offset < 0 || length <= 0)
                    return false;
                var stringBuffer = new byte[40];
                data = new byte[length - 40];
                r.Seek((long)(offset), SeekOrigin.Begin);
                stringBuffer = r.ReadBytes(40);
                data = r.ReadBytes(length - 40);
                name = Encoding.ASCII.GetString(stringBuffer).Trim();
                var end = name.IndexOf("\0");
                name = name.Substring(0, end);
                Metrics.ReportDataRead((int)r.Position - streamStart);
                return true;
            }
        }

        private static void setupFiles()
        {
            try
            {
                _index = ClientVersion.InstallationIsUopFormat ?
                    FileManager.CreateFileIndex("soundLegacyMUL.uop", 0xFFF, false, ".dat") :
                    FileManager.CreateFileIndex("soundidx.mul", "sound.mul", 0x1000, -1); // new BinaryReader(new FileStream(FileManager.GetFilePath("soundidx.mul"), FileMode.Open));
                _filesPrepared = true;
            }
            catch
            {
                _filesPrepared = false;
                return;
            }
            var reg = new Regex(@"(\d{1,3}) \x7B(\d{1,3})\x7D (\d{1,3})", RegexOptions.Compiled);
            _translations = new Dictionary<int, int>();
            string line;
            using (var reader = new StreamReader(FileManager.GetFilePath("Sound.def")))
                while ((line = reader.ReadLine()) != null)
                    if (((line = line.Trim()).Length != 0) && !line.StartsWith("#"))
                    {
                        var match = reg.Match(line);
                        if (match.Success)
                            _translations.Add(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                    }
        }
    }
}