using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OA.Tes.FilePacks
{
    public partial class BsaMultiFile : IDisposable
    {
        readonly List<BsaFile> _packs = new List<BsaFile>();

        public BsaMultiFile(string[] filePaths)
        {
            //var files = Directory.GetFiles(Path.GetDirectoryName(searchPath), Path.GetFileName(searchPath)).Where(x => Path.GetExtension(x) == ".bsa").ToList();
            var files = filePaths.Where(x => Path.GetExtension(x) == ".bsa" || Path.GetExtension(x) == ".ba2");
            _packs.AddRange(files.Select(x => new BsaFile(x)));
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        ~BsaMultiFile()
        {
            Close();
        }

        public void Close()
        {
        }

        /// <summary>
        /// Determines whether the BSA archive contains a file.
        /// </summary>
        public virtual bool ContainsFile(string filePath)
        {
            return _packs.Any(x => x.ContainsFile(filePath));
        }

        /// <summary>
        /// Loads an archived file's data.
        /// </summary>
        public virtual byte[] LoadFileData(string filePath)
        {
            var pack = _packs.FirstOrDefault(x => x.ContainsFile(filePath));
            if (pack == null)
                throw new FileNotFoundException($"Could not find file \"{filePath}\" in a BSA file.");
            return pack.LoadFileData(filePath);
        }
    }
}