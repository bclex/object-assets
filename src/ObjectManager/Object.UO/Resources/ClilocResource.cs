using OA.Core;
using OA.Core.Diagnostics;
using OA.Ultima.IO;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace OA.Ultima.Resources
{
    public class ClilocResource
    {
        Hashtable _table;

        public readonly string Language;

        public ClilocResource(string language)
        {
            Language = language;
            LoadAllClilocs(language);
        }

        public string GetString(int index)
        {
            if (_table[index] == null)
            {
                Utils.Warning($"Missing cliloc with index {index}. Client version is lower than expected by Server.");
                return $"Err: Cliloc Entry {index} not found.";
            }
            return _table[index].ToString();
        }

        void LoadAllClilocs(string language)
        {
            _table = new Hashtable();
            string mainClilocFile = $"Cliloc.{language}";
            LoadCliloc(mainClilocFile);
            // All the other Cliloc*.language files:
            /*var paths = FileManager.GetFilePaths($"cliloc*.{language}");
            foreach (string path in paths)
                if (path != mainClilocFile)
                    LoadCliloc(path);
            */
        }

        void LoadCliloc(string path)
        {
            path = FileManager.GetFilePath(path);
            if (path == null)
                return;
            byte[] buffer;
            using (var bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                buffer = bin.ReadBytes((int)bin.BaseStream.Length);
                Metrics.ReportDataRead((int)bin.BaseStream.Position);
            }
            var pos = 6;
            var count = buffer.Length;
            while (pos < count)
            {
                var number = BitConverter.ToInt32(buffer, pos);
                var length = BitConverter.ToInt16(buffer, pos + 5);
                var text = Encoding.UTF8.GetString(buffer, pos + 7, length);
                pos += length + 7;
                _table[number] = text; // auto replace with updates.
            }
        }

        class StringEntry
        {
            readonly int _number;
            readonly string _text;

            public int Number { get { return _number; } }
            public string Text { get { return _text; } }

            public StringEntry(int number, string text)
            {
                _number = number;
                _text = text;
            }
        }
    }
}