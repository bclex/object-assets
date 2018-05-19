using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OA.Tes.FilePacks
{
    public partial class BsaMultiFile : IDisposable
    {
        public readonly List<BsaFile> Packs = new List<BsaFile>();

        public BsaMultiFile(string[] filePaths)
        {
            if (filePaths == null)
                throw new ArgumentNullException("filePaths");
            var files = filePaths.Where(x => Path.GetExtension(x) == ".bsa" || Path.GetExtension(x) == ".ba2");
            Packs.AddRange(files.Select(x => new BsaFile(x)));
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
            foreach (var pack in Packs)
                pack.Close();
        }

        /// <summary>
        /// Determines whether the BSA archive contains a file.
        /// </summary>
        public virtual bool ContainsFile(string filePath)
        {
            return Packs.Any(x => x.ContainsFile(filePath));
        }

        /// <summary>
        /// Loads an archived file's data.
        /// </summary>
        public virtual byte[] LoadFileData(string filePath)
        {
            var pack = Packs.FirstOrDefault(x => x.ContainsFile(filePath));
            if (pack == null)
                throw new FileNotFoundException($"Could not find file \"{filePath}\" in a BSA file.");
            return pack.LoadFileData(filePath);
        }
    }
}