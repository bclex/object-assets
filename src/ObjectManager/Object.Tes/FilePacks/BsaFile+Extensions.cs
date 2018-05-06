using System;

namespace OA.Tes.FilePacks
{
    partial class BsaFile
    {
        void TestContainsFile()
        {
            foreach (var fileMetadata in FileMetadatas)
                if (!ContainsFile(fileMetadata.Path))
                    throw new FormatException("Hash Invalid");
                else if (FileMetadataHashTable[HashFilePath(fileMetadata.Path)].Path != fileMetadata.Path)
                    throw new FormatException("Hash Invalid");
        }

        void TestLoadFileData()
        {
            foreach (var fileMetadata in FileMetadatas)
            {
                var contents = LoadFileData(fileMetadata);
                if (contents == null)
                    throw new FormatException("Data Invalid");
            }
        }
    }
}