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
        public var sizeFlags: UInt32 // The size of the file in the BSA
        // Fallout 4
        public var packedSize: UInt32 = 0
        public var unpackedSize: UInt32 = 0
        //
        public var offset: UInt64 // The offset of the file in the BSA
        public var tex: F4Tex? = nil
        //
        public var path: String
        public var pathHash: UInt64
        // The size of the file inside the BSA
        public var size: UInt32 { return sizeFlags > 0 ?
            // Skyrim and earlier
            sizeFlags & BsaFile.OB_BSAFILE_SIZEMASK :
            //TODO: Not correct for texture BA2s
            packedSize == 0 ? unpackedSize : packedSize }
        // Whether the file is compressed inside the BSA
        public var compressed: UInt16 { return UInt16(sizeFlags & OB_BSAFILE_FLAG_COMPRESS) }
        
        init(sizeFlags: UInt32 = 0,
             offset: UInt64 = 0,
             path: String = "",
             pathHash: UInt64 = 0) {
            self.sizeFlags = sizeFlags
            self.offset = offset
            self.path = path
            self.pathHash = pathHash
        }
    }
    
    public struct F4Tex {
        public let height: UInt16
        public let width: UInt16
        public let numMips: UInt8
        public let format: DXGIFormat
        public let unk16: UInt16
        public let chunks: [F4TexChunk]?
        
        init(height: UInt16,
             width: UInt16,
             numMips: UInt8,
             format: DXGIFormat,
             unk16: UInt16,
             chunks: [F4TexChunk]?) {
            self.height = height
            self.width = width
            self.numMips = numMips
            self.format = format
            self.unk16 = unk16
            self.chunks = chunks
        }
    }

    public struct F4TexChunk {
        public let offset: UInt64
        public let packedSize: UInt32
        public let unpackedSize: UInt32
        public let startMip: UInt16
        public let endMip: UInt16
        public let unk14: UInt32
        
        init(offset: UInt64,
             packedSize: UInt32,
             unpackedSize: UInt32,
             startMip: UInt16,
             endMip: UInt16,
             unk14: UInt32) {
            self.offset = offset
            self.packedSize = packedSize
            self.unpackedSize = unpackedSize
            self.startMip = startMip
            self.endMip = endMip
            self.unk14 = unk14
        }
    }

    // MARK:Code

    var _r: BinaryReader!
    public var magic: UInt32 = 0 // 4 bytes
    public var version: UInt32 = 0// 4 bytes
    var _compressToggle: UInt16 = 0// Whether the %BSA is compressed
    var _hasNamePrefix = false// Whether Fallout 3 names are prefixed with an extra string
    var _files = [FileMetadata]()
    var _filesByHash = [UInt64 : [FileMetadata]]()
    public let filePath: String
    //public let RootDir: VirtualFileSystem.Directory

    //public var isAtEof: Bool { return _r.baseStream.offsetInFile >= _r.baseStream.count }

    init(_ filePath: String?) {
        guard let filePath = filePath else {
            fatalError("filepath is nil")
        }
        self.filePath = filePath
        _r = BinaryReader(FileBaseStream(path: filePath)!)
        debugPrint("read")
        readMetadata()
        testContainsFile()
        //testLoadFileData()
    }

    deinit {
        close()
    }

    public func close() {
        _r?.close()
        _r = nil
    }

    public func containsFile(_ filePath: String) -> Bool {
        return _filesByHash[hashFilePath(filePath)] != nil
    }

    public func loadFileData(filePath: String) -> Data {
        guard let files = _filesByHash[hashFilePath(filePath)], !files.isEmpty else {
            fatalError("should not happen")
        }
        if files.count == 1 {
            return loadFileData(file: files[0])
        }
        let newPath = filePath.replacingOccurrences(of: "/", with: "\\")
        guard let file = files.first(where: { $0.path.caseInsensitiveCompare(newPath) == .orderedSame }) else {
            fatalError("Could not find file '\(filePath)' in a BSA file.")
        }
        return loadFileData(file: file)
    }

    public func loadFileData(file: FileMetadata) -> Data {
        _r.baseStream.position = file.offset
        var fileSize = Int(file.size)
        if _hasNamePrefix {
            let len = Int(_r.readByte() + 1)
            fileSize -= len
            _r.baseStream.position = file.offset + UInt64(len)
        }
        var newFileSize = fileSize
        if version == BsaFile.SSE_BSAHEADER_VERSION && file.sizeFlags > 0 && (file.compressed ^ _compressToggle) != 0 {
            newFileSize = Int(_r.readLEInt32()) - 4
        }
        var fileData = _r.readBytes(fileSize)
        // BSA
        if file.sizeFlags > 0 && (file.compressed ^ _compressToggle) != 0 {
             var newFileData: Data
             if version != BsaFile.SSE_BSAHEADER_VERSION {
                newFileData = fileData.count > 4 ? fileData.inflate(withOffset: 4)! : Data()
            }
            else {
                newFileData = fileData.lzmaDecompress()!
            }
            fileData = newFileData
        }
        // General BA2
        else if file.packedSize > 0 && file.tex!.chunks == nil {
            fileData = fileData.inflate()!
        }
        // Fill DDS Header
        else if file.tex!.chunks != nil {
            // map tex format
            let ddspf: DDSPixelFormat
            let dwPitchOrLinearSize: UInt32
            switch file.tex!.format {
            case .BC1_UNORM:
                ddspf = DDSPixelFormat(dwFlags: .fourCC, dwFourCC: "DXT1")
                dwPitchOrLinearSize = UInt32(file.tex!.width * file.tex!.height / 2) // 4bpp
            case .BC2_UNORM:
                ddspf = DDSPixelFormat(dwFlags: .fourCC, dwFourCC: "DXT3")
                dwPitchOrLinearSize = UInt32(file.tex!.width * file.tex!.height) // 8bpp
            case .BC3_UNORM:
                ddspf = DDSPixelFormat(dwFlags: .fourCC, dwFourCC: "DXT5")
                dwPitchOrLinearSize = UInt32(file.tex!.width * file.tex!.height) // 8bpp
            case DXGIFormat.BC5_UNORM:
                ddspf = DDSPixelFormat(dwFlags: .fourCC, dwFourCC: "ATI2")
                dwPitchOrLinearSize = UInt32(file.tex!.width * file.tex!.height) // 8bpp
            case .BC7_UNORM:
                ddspf = DDSPixelFormat(dwFlags: .fourCC, dwFourCC: "DX10")
                dwPitchOrLinearSize = UInt32(file.tex!.width * file.tex!.height) // 8bpp
                _ = DDSHeader_DXT10(dxgiFormat: Int32(DXGIFormat.BC7_UNORM.rawValue), resourceDimension: .texture2D)
                //dx10Header.write(nil)
            case .DXGI_FORMAT_B8G8R8A8_UNORM:
                ddspf = DDSPixelFormat(
                    dwFlags: [DDSPixelFormats.rgb, DDSPixelFormats.alphaPixels], dwRGBBitCount: UInt32(32),
                    dwRBitMask: UInt32(0x00FF0000),
                    dwGBitMask: UInt32(0x0000FF00),
                    dwBBitMask: UInt32(0x000000FF),
                    dwABitMask: UInt32(0xFF000000))
                dwPitchOrLinearSize = UInt32(file.tex!.width * file.tex!.height * 4) // 32bpp
            case .DXGI_FORMAT_R8_UNORM:
                ddspf = DDSPixelFormat(
                    dwFlags: DDSPixelFormats.rgb, dwRGBBitCount: UInt32(8),
                    dwRBitMask: UInt32(0xFF))
                dwPitchOrLinearSize = UInt32(file.tex!.width * file.tex!.height) // 8bpp
            default: fatalError("DDS FAILED")
            }
            
            // Fill DDS Header
            _ = DDSHeader(
                dwFlags: [.HEADER_FLAGS_TEXTURE, .HEADER_FLAGS_LINEARSIZE, .HEADER_FLAGS_MIPMAP],
                dwHeight: UInt32(file.tex!.height), dwWidth: UInt32(file.tex!.width),
                dwPitchOrLinearSize: dwPitchOrLinearSize,
                dwMipMapCount: UInt32(file.tex!.numMips),
                ddspf: ddspf,
                dwCaps: [.SURFACE_FLAGS_TEXTURE, .SURFACE_FLAGS_MIPMAP],
                dwCaps2: file.tex!.unk16 == 2049 ? DDSCaps2.CUBEMAP_ALLFACES : DDSCaps2.none)
        }
        return fileData
    }

    func readMetadata() {
        // Open
        magic = _r.readLEUInt32()
        if magic == BsaFile.F4_BSAHEADER_FILEID {
            version = _r.readLEUInt32()
            if version != BsaFile.F4_BSAHEADER_VERSION {
                fatalError("BAD MAGIC")
            }
            // Read the header
            let header_type = _r.readASCIIString(4)            // 08 GNRL=General, DX10=Textures
            let header_numFiles = Int(_r.readLEUInt32())       // 0C
            let header_nameTableOffset = _r.readLEUInt64()     // 10 - relative to start of file

            // Create file metadatas
            _r.baseStream.position = UInt64(header_nameTableOffset)
            _files = [FileMetadata](); _files.reserveCapacity(Int(header_numFiles))
            for i in 0..<header_numFiles {
                let length = Int(_r.readLEUInt16())
                let path = _r.readASCIIString(length)
                _files[i] = FileMetadata(
                    path: path,
                    pathHash: BsaFile.tes4HashFilePath(path)
                )
            }
            if header_type == "GNRL" { // General BA2 Format
                _r.baseStream.position = 16 + 8 // sizeof(header) + 8
                for i in 0..<header_numFiles {
                    _ = _r.readLEUInt32()      // 00
                    _ = _r.readASCIIString(4)       // 04 - extension
                    _ = _r.readLEUInt32()       // 08
                    _ = _r.readLEUInt32()         // 0C - flags? 00100100
                    let info_offset = _r.readLEUInt64()        // 10 - relative to start of file
                    let info_packedSize = _r.readLEUInt32()    // 18 - packed length (zlib)
                    let info_unpackedSize = _r.readLEUInt32()  // 1C - unpacked length
                    _ = _r.readLEUInt32()         // 20 - BAADF00D
                    _files[i].packedSize = info_packedSize
                    _files[i].unpackedSize = info_unpackedSize
                    _files[i].offset = info_offset
                }
            }
            else if header_type == "DX10" { // Texture BA2 Format
                _r.baseStream.position = 16 + 8 // sizeof(header) + 8
                for i in 0..<header_numFiles {
                    let fileMetadata = _files[i]
                    _ = _r.readLEUInt32()      // 00
                    _ = _r.readASCIIString(4)       // 04
                    _ = _r.readLEUInt32()       // 08
                    _ = _r.readByte()             // 0C
                    let info_numChunks = Int(_r.readByte())    // 0D
                    _ = _r.readLEUInt16()// 0E - size of one chunk header
                    let info_height = _r.readLEUInt16()        // 10
                    let info_width = _r.readLEUInt16()         // 12
                    let info_numMips = _r.readByte()           // 14
                    let info_format = _r.readByte()            // 15 - DXGI_FORMAT
                    let info_unk16 = _r.readLEUInt16()         // 16 - 0800
                    // read tex-chunks
                    var texChunks = [F4TexChunk](); texChunks.reserveCapacity(info_numChunks)
                    for j in 0..<info_numChunks {
                        texChunks[j] = F4TexChunk(
                            offset: _r.readLEUInt64(),         // 00
                            packedSize: _r.readLEUInt32(),     // 08
                            unpackedSize: _r.readLEUInt32(),   // 0C
                            startMip: _r.readLEUInt16(),       // 10
                            endMip: _r.readLEUInt16(),         // 12
                            unk14: _r.readLEUInt32()          // 14 - BAADFOOD
                        )
                    }
                    let firstChunk = texChunks.first!
                    _files[i].packedSize = firstChunk.packedSize
                    _files[i].unpackedSize = firstChunk.unpackedSize
                    _files[i].offset = firstChunk.offset
                    fileMetadata.tex = F4Tex(
                        height: info_height,
                        width: info_width,
                        numMips: info_numMips,
                        format: DXGIFormat(rawValue: info_format)!,
                        unk16: info_unk16,
                        chunks: texChunks
                    )
                }
            }
        }
        else if magic == BsaFile.OB_BSAHEADER_FILEID {
            version = _r.readLEUInt32()
            if version != BsaFile.OB_BSAHEADER_VERSION && version != BsaFile.F3_BSAHEADER_VERSION && version != BsaFile.SSE_BSAHEADER_VERSION {
                fatalError("BAD MAGIC")
            }
            // Read the header
            let header_folderRecordOffset = _r.readLEUInt32() // Offset of beginning of folder records
            let header_archiveFlags = UInt16(_r.readLEUInt32()) // Archive flags
            let header_folderCount = Int(_r.readLEUInt32()) // Total number of folder records (OBBSAFolderInfo)
            let header_fileCount = Int(_r.readLEUInt32()) // Total number of file records (OBBSAFileInfo)
            let header_folderNameLength = Int(_r.readLEUInt32()) // Total length of folder names
            let header_fileNameLength = _r.readLEUInt32() // Total length of file names
            _ = _r.readLEUInt32() // File flags

            // Calculate some useful values
            if (header_archiveFlags & BsaFile.OB_BSAARCHIVE_PATHNAMES) == 0 || (header_archiveFlags & BsaFile.OB_BSAARCHIVE_FILENAMES) == 0 {
                fatalError("HEADER FLAGS")
            }
            _compressToggle = (header_archiveFlags & BsaFile.OB_BSAARCHIVE_COMPRESSFILES)
            if version == BsaFile.F3_BSAHEADER_VERSION || version == BsaFile.SSE_BSAHEADER_VERSION {
                _hasNamePrefix = (header_archiveFlags & BsaFile.F3_BSAARCHIVE_PREFIXFULLFILENAMES) != 0
            }
            let folderSize = version != BsaFile.SSE_BSAHEADER_VERSION ? 16 : 24

            // Create file metadatas
            _files = [FileMetadata](); _files.reserveCapacity(header_fileCount)
            let filenamesSectionStartPos = UInt64(header_folderRecordOffset) +
                UInt64(header_folderNameLength + header_folderCount * (folderSize + 1) + header_fileCount * 16)
            _r.baseStream.position = filenamesSectionStartPos
            var buf = Data(); buf.reserveCapacity(64)
            for i in 0..<header_fileCount {
                buf.removeAll(keepingCapacity: true)
                var curCharAsByte: UInt8 = _r.readByte()
                while curCharAsByte != 0 {
                    buf.append(curCharAsByte)
                    curCharAsByte = _r.readByte()
                }
                let path = String(data: buf, encoding: .utf8)!
                _files[i] = FileMetadata(
                    path: path
                )
            }
            if _r.baseStream.position != filenamesSectionStartPos + UInt64(header_fileNameLength) {
                fatalError("HEADER FILENAMES")
            }

            // read-all folders
            _r.baseStream.position = UInt64(header_folderRecordOffset)
            var foldersFiles = [Int](); foldersFiles.reserveCapacity(header_folderCount)
            for i in 0..<header_folderCount {
                _ = _r.readLEUInt64() // Hash of the folder name
                let folder_fileCount = Int(_r.readLEUInt32()) // Number of files in folder
                var folder_unk: UInt32 = 0; var folder_offset: UInt64 = 0
                if version == BsaFile.SSE_BSAHEADER_VERSION { folder_unk = _r.readLEUInt32(); folder_offset = _r.readLEUInt64() }
                else { folder_offset = UInt64(_r.readLEUInt32()) }
                foldersFiles[i] = folder_fileCount
            }

            // add file
            var fileNameIndex = 0
            for i in 0..<header_folderCount {
                let folder_name = _r.readASCIIString(Int(_r.readByte()), format: .possibleNullTerminated) // BSAReadSizedString
                let folderFiles = foldersFiles[i]
                for _ in 0..<folderFiles {
                    _ = _r.readLEUInt64() // Hash of the filename
                    let file_sizeFlags = _r.readLEUInt32() // Size of the data, possibly with OB_BSAFILE_FLAG_COMPRESS set
                    let file_offset = _r.readLEUInt32() // Offset to raw file data
                    let fileMetadata = _files[fileNameIndex]; fileNameIndex += 1
                    fileMetadata.sizeFlags = file_sizeFlags
                    fileMetadata.offset = UInt64(file_offset)
                    let path = folder_name + "\\" + fileMetadata.path
                    fileMetadata.path = path
                    fileMetadata.pathHash = BsaFile.tes4HashFilePath(path)
                }
            }
        }
        else if magic == BsaFile.MW_BSAHEADER_FILEID {
            // Read the header
            let header_hashOffset = _r.readLEUInt32() // Offset of hash table minus header size (12)
            let header_fileCount = Int(_r.readLEUInt32()) // Number of files in the archive

            // Calculate some useful values
            let headerSize = _r.baseStream.position
            let hashTablePosition = headerSize + UInt64(header_hashOffset)
            let fileDataSectionPostion = hashTablePosition + UInt64(8 * header_fileCount)

            // Create file metadatas
            _files = [FileMetadata](); _files.reserveCapacity(header_fileCount)
            for i in 0..<header_fileCount {
                _files[i] = FileMetadata(
                    // Read file sizes/offsets
                    sizeFlags: _r.readLEUInt32(),
                    offset: fileDataSectionPostion + UInt64(_r.readLEUInt32())
                )
            }

            // Read filename offsets
            var filenameOffsets = [UInt32](); filenameOffsets.reserveCapacity(header_fileCount) // relative offset in filenames section
            for i in 0..<header_fileCount {
                filenameOffsets[i] = _r.readLEUInt32()
            }

            // Read filenames
            let filenamesSectionStartPos = _r.baseStream.position
            var buf = Data(); buf.reserveCapacity(64)
            for i in 0..<header_fileCount {
                _r.baseStream.position = filenamesSectionStartPos + UInt64(filenameOffsets[i])
                buf.removeAll(keepingCapacity: true)
                var curCharAsByte: UInt8 = _r.readByte()
                while curCharAsByte != 0 {
                    buf.append(curCharAsByte)
                    curCharAsByte = _r.readByte()
                }
                _files[i].path = String(data: buf, encoding: .utf8)!
            }

            // Read filename hashes
            _r.baseStream.position = hashTablePosition
            for i in 0..<header_fileCount {
                _files[i].pathHash = UInt64((_r.readLEUInt32() << 32) | _r.readLEUInt32())
            }
        }
        else { fatalError("BAD MAGIC") }

        // Create the file metadata hash table
        //_filesByHash = _files.ToLookup(x => x.PathHash);

        // // Create a virtual directory tree.
        // RootDir = VirtualFileSystem.Directory();
        // foreach (var fileMetadata in _files)
        //     RootDir.CreateDescendantFile(fileMetadata.Path);
    }

    func hashFilePath(_ filePath: String) -> UInt64 {
        if magic == BsaFile.MW_BSAHEADER_FILEID { return BsaFile.tes3HashFilePath(filePath) }
        else { return BsaFile.tes4HashFilePath(filePath) }
    }

    // http://en.uesp.net/wiki/Tes3Mod:BSA_File_Format
    static func tes3HashFilePath(_ filePath: String) -> UInt64 {
        let fileData = [UInt8](filePath.lowercased().replacingOccurrences(of: "/", with: "\\").utf8)
        let len = Int(fileData.count)
        //
        let l: Int = (len >> 1)
        var off: Int = 0, i: Int = 0
        var sum: UInt = 0, temp: UInt, n: UInt
        for i in 0..<l {
            sum ^= UInt(fileData[i]) << (off & 0x1F)
            off += 8
        }
        let value1 = sum
        sum = 0; off = 0
        for i in i..<len {
            temp = UInt(fileData[i]) << (off & 0x1F)
            sum ^= temp
            n = temp & 0x1F
            sum = (sum << (32 - n)) | (sum >> n)
            off += 8
        }
        let value2 = sum
        return UInt64((value1 << 32) | value2)
    }

    static func tes4HashFilePath(_ filePath: String) -> UInt64 {
        func genHash2(_ s: String) -> UInt32 {
            var hash: UInt32 = 0
            for i in [UInt8](s.utf8) {
                hash *= 0x1003f
                hash += UInt32(i)
            }
            return hash
        }
        
        func genHash(_ file: String, ext: String) -> UInt64 {
            let fileData = [UInt8](file.utf8)
            let fileLen = fileData.count
            var hash: UInt64 = 0
            if fileLen > 0 {
                let val0 = (UInt64(fileData[fileLen - 1]) * 0x1) +
                    (UInt64(fileLen > 2 ? fileData[fileLen - 2] : 0) * 0x100)
                let val1 = (UInt64(fileLen) * 0x10000) +
                    (UInt64(fileData[0]) * 0x1000000)
                hash = UInt64(val0 + val1)
            }
            if fileLen > 3 {
                let str = String(file[file.index(file.startIndex, offsetBy: 1)..<file.index(file.endIndex, offsetBy: -3)])
                hash += UInt64(UInt64(genHash2(str)) * 0x100000000)
            }
            if ext.count > 0 {
                hash += UInt64(UInt64(genHash2(ext)) * 0x100000000)
                var i: UInt8
                switch ext {
                case ".nif": i = 1
                case ".kf": i = 2
                case ".dds": i = 3
                case ".wav": i = 4
                default: i = 0
                }
                if i != 0 {
                    let a: UInt8 = ((i & 0xfc) << 5) + UInt8((hash & 0xff000000) >> 24)
                    let b: UInt8 = ((i & 0xfe) << 6) + UInt8(hash & 0xff)
                    let c: UInt8 = (i << 7) + UInt8((hash & 0xff00) >> 8)
                    hash -= hash & 0xFF00FFFF
                    hash += UInt64((a << 24) + b + (c << 8))
                }
            }
            return hash
        }
        
        let newPath = filePath.lowercased().replacingOccurrences(of: "/", with: "\\")
        let url = URL(fileURLWithPath: newPath)
        return genHash(url.deletingPathExtension().path, ext: url.pathExtension)
    }
}
