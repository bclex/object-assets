using System;
using System.Linq;

namespace OA.Tes.FilePacks
{
    partial class BsaFile
    {
        void TestContainsFile()
        {
            foreach (var file in _files)
            {
                Console.WriteLine(file.Path);
                if (!ContainsFile(file.Path))
                    throw new FormatException("Hash Invalid");
                else if (!_filesByHash[HashFilePath(file.Path)].Any(x => x.Path == file.Path))
                    throw new FormatException("Hash Invalid");
            }
        }

        void TestLoadFileData()
        {
            foreach (var file in _files)
            {
                Console.WriteLine(file.Path);
                LoadFileData(file);
            }
        }
    }
}