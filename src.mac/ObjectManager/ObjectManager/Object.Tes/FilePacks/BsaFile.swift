//
//  BsaFile.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import Compression

public class BsaFile {
    // MARK:Types

    // Default header data
    static let MW_BSAHEADER_FILEID: UInt32 = 0x00000100 // Magic for Morrowind BSA
    static let OB_BSAHEADER_FILEID: UInt32 = 0x00415342 // Magic for Oblivion BSA, the literal string "BSA\0".
    static let F4_BSAHEADER_FILEID: UInt32 = 0x58445442 // Magic for Fallout 4 BA2, the literal string "BTDX".
    static let OB_BSAHEADER_VERSION: UInt32 = 0x67 // Version number of an Oblivion BSA
    static let F3_BSAHEADER_VERSION: UInt32 = 0x68 // Version number of a Fallout 3 BSA
    static let SSE_BSAHEADER_VERSION: UInt32 = 0x69 // Version number of a Skyrim SE BSA
    static let F4_BSAHEADER_VERSION: UInt32 = 0x01 // Version number of a Fallout 4 BA2

    // Archive flags
    static let OB_BSAARCHIVE_PATHNAMES: UInt16 = 0x0001 // Whether the BSA has names for paths
    static let OB_BSAARCHIVE_FILENAMES: UInt16 = 0x0002 // Whether the BSA has names for files
    static let OB_BSAARCHIVE_COMPRESSFILES: UInt16 = 0x0004 // Whether the files are compressed
    static let F3_BSAARCHIVE_PREFIXFULLFILENAMES: UInt16 = 0x0100 // Whether the name is prefixed to the data?

    // File flags
    static let OB_BSAFILE_NIF: UInt16 = 0x0001 // Set when the BSA contains NIF files
    static let OB_BSAFILE_DDS: UInt16 = 0x0002 // Set when the BSA contains DDS files
    static let OB_BSAFILE_XML: UInt16 = 0x0004 // Set when the BSA contains XML files
    static let OB_BSAFILE_WAV: UInt16 = 0x0008 // Set when the BSA contains WAV files
    static let OB_BSAFILE_MP3: UInt16 = 0x0010 // Set when the BSA contains MP3 files
    static let OB_BSAFILE_TXT: UInt16 = 0x0020 // Set when the BSA contains TXT files
    static let OB_BSAFILE_HTML: UInt16 = 0x0020 // Set when the BSA contains HTML files
    static let OB_BSAFILE_BAT: UInt16 = 0x0020 // Set when the BSA contains BAT files
    static let OB_BSAFILE_SCC: UInt16 = 0x0020 // Set when the BSA contains SCC files
    static let OB_BSAFILE_SPT: UInt16 = 0x0040 // Set when the BSA contains SPT files
    static let OB_BSAFILE_TEX: UInt16 = 0x0080 // Set when the BSA contains TEX files
    static let OB_BSAFILE_FNT: UInt16 = 0x0080 // Set when the BSA contains FNT files
    static let OB_BSAFILE_CTL: UInt16 = 0x0100 // Set when the BSA contains CTL files

    // Bitmasks for the size field in the header
    static let OB_BSAFILE_SIZEMASK: UInt32 = 0x3FFFFFFF // Bit mask with OBBSAFileInfo::sizeFlags to get the size of the file

    // Record flags
    static let OB_BSAFILE_FLAG_COMPRESS: UInt32 = 0xC0000000 // Bit mask with OBBSAFileInfo::sizeFlags to get the compression status

    public class FileMetadata {
        // Skyrim and earlier
        public let sizeFlags: UInt32 // The size of the file in the BSA
        // Fallout 4
        public let packedSize: UInt32
        public let unpackedSize: UInt32
        //
        public let offset: Int64 // The offset of the file in the BSA
        public let tex: F4Tex
        //
        public let path: String
        public let pathHash: UInt64
        public func size() -> UInt32 { return 0 }
        public var compressed: Bool { return (sizeFlags & OB_BSAFILE_FLAG_COMPRESS) != 0 }
        
        init() {
        }
    }

    public struct DXGIFormat {
    }
    
    public struct F4Tex {
        public let height: UInt16
        public let width: UInt16
        public let numMips: UInt8
        public let format: DXGIFormat
        public let unk16: UInt16
        public let chunks: [F4TexChunk]
    }

