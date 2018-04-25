using OA.Ultima.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace OA.Ultima.Resources
{
    public class MusicData
    {
        const string _configFilePath = @"Music/Digital/Config.txt";
        static char[] _configFileDelimiters = { ' ', ',', '\t' };

        static readonly Dictionary<int, Tuple<string, bool>> _musicData = new Dictionary<int, Tuple<string, bool>>();

        static MusicData()
        {
            // open UO's music Config.txt
            if (!FileManager.Exists(_configFilePath))
                return;
            // attempt to read out all the values from the file.
            using (var reader = new StreamReader(FileManager.GetFile(_configFilePath)))
            {
                String line;
                while ((line = reader.ReadLine()) != null)
                    if (TryParseConfigLine(line, out Tuple<int, string, bool> songData))
                        _musicData.Add(songData.Item1, new Tuple<string, bool>(songData.Item2, songData.Item3));
            }
        }

        /// <summary>
        /// Attempts to parse a line from UO's music Config.txt.
        /// </summary>
        /// <param name="line">A line from the file.</param>
        /// <param name="?">If successful, contains a tuple with these fields: int songIndex, string songName, bool doesLoop</param>
        /// <returns>true if line could be parsed, false otherwise.</returns>
        private static bool TryParseConfigLine(string line, out Tuple<int, string, bool> songData)
        {
            songData = null;
            var splits = line.Split(_configFileDelimiters);
            if (splits.Length < 2 || splits.Length > 3)
                return false;
            var index = int.Parse(splits[0]);
            var name = splits[1].Trim();
            var doesLoop = splits.Length == 3 && splits[2] == "loop";
            songData = new Tuple<int, string, bool>(index, name, doesLoop);
            return true;
        }

        public static bool TryGetMusicData(int index, out string name, out bool doesLoop)
        {
            name = null;
            doesLoop = false;
            if (_musicData.ContainsKey(index))
            {
                name = _musicData[index].Item1;
                doesLoop = _musicData[index].Item2;
                return true;
            }
            return false;
        }
    }
}
