using OA.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OA.Tes.FilePacks
{
    // https://github.com/jonwd7/bae/blob/master/src/bsa.cpp
    public partial class BsaFile : IDisposable
    {
        UnityBinaryReader _r;
        long _hashTablePosition;
        long _fileDataSectionPostion;

        // Default header data
        const uint MW_BSAHEADER_FILEID = 0x00000100; // Magic for Morrowind BSA
        const uint OB_BSAHEADER_FILEID = 0x00415342; // Magic for Oblivion BSA, the literal string "BSA\0".
        const uint F4_BSAHEADER_FILEID = 0x58445442; // Magic for Fallout 4 BA2, the literal string "BTDX".
        const uint OB_BSAHEADER_VERSION = 0x67; // Version number of an Oblivion BSA
        const uint F3_BSAHEADER_VERSION = 0x68; // Version number of a Fallout 3 BSA
        const uint SSE_BSAHEADER_VERSION = 0x69; // Version number of a Skyrim SE BSA
        const uint F4_BSAHEADER_VERSION = 0x01; // Version number of a Fallout 4 BA2

        // Archive flags
        const ushort OB_BSAARCHIVE_PATHNAMES = 0x0001; // Whether the BSA has names for paths
        const ushort OB_BSAARCHIVE_FILENAMES = 0x0002; // Whether the BSA has names for files
        const ushort OB_BSAARCHIVE_COMPRESSFILES = 0x0004; // Whether the files are compressed
        const ushort F3_BSAARCHIVE_PREFIXFULLFILENAMES = 0x0100; // Whether the name is prefixed to the data?

        // File flags
        const ushort OB_BSAFILE_NIF = 0x0001; // Set when the BSA contains NIF files
        const ushort OB_BSAFILE_DDS = 0x0002; // Set when the BSA contains DDS files
        const ushort OB_BSAFILE_XML = 0x0004; // Set when the BSA contains XML files
        const ushort OB_BSAFILE_WAV = 0x0008; // Set when the BSA contains WAV files
        const ushort OB_BSAFILE_MP3 = 0x0010; // Set when the BSA contains MP3 files
        const ushort OB_BSAFILE_TXT = 0x0020; // Set when the BSA contains TXT files
        const ushort OB_BSAFILE_HTML = 0x0020; // Set when the BSA contains HTML files
        const ushort OB_BSAFILE_BAT = 0x0020; // Set when the BSA contains BAT files
        const ushort OB_BSAFILE_SCC = 0x0020; // Set when the BSA contains SCC files
        const ushort OB_BSAFILE_SPT = 0x0040; // Set when the BSA contains SPT files
        const ushort OB_BSAFILE_TEX = 0x0080; // Set when the BSA contains TEX files
        const ushort OB_BSAFILE_FNT = 0x0080; // Set when the BSA contains FNT files
        const ushort OB_BSAFILE_CTL = 0x0100; // Set when the BSA contains CTL files

        // Bitmasks for the size field in the header
        const uint OB_BSAFILE_SIZEMASK = 0x3FFFFFFF; // Bit mask with OBBSAFileInfo::sizeFlags to get the size of the file

        // Record flags
        const uint OB_BSAFILE_FLAG_COMPRESS = 0xC0000000; // Bit mask with OBBSAFileInfo::sizeFlags to get the compression status

        public class FileMetadata
        {
            public uint Size;
            public long OffsetInDataSection;
            public string Path;
            public ulong PathHash;
        }

        public class F4FileMetadata : FileMetadata
        {
            public F4TexInfo TexInfo;
            public F4TexChunk[] TexChunks;
        }

        public class OBFileMetadata : FileMetadata
        {
        }

        public class MWFileMetadata : FileMetadata
        {
        }

        public uint Magic; // 4 bytes
        public uint Version; // 4 bytes
        public bool HasNamePrefix;
        public FileMetadata[] FileMetadatas;
        public Dictionary<ulong, FileMetadata> FileMetadataHashTable;
        public VirtualFileSystem.Directory RootDir;

        public bool IsAtEof => _r.BaseStream.Position >= _r.BaseStream.Length;

        public BsaFile(string filePath)
        {
            if (filePath == null)
                return;
            _r = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            ReadMetadata();
            TestContainsFile();
            //TestLoadFileData();
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
            if (_r != null)
            {
                _r.Close();
                _r = null;
            }
        }

        /// <summary>
        /// Determines whether the BSA archive contains a file.
        /// </summary>
        public bool ContainsFile(string filePath)
        {
            return FileMetadataHashTable.ContainsKey(HashFilePath(filePath));
        }

        /// <summary>
        /// Loads an archived file's data.
        /// </summary>
        public byte[] LoadFileData(string filePath)
        {
            var hash = HashFilePath(filePath);
            if (FileMetadataHashTable.TryGetValue(hash, out FileMetadata metadata))
                return LoadFileData(metadata);
            throw new FileNotFoundException($"Could not find file \"{filePath}\" in a BSA file.");
        }

        /// <summary>
        /// Loads an archived file's data.
        /// </summary>
        public byte[] LoadFileData(FileMetadata fileMetadata)
        {
            _r.BaseStream.Position = _fileDataSectionPostion + fileMetadata.OffsetInDataSection;
            //
            return _r.ReadBytes((int)fileMetadata.Size);
        }

        public struct F4TexInfo
        {
            public ushort Height;
            public ushort Width;
            public byte NumMips;
            public byte Format;
        }

        public struct F4TexChunk
        {
            public ulong Offset;
            public uint PackedSize;
            public uint UnpackedSize;
            public ushort StartMip;
            public ushort EndMip;
            public uint Unk14;
        }

        private void ReadMetadata()
        {
            // Open
            Magic = BitConverter.ToUInt32(_r.ReadBytes(4), 0);
            if (Magic == F4_BSAHEADER_FILEID)
            {
                Version = BitConverter.ToUInt32(_r.ReadBytes(4), 0);
                if (Version != F4_BSAHEADER_VERSION)
                    throw new InvalidOperationException("BAD MAGIC");
                // Read the header
                var header_Type = _r.ReadASCIIString(4);            // 08 GNRL=General, DX10=Textures
                var header_NumFiles = _r.ReadLEUInt32();            // 0C
                var header_NameTableOffset = _r.ReadLEUInt64();     // 10 - relative to start of file

                // Create file metadatas
                _r.BaseStream.Position = (long)header_NameTableOffset;
                FileMetadatas = new F4FileMetadata[header_NumFiles];
                for (var i = 0; i < header_NumFiles; i++)
                {
                    var length = _r.ReadLEUInt16();
                    var path = _r.ReadASCIIString(length);
                    FileMetadatas[i] = new F4FileMetadata
                    {
                        Path = path,
                        PathHash = Tes4HashFilePath(path),
                    };
                }
                if (header_Type == "GNRL") // General BA2 Format
                {
                    _r.BaseStream.Position = 16 + 8; // sizeof(header) + 8
                    for (var i = 0; i < header_NumFiles; i++)
                    {
                        var info_NameHash = _r.ReadLEUInt32();      // 00
                        var info_Ext = _r.ReadASCIIString(4);       // 04 - extension
                        var info_DirHash = _r.ReadLEUInt32();       // 08
                        var info_Unk0C = _r.ReadLEUInt32();         // 0C - flags? 00100100
                        var info_Offset = _r.ReadLEUInt64();        // 10 - relative to start of file
                        var info_PackedSize = _r.ReadLEUInt32();    // 18 - packed length (zlib)
                        var info_UnpackedSize = _r.ReadLEUInt32();  // 1C - unpacked length
                        var info_Unk20 = _r.ReadLEUInt32();         // 20 - BAADF00D
                        FileMetadatas[i].OffsetInDataSection = (long)info_Offset;
                        FileMetadatas[i].Size = info_UnpackedSize;
                    }
                }
                else if (header_Type == "DX10") // Texture BA2 Format
                {
                    _r.BaseStream.Position = 16 + 8; // sizeof(header) + 8
                    for (var i = 0; i < header_NumFiles; i++)
                    {
                        var fileMetadata = (F4FileMetadata)FileMetadatas[i];
                        var info_NameHash = _r.ReadLEUInt32();      // 00
                        var info_Ext = _r.ReadASCIIString(4);       // 04
                        var info_DirHash = _r.ReadLEUInt32();       // 08
                        var info_Unk0C = _r.ReadByte();             // 0C
                        var info_NumChunks = _r.ReadByte();         // 0D
                        var info_ChunkHeaderSize = _r.ReadLEUInt16();// 0E - size of one chunk header
                        var info_Height = _r.ReadLEUInt16();        // 10
                        var info_Width = _r.ReadLEUInt16();         // 12
                        var info_NumMips = _r.ReadByte();           // 14
                        var info_Format = _r.ReadByte();            // 15 - DXGI_FORMAT
                        var info_Unk16 = _r.ReadLEUInt16();         // 16 - 0800
                        fileMetadata.TexInfo = new F4TexInfo
                        {
                            Height = info_Height,
                            Width = info_Width,
                            NumMips = info_NumMips,
                            Format = info_Format,
                        };
                        // read tex-chunks
                        var texChunks = new F4TexChunk[info_NumChunks];
                        for (var j = 0; j < info_NumChunks; j++)
                            texChunks[j] = new F4TexChunk
                            {
                                Offset = _r.ReadLEUInt64(),         // 00
                                PackedSize = _r.ReadLEUInt32(),     // 08
                                UnpackedSize = _r.ReadLEUInt32(),   // 0C
                                StartMip = _r.ReadLEUInt16(),       // 10
                                EndMip = _r.ReadLEUInt16(),         // 12
                                Unk14 = _r.ReadLEUInt32(),          // 14 - BAADFOOD
                            };
                        fileMetadata.TexChunks = texChunks;
                    }
                }
            }
            else if (Magic == OB_BSAHEADER_FILEID)
            {
                Version = BitConverter.ToUInt32(_r.ReadBytes(4), 0);
                if (Version != OB_BSAHEADER_VERSION && Version != F3_BSAHEADER_VERSION && Version != SSE_BSAHEADER_VERSION)
                    throw new InvalidOperationException("BAD MAGIC");
                // Read the header
                var header_FolderRecordOffset = _r.ReadLEUInt32(); // Offset of beginning of folder records
                var header_ArchiveFlags = _r.ReadLEUInt32(); // Archive flags
                var header_FolderCount = _r.ReadLEUInt32(); // Total number of folder records (OBBSAFolderInfo)
                var header_FileCount = _r.ReadLEUInt32(); // Total number of file records (OBBSAFileInfo)
                var header_FolderNameLength = _r.ReadLEUInt32(); // Total length of folder names
                var header_FileNameLength = _r.ReadLEUInt32(); // Total length of file names
                var header_FileFlags = _r.ReadLEUInt32(); // File flags

                // Calculate some useful values
                if ((header_ArchiveFlags & OB_BSAARCHIVE_PATHNAMES) == 0 || (header_ArchiveFlags & OB_BSAARCHIVE_FILENAMES) == 0)
                    throw new InvalidOperationException("HEADER FLAGS");
                var compressToggle = header_ArchiveFlags & OB_BSAARCHIVE_COMPRESSFILES;
                if (Version == F3_BSAHEADER_VERSION || Version == SSE_BSAHEADER_VERSION)
                    HasNamePrefix = (header_ArchiveFlags & F3_BSAARCHIVE_PREFIXFULLFILENAMES) != 0;
                var folderSize = Version != SSE_BSAHEADER_VERSION ? 16 : 24;

                // Create file metadatas
                FileMetadatas = new FileMetadata[header_FileCount];
                var filenamesSectionStartPos = _r.BaseStream.Position = header_FolderRecordOffset + header_FolderNameLength + header_FolderCount * (folderSize + 1) + header_FileCount * 16;
                var buf = new List<byte>(64);
                for (var i = 0; i < header_FileCount; i++)
                {
                    buf.Clear();
                    byte curCharAsByte; while ((curCharAsByte = _r.ReadByte()) != 0)
                        buf.Add(curCharAsByte);
                    var path = Encoding.ASCII.GetString(buf.ToArray());
                    FileMetadatas[i] = new FileMetadata
                    {
                        Path = path,
                    };
                }
                if (_r.BaseStream.Position != filenamesSectionStartPos + header_FileNameLength)
                    throw new InvalidOperationException("HEADER FILENAMES");

                // read-all folders
                _r.BaseStream.Position = header_FolderRecordOffset;
                var folders = new Tuple<uint>[header_FolderCount];
                for (var i = 0; i < header_FolderCount; i++)
                {
                    var folder_Hash = _r.ReadLEUInt64(); // Hash of the folder name
                    var folder_FileCount = _r.ReadLEUInt32(); // Number of files in folder
                    var folder_Unk = 0U; var folder_Offset = 0UL;
                    if (Version == SSE_BSAHEADER_VERSION) { folder_Unk = _r.ReadLEUInt32(); folder_Offset = _r.ReadLEUInt64(); }
                    else folder_Offset = _r.ReadLEUInt32();
                    folders[i] = new Tuple<uint>(folder_FileCount);
                }

                // add file
                var fileNameIndex = 0U;
                for (var i = 0; i < header_FolderCount; i++)
                {
                    var folder_name = _r.ReadPossiblyNullTerminatedASCIIString(_r.ReadByte()); // BSAReadSizedString
                    var folder = folders[i];
                    for (var j = 0; j < folder.Item1; j++)
                    {
                        var file_Hash = _r.ReadLEUInt64(); // Hash of the filename
                        var file_SizeFlags = _r.ReadLEUInt32(); // Size of the data, possibly with OB_BSAFILE_FLAG_COMPRESS set
                        var file_Offset = _r.ReadLEUInt32(); // Offset to raw file data
                        var fileMetadata = FileMetadatas[fileNameIndex++];
                        fileMetadata.Size = file_SizeFlags;
                        fileMetadata.OffsetInDataSection = file_Offset;
                        var path = folder_name + "\\" + fileMetadata.Path;
                        fileMetadata.Path = path;
                        fileMetadata.PathHash = Tes4HashFilePath(path);
                    }
                }
            }
            else if (Magic == MW_BSAHEADER_FILEID)
            {
                // Read the header
                var header_HashOffset = _r.ReadLEUInt32(); // Offset of hash table minus header size (12)
                var header_FileCount = _r.ReadLEUInt32(); // Number of files in the archive

                // Calculate some useful values
                var headerSize = _r.BaseStream.Position;
                _hashTablePosition = headerSize + header_HashOffset;
                _fileDataSectionPostion = _hashTablePosition + (8 * header_FileCount);

                // Create file metadatas
                FileMetadatas = new FileMetadata[header_FileCount];
                for (var i = 0; i < header_FileCount; i++)
                    FileMetadatas[i] = new FileMetadata
                    {
                        // Read file sizes/offsets
                        Size = _r.ReadLEUInt32(),
                        OffsetInDataSection = _r.ReadLEUInt32(),
                    };

                // Read filename offsets
                var filenameOffsets = new uint[header_FileCount]; // relative offset in filenames section
                for (var i = 0; i < header_FileCount; i++)
                    filenameOffsets[i] = _r.ReadLEUInt32();

                // Read filenames
                var filenamesSectionStartPos = _r.BaseStream.Position;
                var buf = new List<byte>(64);
                for (var i = 0; i < header_FileCount; i++)
                {
                    _r.BaseStream.Position = filenamesSectionStartPos + filenameOffsets[i];
                    buf.Clear();
                    byte curCharAsByte; while ((curCharAsByte = _r.ReadByte()) != 0)
                        buf.Add(curCharAsByte);
                    FileMetadatas[i].Path = Encoding.ASCII.GetString(buf.ToArray());
                }

                // Read filename hashes
                _r.BaseStream.Position = _hashTablePosition;
                for (var i = 0; i < header_FileCount; i++)
                    FileMetadatas[i].PathHash = (_r.ReadLEUInt32() << 32) | (ulong)_r.ReadLEUInt32();
            }
            else throw new InvalidOperationException("BAD MAGIC");

            // Create the file metadata hash table
            FileMetadataHashTable = new Dictionary<ulong, FileMetadata>();
            for (var i = 0; i < FileMetadatas.Length; i++)
                FileMetadataHashTable[FileMetadatas[i].PathHash] = FileMetadatas[i];

            // Create a virtual directory tree.
            RootDir = new VirtualFileSystem.Directory();
            foreach (var fileMetadata in FileMetadatas)
                RootDir.CreateDescendantFile(fileMetadata.Path);
            // Skip to the file data section.
            _r.BaseStream.Position = _fileDataSectionPostion;
        }

        private ulong HashFilePath(string filePath)
        {
            if (Magic == MW_BSAHEADER_FILEID) return Tes3HashFilePath(filePath);
            else return Tes4HashFilePath(filePath);
        }

        // http://en.uesp.net/wiki/Tes3Mod:BSA_File_Format
        static ulong Tes3HashFilePath(string filePath)
        {
            filePath = filePath.ToLowerInvariant().Replace('/', '\\');
            var len = (uint)filePath.Length;
            //
            uint l = (len >> 1);
            int off, i;
            uint sum, temp, n;
            for (sum = 0, off = 0, i = 0; i < l; i++)
            {
                sum ^= (uint)(filePath[i]) << (off & 0x1F);
                off += 8;
            }
            var value1 = sum;
            for (sum = 0, off = 0; i < len; i++)
            {
                temp = (uint)(filePath[i]) << (off & 0x1F);
                sum ^= temp;
                n = temp & 0x1F;
                sum = (sum << (32 - (int)n)) | (sum >> (int)n);
                off += 8;
            }
            var value2 = sum;
            return (value1 << 32) | (ulong)value2;
        }

        // http://en.uesp.net/wiki/Tes4Mod:Hash_Calculation
        static ulong Tes4HashFilePath(string filePath)
        {
            filePath = filePath.ToLowerInvariant().Replace('/', '\\');
            return GenHash(Path.ChangeExtension(filePath, null), Path.GetExtension(filePath));

            ulong GenHash(string file, string ext)
            {
                var hash = 0UL;
                if (file.Length > 0)
                {
                    hash = (ulong)(
                        (((byte)file[file.Length - 1]) * 0x1) +
                        ((file.Length > 2 ? (byte)file[file.Length - 2] : (byte)0) * 0x100) +
                        (file.Length * 0x10000) +
                        (((byte)file[0]) * 0x1000000)
                    );
                }
                if (file.Length > 3)
                    hash += (ulong)(GenHash2(file.Substring(1, file.Length - 3)) * 0x100000000);
                if (ext.Length > 0)
                {
                    hash += (ulong)(GenHash2(ext) * 0x100000000);
                    byte i = 0;
                    switch (ext)
                    {
                        case ".nif": i = 1; break;
                        case ".kf": i = 2; break;
                        case ".dds": i = 3; break;
                        case ".wav": i = 4; break;
                    }
                    if (i != 0)
                    {
                        var a = (byte)(((i & 0xfc) << 5) + (byte)((hash & 0xff000000) >> 24));
                        var b = (byte)(((i & 0xfe) << 6) + (byte)(hash & 0xff));
                        var c = (byte)((i << 7) + (byte)((hash & 0xff00) >> 8));
                        hash -= hash & 0xFF00FFFF;
                        hash += (uint)((a << 24) + b + (c << 8));
                    }
                }
                return hash;
            }

            uint GenHash2(string s)
            {
                var hash = 0U;
                for (var i = 0; i < s.Length; i++)
                {
                    hash *= 0x1003f;
                    hash += (byte)s[i];
                }
                return hash;
            }
        }
    }
}