    public struct F4TexChunk {
        public let offset: UInt64
        public let packedSize: UInt32
        public let unpackedSize: UInt32
        public let startMip: UInt16
        public let endMip: UInt16
        public let unk14: UInt32
    }

    // MARK:Code

    let _r: BinaryReader
    public let magic: UInt32 // 4 bytes
    public let version: UInt32 // 4 bytes
    let _compressToggle: Bool // Whether the %BSA is compressed
    let _hasNamePrefix: Bool // Whether Fallout 3 names are prefixed with an extra string
    let _files: [FileMetadata]
    let _filesByHash: [UInt64 : [FileMetadata]]
    public let filePath: String
    //public let RootDir: VirtualFileSystem.Directory

    public var isAtEof: Bool { return _r.offsetInFileo >= _r.Length }

    init(filePath: String?) {
        guard let filePath = filePath else {
            return
        }
        self.filePath = filePath
        _r = BinaryReader(stream: FileHandle(forReadingAtPath: filePath))
        readMetadata()
        //testContainsFile()
        //testLoadFileData()
    }

    deinit {
        close()
    }

    public func close() {
        _r?.close()
        _r = null
    }

    public func containsFile(filePath: String) -> Bool {
        return _filesByHash.contains(hashFilePath(filePath))
    }

    public func loadFileData(filePath: String) -> Data {
        var files = _filesByHash[hashFilePath(filePath)]
        guard !files.isEmpty else {
            fatalError("should not happen")
        }
        if files.count == 1 {
            return loadFileData(files[0])
        }
        guard let file = files.first { $0.path.caseInsensitiveCompare(newPath) == ComparisonResult.orderedSame } else {
            fatalError("Could not find file '\(filePath)' in a BSA file.")
        }
        return loadFileData(file)
    }

