using OA.Ultima.Core.IO;
using System.IO;

namespace OA.Ultima.IO
{
    public abstract class AFileIndex
    {
        FileIndexEntry3D[] _index;
        Stream _stream;

        public FileIndexEntry3D[] Index => _index;
        public Stream Stream => _stream;

        public string DataPath { get; private set; }
        public int Length { get; protected set; }

        protected abstract FileIndexEntry3D[] ReadEntries();

        protected AFileIndex(string dataPath)
        {
            DataPath = dataPath;
        }

        protected AFileIndex(string dataPath, int length)
        {
            Length = length;
            DataPath = dataPath;
        }

        public void Open()
        {
            _index = ReadEntries();
            Length = _index.Length;
        }

        public BinaryFileReader Seek(int index, out int length, out int extra, out bool patched)
        {
            if (Stream == null)
                _stream = new FileStream(DataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (index < 0 || index >= _index.Length)
            {
                length = extra = 0;
                patched = false;
                return null;
            }
            var e = _index[index];
            if (e.Lookup < 0)
            {
                length = extra = 0;
                patched = false;
                return null;
            }
            length = e.Length & 0x7FFFFFFF;
            extra = e.Extra;
            if ((e.Length & 0xFF000000) != 0)
            {
                patched = true;
                VerData.Stream.Seek(e.Lookup, SeekOrigin.Begin);
                return new BinaryFileReader(new BinaryReader(VerData.Stream));
            }
            else if (_stream == null)
            {
                length = extra = 0;
                patched = false;
                return null;
            }
            patched = false;
            _stream.Position = e.Lookup;
            return new BinaryFileReader(new BinaryReader(_stream));
        }
    }
}
