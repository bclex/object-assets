using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OA.Ultima.IO
{
    class MulFileIndex : AFileIndex
    {
        readonly string IndexPath;
        public int patchFile { get; set; }

        /// <summary>
        /// Creates a reference to an index file. (Ex: anim.idx)
        /// </summary>
        /// <param name="idxFile">Name of .idx file in UO base directory.</param>
        /// <param name="mulFile">Name of .mul file that this index file provides an index for.</param>
        /// <param name="length">Number of indexes in this index file.</param>
        /// <param name="patch_file">Index to patch data in Versioning.</param>
        public MulFileIndex(string idxFile, string mulFile, int length, int patch_file)
            : base(mulFile)
        {
            IndexPath = FileManager.GetFilePath(idxFile);
            Length = length;
            patchFile = patch_file;
            Open();
        }

        protected override FileIndexEntry3D[] ReadEntries()
        {
            if (!File.Exists(IndexPath) || !File.Exists(DataPath))
                return new FileIndexEntry3D[0];
            var entries = new List<FileIndexEntry3D>();
            var length = (int)((new FileInfo(IndexPath).Length / 3) / 4);
            using (FileStream index = new FileStream(IndexPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var bin = new BinaryReader(index);
                var count = (int)(index.Length / 12);
                for (var i = 0; i < count && i < length; ++i)
                {
                    var entry = new FileIndexEntry3D(bin.ReadInt32(), bin.ReadInt32(), bin.ReadInt32());
                    entries.Add(entry);
                }
                for (var i = count; i < length; ++i)
                {
                    var entry = new FileIndexEntry3D(-1, -1, -1);
                    entries.Add(entry);
                }
            }
            var patches = VerData.Patches;
            for (var i = 0; i < patches.Length; ++i)
            {
                var patch = patches[i];
                if (patch.file == patchFile && patch.index >= 0 && patch.index < entries.Count)
                {
                    var entry = entries.ElementAt(patch.index);
                    entry.Lookup = patch.lookup;
                    entry.Length = patch.length | (1 << 31);
                    entry.Extra = patch.extra;
                }
            }
            return entries.ToArray();
        }
    }
}