    public func loadFileData(file: FileMetadata) -> Data {
        _r.baseStream.offsetInFile = file.offset
        var fileSize = Int32(file.size)
        if _hasNamePrefix {
            let len = _r.readByte()
            fileSize -= len + 1
            _r.baseStream.position = file.offset + 1 + len
        }
        var newFileSize = fileSize;
        if Version == SSE_BSAHEADER_VERSION && file.sizeFlags > 0 && file.compressed ^ _compressToggle
            newFileSize = _r.readLEInt32() - 4;
        let fileData = _r.readBytes(fileSize);
        // BSA
        if file.sizeFlags > 0 && file.compressed ^ _compressToggle {
             var newFileData: Data
             if Version != SSE_BSAHEADER_VERSION {
                if fileData.count > 4 {
                    newFileData = fileData.inflate(withOffset: 4)
                }
            }
            else {
                newFileData = fileData.lzmaDecompress()
            }
            fileData = newFileData;
        }
        // General BA2
        else if file.packedSize > 0 && file.tex.chunks == nil {
            let newFileData = fileData.inflate()
            fileData = newFileData
        }
        // Fill DDS Header
        else if file.tex.chunks != nil {
            // // Fill DDS Header
            // var ddsHeader = new DDSHeader
            // {
            //     dwFlags = DDSFlags.HEADER_FLAGS_TEXTURE | DDSFlags.HEADER_FLAGS_LINEARSIZE | DDSFlags.HEADER_FLAGS_MIPMAP,
            //     dwHeight = file.Tex.Height,
            //     dwWidth = file.Tex.Width,
            //     dwMipMapCount = file.Tex.NumMips,
            //     dwCaps = DDSCaps.SURFACE_FLAGS_TEXTURE | DDSCaps.SURFACE_FLAGS_MIPMAP,
            //     dwCaps2 = file.Tex.Unk16 == 2049 ? DDSCaps2.CUBEMAP_ALLFACES : 0,
            // };
            // var dx10Header = new DDSHeader_DXT10();
            // var dx10 = false;

            // // map tex format
            // switch (file.Tex.Format)
            // {
            //     case DXGIFormat.BC1_UNORM:
            //         ddsHeader.ddspf.dwFlags = DDSPixelFormats.FourCC;
            //         ddsHeader.ddspf.dwFourCC = Encoding.ASCII.GetBytes("DXT1");
            //         ddsHeader.dwPitchOrLinearSize = (uint)file.Tex.Width * file.Tex.Height / 2U; // 4bpp
            //         break;
            //     case DXGIFormat.BC2_UNORM:
            //         ddsHeader.ddspf.dwFlags = DDSPixelFormats.FourCC;
            //         ddsHeader.ddspf.dwFourCC = Encoding.ASCII.GetBytes("DXT3");
            //         ddsHeader.dwPitchOrLinearSize = (uint)file.Tex.Width * file.Tex.Height; // 8bpp
            //         break;
            //     case DXGIFormat.BC3_UNORM:
            //         ddsHeader.ddspf.dwFlags = DDSPixelFormats.FourCC;
            //         ddsHeader.ddspf.dwFourCC = Encoding.ASCII.GetBytes("DXT5");
            //         ddsHeader.dwPitchOrLinearSize = (uint)file.Tex.Width * file.Tex.Height; // 8bpp
            //         break;
            //     case DXGIFormat.BC5_UNORM:
            //         ddsHeader.ddspf.dwFlags = DDSPixelFormats.FourCC;
            //         ddsHeader.ddspf.dwFourCC = Encoding.ASCII.GetBytes("ATI2");
            //         ddsHeader.dwPitchOrLinearSize = (uint)file.Tex.Width * file.Tex.Height; // 8bpp
            //         break;
            //     case DXGIFormat.BC7_UNORM:
            //         ddsHeader.ddspf.dwFlags = DDSPixelFormats.FourCC;
            //         ddsHeader.ddspf.dwFourCC = Encoding.ASCII.GetBytes("DX10");
            //         ddsHeader.dwPitchOrLinearSize = (uint)file.Tex.Width * file.Tex.Height; // 8bpp
            //         dx10 = true;
            //         dx10Header.dxgiFormat = (int)DXGIFormat.BC7_UNORM;
            //         break;
            //     case DXGIFormat.DXGI_FORMAT_B8G8R8A8_UNORM:
            //         ddsHeader.ddspf.dwFlags = DDSPixelFormats.RGB | DDSPixelFormats.AlphaPixels;
            //         ddsHeader.ddspf.dwRGBBitCount = 32;
            //         ddsHeader.ddspf.dwRBitMask = 0x00FF0000;
            //         ddsHeader.ddspf.dwGBitMask = 0x0000FF00;
            //         ddsHeader.ddspf.dwBBitMask = 0x000000FF;
            //         ddsHeader.ddspf.dwABitMask = 0xFF000000;
            //         ddsHeader.dwPitchOrLinearSize = (uint)file.Tex.Width * file.Tex.Height * 4; // 32bpp
            //         break;
            //     case DXGIFormat.DXGI_FORMAT_R8_UNORM:
            //         ddsHeader.ddspf.dwFlags = DDSPixelFormats.RGB;
            //         ddsHeader.ddspf.dwRGBBitCount = 8;
            //         ddsHeader.ddspf.dwRBitMask = 0xFF;
            //         ddsHeader.dwPitchOrLinearSize = (uint)file.Tex.Width * file.Tex.Height; // 8bpp
            //         break;
            //     default: throw new InvalidOperationException("DDS FAILED");
            // }
            // if (dx10)
            // {
            //     dx10Header.resourceDimension = DDSDimension.Texture2D;
            //     dx10Header.miscFlag = 0;
            //     dx10Header.arraySize = 1;
            //     dx10Header.miscFlags2 = 0;
            //     dx10Header.Write(null);
            //     //char dds2[sizeof(dx10Header)];
            //     //memcpy(dds2, &dx10Header, sizeof(dx10Header));
            //     //content.append(QByteArray::fromRawData(dds2, sizeof(dx10Header)));
            // }
        }
        return fileData;
    }

