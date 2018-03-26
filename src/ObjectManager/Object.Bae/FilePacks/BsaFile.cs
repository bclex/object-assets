using OA.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace OA.Bae.FilePacks
{
    public class BsaFile : IDisposable
    {
        UnityBinaryReader r;

        long hashTablePosition;
        long fileDataSectionPostion;

        public struct FileNameHash
        {
            public uint value1;
            public uint value2;

            public override int GetHashCode()
            {
                return unchecked((int)(value1 ^ value2));
            }
        }

        public class FileMetadata
        {
            public uint size;
            public uint offsetInDataSection;
            public string path;
            public FileNameHash pathHash;
        }

        public byte[] version; // 4 bytes
        public FileMetadata[] fileMetadatas;

        public Dictionary<FileNameHash, FileMetadata> fileMetadataHashTable;

        public Core.VirtualFileSystem.Directory rootDir;

        public bool isAtEOF
        {
            get { return r.BaseStream.Position >= r.BaseStream.Length; }
        }

        public BsaFile(string filePath)
        {
            r = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            ReadMetadata();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        ~BsaFile()
        {
            Close();
        }

        public void Close()
        {
            if (r != null)
            {
                r.Close();
                r = null;
            }
        }

        /// <summary>
        /// Determines whether the BSA archive contains a file.
        /// </summary>
        public bool ContainsFile(string filePath)
        {
            return fileMetadataHashTable.ContainsKey(HashFilePath(filePath));
        }

        /// <summary>
        /// Loads an archived file's data.
        /// </summary>
        public byte[] LoadFileData(string filePath)
        {
            var hash = HashFilePath(filePath);
            FileMetadata metadata;
            if (fileMetadataHashTable.TryGetValue(hash, out metadata))
                return LoadFileData(metadata);
            throw new FileNotFoundException("Could not find file \"" + filePath + "\" in a BSA file.");
        }

        /// <summary>
        /// Loads an archived file's data.
        /// </summary>
        public byte[] LoadFileData(FileMetadata fileMetadata)
        {
            r.BaseStream.Position = fileDataSectionPostion + fileMetadata.offsetInDataSection;
            return r.ReadBytes((int)fileMetadata.size);
        }

        private void ReadMetadata()
        {
            // Read the header.
            version = r.ReadBytes(4);
            var hashTableOffsetFromEndOfHeader = r.ReadLEUInt32(); // minus header size (12 bytes)
            var fileCount = r.ReadLEUInt32();
            // Calculate some useful values.
            var headerSize = r.BaseStream.Position;
            hashTablePosition = headerSize + hashTableOffsetFromEndOfHeader;
            fileDataSectionPostion = hashTablePosition + (8 * fileCount);
            // Create file metadatas.
            fileMetadatas = new FileMetadata[fileCount];
            for (var i = 0; i < fileCount; i++)
                fileMetadatas[i] = new FileMetadata();
            // Read file sizes/offsets.
            for (var i = 0; i < fileCount; i++)
            {
                fileMetadatas[i].size = r.ReadLEUInt32();
                fileMetadatas[i].offsetInDataSection = r.ReadLEUInt32();
            }
            // Read filename offsets.
            var filenameOffsets = new uint[fileCount]; // relative offset in filenames section
            for (var i = 0; i < fileCount; i++)
                filenameOffsets[i] = r.ReadLEUInt32();
            // Read filenames.
            var filenamesSectionStartPos = r.BaseStream.Position;
            var filenameBuffer = new List<byte>(64);
            for (var i = 0; i < fileCount; i++)
            {
                r.BaseStream.Position = filenamesSectionStartPos + filenameOffsets[i];
                filenameBuffer.Clear();
                byte curCharAsByte;
                while ((curCharAsByte = r.ReadByte()) != 0)
                    filenameBuffer.Add(curCharAsByte);
                fileMetadatas[i].path = System.Text.Encoding.ASCII.GetString(filenameBuffer.ToArray());
            }
            // Read filename hashes.
            r.BaseStream.Position = hashTablePosition;
            for (int i = 0; i < fileCount; i++)
            {
                fileMetadatas[i].pathHash.value1 = r.ReadLEUInt32();
                fileMetadatas[i].pathHash.value2 = r.ReadLEUInt32();
            }
            // Create the file metadata hash table.
            fileMetadataHashTable = new Dictionary<FileNameHash, FileMetadata>();
            for (var i = 0; i < fileCount; i++)
                fileMetadataHashTable[fileMetadatas[i].pathHash] = fileMetadatas[i];
            // Create a virtual directory tree.
            rootDir = new Core.VirtualFileSystem.Directory();
            foreach (var fileMetadata in fileMetadatas)
                rootDir.CreateDescendantFile(fileMetadata.path);
            // Skip to the file data section.
            r.BaseStream.Position = fileDataSectionPostion;
        }

        private FileNameHash HashFilePath(string filePath)
        {
            filePath = filePath.Replace('/', '\\');
            filePath = filePath.ToLower();
            var hash = new FileNameHash();
            uint len = (uint)filePath.Length;
            uint l = (len >> 1);
            int off, i;
            uint sum, temp, n;
            sum = 0;
            off = 0;
            for (i = 0; i < l; i++)
            {
                sum ^= (uint)(filePath[i]) << (off & 0x1F);
                off += 8;
            }
            hash.value1 = sum;
            sum = 0;
            off = 0;
            for (; i < len; i++)
            {
                temp = (uint)(filePath[i]) << (off & 0x1F);
                sum ^= temp;
                n = temp & 0x1F;
                sum = (sum << (32 - (int)n)) | (sum >> (int)n);
                off += 8;
            }
            hash.value2 = sum;
            return hash;
        }
    }
}