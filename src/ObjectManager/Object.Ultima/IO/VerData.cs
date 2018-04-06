using System.IO;

namespace OA.Ultima.IO
{
    public class VerData
    {
        static readonly FileIndexEntry5D[] _patches;
        static readonly Stream _stream;

        public static Stream Stream { get { return _stream; } }
        public static FileIndexEntry5D[] Patches { get { return _patches; } }

        static VerData()
        {
            var path = FileManager.GetFilePath("verdata.mul");
            if (!File.Exists(path))
            {
                _patches = new FileIndexEntry5D[0];
                _stream = Stream.Null;
            }
            else
            {
                _stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var bin = new BinaryReader(_stream);
                _patches = new FileIndexEntry5D[bin.ReadInt32()];
                for (var i = 0; i < _patches.Length; ++i)
                {
                    _patches[i].file = bin.ReadInt32();
                    _patches[i].index = bin.ReadInt32();
                    _patches[i].lookup = bin.ReadInt32();
                    _patches[i].length = bin.ReadInt32();
                    _patches[i].extra = bin.ReadInt32();
                }
            }
        }
    }
}