    func readMetadata() {
        // Open
        magic = _r.readLEUInt32()
        if magic == F4_BSAHEADER_FILEID {
            version = _r.readLEUInt32()
            if version != F4_BSAHEADER_VERSION
                fatalError("BAD MAGIC")
            // Read the header
            let header_type = _r.readASCIIString(4)            // 08 GNRL=General, DX10=Textures
            let header_numFiles = _r.readLEUInt32()            // 0C
            let header_nameTableOffset = _r.readLEUInt64()     // 10 - relative to start of file

            // Create file metadatas
            _r.baseStream.offsetInFile = UInt64(header_nameTableOffset)
            _files = [FileMetadata]() ; _files.reserveCapacity(header_numFiles)
            for i in 0..<header_NumFiles {
                let length = _r.readLEUInt16()
                let path = _r.readASCIIString(length)
                _files[i] = FileMetadata()
                {
                    path = path,
                    pathHash = tes4HashFilePath(path),
                }
            }
            if header_Type == "GNRL" { // General BA2 Format
                _r.baseStream.offsetInFile = 16 + 8 // sizeof(header) + 8
                for i in 0..<header_numFiles {
                    let info_nameHash = _r.readLEUInt32()      // 00
                    let info_ext = _r.readASCIIString(4)       // 04 - extension
                    let info_dirHash = _r.readLEUInt32()       // 08
                    let info_unk0C = _r.readLEUInt32()         // 0C - flags? 00100100
                    let info_offset = _r.readLEUInt64()        // 10 - relative to start of file
                    let info_packedSize = _r.readLEUInt32()    // 18 - packed length (zlib)
                    let info_unpackedSize = _r.readLEUInt32()  // 1C - unpacked length
                    let info_unk20 = _r.readLEUInt32()         // 20 - BAADF00D
                    _files[i].packedSize = info_packedSize
                    _files[i].unpackedSize = info_unpackedSize
                    _files[i].offset = Int64(info_offset)
                }
            }
            else if header_Type == "DX10" { // Texture BA2 Format
                _r.baseStream.offsetInFile = 16 + 8 // sizeof(header) + 8
                for i in 0..<header_numFiles {
                    let fileMetadata = _files[i]
                    let info_nameHash = _r.readLEUInt32()      // 00
                    let info_ext = _r.readASCIIString(4)       // 04
                    let info_dirHash = _r.readLEUInt32()       // 08
                    let info_unk0C = _r.readByte()             // 0C
                    let info_numChunks = _r.readByte()         // 0D
                    let info_chunkHeaderSize = _r.readLEUInt16()// 0E - size of one chunk header
                    let info_height = _r.readLEUInt16()        // 10
                    let info_width = _r.readLEUInt16()         // 12
                    let info_numMips = _r.readByte()           // 14
                    let info_format = _r.readByte()            // 15 - DXGI_FORMAT
                    let info_unk16 = _r.readLEUInt16()         // 16 - 0800
                    // read tex-chunks
                    var texChunks = [F4TexChunk]() ; texChunks.reserveCapacity(info_numChunks)
                    for j in 0..<info_numChunks {
                        texChunks[j] = F4TexChunk() {
                            offset = _r.readLEUInt64(),         // 00
                            packedSize = _r.readLEUInt32(),     // 08
                            unpackedSize = _r.readLEUInt32(),   // 0C
                            startMip = _r.readLEUInt16(),       // 10
                            endMip = _r.readLEUInt16(),         // 12
                            unk14 = _r.readLEUInt32(),          // 14 - BAADFOOD
                        }
                    }
                    let firstChunk = texChunks.first
                    _files[i].PackedSize = firstChunk.packedSize
                    _files[i].UnpackedSize = firstChunk.unpackedSize
                    _files[i].Offset = Int64(firstChunk.offset)
                    fileMetadata.tex = F4Tex() {
                        height = info_Height,
                        width = info_Width,
                        numMips = info_numMips,
                        format = (DXGIFormat)info_format,
                        unk16 = info_unk16,
                        chunks = texChunks,
                    }
                }
            }
        }
        else if magic == OB_BSAHEADER_FILEID {
            version = _r.readUInt32()
            if version != OB_BSAHEADER_VERSION && version != F3_BSAHEADER_VERSION && version != SSE_BSAHEADER_VERSION
                fatalError("BAD MAGIC")
            // Read the header
            let header_folderRecordOffset = _r.readLEUInt32() // Offset of beginning of folder records
            let header_archiveFlags = _r.readLEUInt32() // Archive flags
            let header_folderCount = _r.readLEUInt32() // Total number of folder records (OBBSAFolderInfo)
            let header_fileCount = _r.readLEUInt32() // Total number of file records (OBBSAFileInfo)
            let header_folderNameLength = _r.readLEUInt32() // Total length of folder names
            let header_fileNameLength = _r.readLEUInt32() // Total length of file names
            let header_fileFlags = _r.readLEUInt32() // File flags

            // Calculate some useful values
            if (header_archiveFlags & OB_BSAARCHIVE_PATHNAMES) == 0 || (header_archiveFlags & OB_BSAARCHIVE_FILENAMES) == 0
                fatalError("HEADER FLAGS")
            _compressToggle = (header_archiveFlags & OB_BSAARCHIVE_COMPRESSFILES) != 0
            if version == F3_BSAHEADER_VERSION || version == SSE_BSAHEADER_VERSION
                _hasNamePrefix = (header_archiveFlags & F3_BSAARCHIVE_PREFIXFULLFILENAMES) != 0
            let folderSize = version != SSE_BSAHEADER_VERSION ? 16 : 24

            // Create file metadatas
            _files = [FileMetadata]() ; _files.reserveCapacity(header_fileCount)
            var filenamesSectionStartPos = _r.baseStream.offsetInFile = header_folderRecordOffset + header_folderNameLength + header_folderCount * (folderSize + 1) + header_fileCount * 16
            var buf = Data() ; buf.reserveCapacity(64)
            for i in 0..<header_fileCount {
                buf.clear()
                curCharAsByte: UInt8; while (curCharAsByte = _r.readByte()) != 0 {
                    buf.add(curCharAsByte)
                }
                let path = String(data: buf, encoding: .utf8)
                _files[i] = FileMetadata()
                {
                    Path = path,
                }
            }
            if _r.baseStream.offsetInFile != filenamesSectionStartPos + header_fileNameLength
                fatalError("HEADER FILENAMES")

            // read-all folders
            _r.baseStream.offsetInFile = header_folderRecordOffset;
            var foldersFiles = [UInt32]() ; foldersFiles.reserveCapacity(header_folderCount)
            for i in 0..<header_folderCount {
                var folder_hash = _r.readLEUInt64() // Hash of the folder name
                var folder_fileCount = _r.readLEUInt32() // Number of files in folder
                var folder_unk = 0U ; var folder_offset = 0UL
                if version == SSE_BSAHEADER_VERSION { folder_unk = _r.readLEUInt32() ; folder_offset = _r.readLEUInt64() }
                else { folder_offset = _r.readLEUInt32() }
                foldersFiles[i] = folder_fileCount
            }

            // add file
            var fileNameIndex = 0U
            for i in 0..<header_folderCount {
                let folder_name = _r.readASCIIString(_r.readByte(), ASCIIFormat.possiblyNullTerminated) // BSAReadSizedString
                var folderFiles = foldersFiles[i]
                for j in 0..<folderFiles {
                    let file_hash = _r.readLEUInt64() // Hash of the filename
                    let file_sizeFlags = _r.readLEUInt32() // Size of the data, possibly with OB_BSAFILE_FLAG_COMPRESS set
                    let file_offset = _r.readLEUInt32() // Offset to raw file data
                    let fileMetadata = _files[fileNameIndex++]
                    fileMetadata.sizeFlags = file_sizeFlags
                    fileMetadata.offset = file_offset
                    var path = folder_name + "\\" + fileMetadata.path
                    fileMetadata.path = path
                    fileMetadata.pathHash = tes4HashFilePath(path)
                }
            }
        }
        else if magic == MW_BSAHEADER_FILEID {
            // Read the header
            let header_hashOffset = _r.readLEUInt32() // Offset of hash table minus header size (12)
            let header_fileCount = _r.readLEUInt32() // Number of files in the archive

            // Calculate some useful values
            let headerSize = _r.baseStream.offsetInFile;
            let hashTablePosition = headerSize + header_hashOffset;
            let fileDataSectionPostion = hashTablePosition + (8 * header_fileCount)

            // Create file metadatas
            _files = [FileMetadata]() ; _files.reserveCapacity(header_fileCount)
            for i in 0..<header_fileCount {
                _files[i] = FileMetadata()
                {
                    // Read file sizes/offsets
                    sizeFlags = _r.readLEUInt32(),
                    offset = fileDataSectionPostion + _r.readLEUInt32(),
                }
            }

            // Read filename offsets
            var filenameOffsets = [UInt32]() ; filenameOffsets.reserveCapacity(header_FileCount) // relative offset in filenames section
            for i in 0..<header_fileCount {
                filenameOffsets[i] = _r.readLEUInt32()
            }

            // Read filenames
            var filenamesSectionStartPos = _r.baseStream.offsetInFile;
            var buf = Data() ; buf.reserveCapacity(64)
            for i in 0..<header_fileCount {
                _r.baseStream.offsetInFile = filenamesSectionStartPos + filenameOffsets[i];
                buf.clear()
                curCharAsByte : UInt8 ; while (curCharAsByte = _r.readByte()) != 0 {
                    buf.Add(curCharAsByte)
                }
                _files[i].path = String(data: buf, encoding: .utf8)
            }

            // Read filename hashes
            _r.baseStream.offsetInFile = hashTablePosition;
            for i in 0..<header_fileCount
                _files[i].pathHash = UInt64((_r.readLEUInt32() << 32) | _r.readLEUInt32())
        }
        else { fatalError("BAD MAGIC") }

        // Create the file metadata hash table
        //_filesByHash = _files.ToLookup(x => x.PathHash);

        // // Create a virtual directory tree.
        // RootDir = new VirtualFileSystem.Directory();
        // foreach (var fileMetadata in _files)
        //     RootDir.CreateDescendantFile(fileMetadata.Path);
    }

    func hashFilePath(string filePath: String) -> UInt64 {
        if Magic == MW_BSAHEADER_FILEID { return tes3HashFilePath(filePath) }
        else { return tes4HashFilePath(filePath) }
    }

    // http://en.uesp.net/wiki/Tes3Mod:BSA_File_Format
    static func tes3HashFilePath(filePath: String) -> UInt64 {
        filePath = filePath.lowered().replacingOccurrences(of: "/", with: "\\")
        let len = UInt32(filePath.size)
        //
        var l: UInt32 = (len >> 1)
        var off: Int = 0, i: Int = 0
        var sum: UInt32 = 0, temp: UInt32
        for i in 0..<l {
            sum ^= UInt32(filePath[i]) << (off & 0x1F)
            off += 8
        }
        let value1 = sum
        sum = 0, off = 0
        Int n
        for i in i..len {
            temp = UInt32(filePath[i]) << (off & 0x1F)
            sum ^= temp
            n = temp & 0x1F
            sum = (sum << (32 - n)) | (sum >> n)
            off += 8;
        }
        let value2 = sum
        return UInt64((value1 << 32) | value2)
    }

    static func tes4HashFilePath(filePath: String) -> UInt64 {
        filePath = filePath.lowered().replacingOccurrences(of: "/", with: "\\")
        let url = URL(filePath)
        return genHash(url.deletingPathExtension().path, url.pathExtension)

        func genHash(file: String, ext: String) -> UInt64 {
            var hash: UInt64 = 0UL
            if file.size > 0 {
                hash = UInt64(
                    (((byte)file[file.Length - 1]) * 0x1) +
                    ((file.Length > 2 ? (byte)file[file.Length - 2] : (byte)0) * 0x100) +
                    (file.Length * 0x10000) +
                    (((byte)file[0]) * 0x1000000)
                )
            }
            if file.size > 3 {
                hash += UInt64(genHash2(file.Substring(1, file.Length - 3)) * 0x100000000)
            }
            if ext.size > 0 {
                hash += UInt64(genHash2(ext) * 0x100000000)
                var i: UInt8 = 0
                switch ext {
                    case ".nif": i = 1
                    case ".kf": i = 2
                    case ".dds": i = 3
                    case ".wav": i = 4
                }
                if i != 0 {
                    let a: UInt8 = ((i & 0xfc) << 5) + UInt8((hash & 0xff000000) >> 24)
                    let b: UInt8 = ((i & 0xfe) << 6) + UInt8(hash & 0xff)
                    let c: UInt8 = (i << 7) + UInt8((hash & 0xff00) >> 8)
                    hash -= hash & 0xFF00FFFF
                    hash += UInt32((a << 24) + b + (c << 8))
                }
            }
            return hash
        }

        func genHash2(s: String) -> UInt32 {
            var hash: UInt32 = 0
            for i in s.startIndex..<s.endIndex {
                hash *= 0x1003f
                hash += UInt8(s[i])
            }
            return hash
        }
    }
}
