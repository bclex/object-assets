using OA.Ultima.Core;
using OA.Ultima.Data;
using OA.Ultima.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace OA.Ultima.Resources
{
    public class TileMatrixDataPatch
    {
        public interface IMapDiffInfo
        {
            int MapCount { get; }
            int[] MapPatches { get; }
            int[] StaticPatches { get; }
        }

        // ============================================================================================================ Static Data ============================================================================================
        static IMapDiffInfo EnabledDiffs;
        public static void EnableMapDiffs(IMapDiffInfo diffs)
        {
            EnabledDiffs = diffs;
        }

        // ============================================================================================================ Instance data ==========================================================================================
        FileStream _landPatchStream;
        FileStream _staticPatchStream;

        Dictionary<uint, LandPatchData> _landPatchPtrs;
        Dictionary<uint, StaticPatchData> _staticPatchPtrs;

        class LandPatchData
        {
            public readonly uint Index;
            public readonly uint Pointer;
            public LandPatchData Next;

            public LandPatchData(uint index, uint ptr)
            {
                Index = index;
                Pointer = ptr;
            }
        }

        class StaticPatchData
        {
            public readonly uint Index;
            public readonly uint Pointer;
            public readonly int Length; // lengths can be negative; if they are, then they should be ignored.
            public StaticPatchData Next;

            public StaticPatchData(uint index, uint ptr, int length)
            {
                Index = index;
                Pointer = ptr;
                Length = length;
            }
        }

        public TileMatrixDataPatch(TileMatrixData matrix, uint index)
        {
            LoadLandPatches(matrix, String.Format("mapdif{0}.mul", index), String.Format("mapdifl{0}.mul", index));
            LoadStaticPatches(matrix, String.Format("stadif{0}.mul", index), String.Format("stadifl{0}.mul", index), String.Format("stadifi{0}.mul", index));
        }

        uint MakeChunkKey(uint blockX, uint blockY)
        {
            return ((blockY & 0x0000ffff) << 16) | (blockX & 0x0000ffff);
        }

        public unsafe bool TryGetLandPatch(uint map, uint blockX, uint blockY, ref byte[] landData)
        {
            if (ClientVersion.InstallationIsUopFormat)
                return false;
            var key = MakeChunkKey(blockX, blockY);
            LandPatchData data;
            if (_landPatchPtrs.TryGetValue(key, out data))
            {
                if (data.Index >= EnabledDiffs.MapPatches[map])
                    return false;
                while (data.Next != null)
                {
                    if (data.Next.Index >= EnabledDiffs.MapPatches[map])
                        break;
                    data = data.Next;
                }
                _landPatchStream.Seek(data.Pointer, SeekOrigin.Begin);
                landData = new byte[192];
                Utility.ReadBuffer(_landPatchStream, landData, 192);
                return true;
            }
            return false;
        }

        unsafe int LoadLandPatches(TileMatrixData tileMatrix, string landPath, string indexPath)
        {
            _landPatchPtrs = new Dictionary<uint, LandPatchData>();
            if (ClientVersion.InstallationIsUopFormat)
                return 0;
            _landPatchStream = FileManager.GetFile(landPath);
            if (_landPatchStream == null)
                return 0;
            using (var fsIndex = FileManager.GetFile(indexPath))
            {
                var indexReader = new BinaryReader(fsIndex);
                var count = (int)(indexReader.BaseStream.Length / 4);
                uint ptr = 0;
                for (uint i = 0; i < count; ++i)
                {
                    var blockID = indexReader.ReadUInt32();
                    var x = blockID / tileMatrix.ChunkHeight;
                    var y = blockID % tileMatrix.ChunkHeight;
                    var key = MakeChunkKey(x, y);
                    ptr += 4;
                    if (_landPatchPtrs.ContainsKey(key))
                    {
                        var current = _landPatchPtrs[key];
                        while (current.Next != null)
                            current = current.Next;
                        current.Next = new LandPatchData(i, ptr);
                    }
                    else  _landPatchPtrs.Add(key, new LandPatchData(i, ptr));
                    ptr += 192;
                }
                indexReader.Close();
                return count;
            }
        }

        public unsafe bool TryGetStaticChunk(uint map, uint blockX, uint blockY, ref byte[] staticData, out int length)
        {
            try
            {
                length = 0;
                if (ClientVersion.InstallationIsUopFormat)
                    return false;
                var key = MakeChunkKey(blockX, blockY);
                StaticPatchData data;
                if (_staticPatchPtrs.TryGetValue(key, out data))
                {
                    if (data.Index >= EnabledDiffs.StaticPatches[map])
                        return false;
                    while (data.Next != null)
                    {
                        if (data.Next.Index >= EnabledDiffs.StaticPatches[map])
                            break;
                        data = data.Next;
                    }
                    if (data.Pointer == 0 || data.Length <= 0)
                        return false;
                    length = data.Length;
                    _staticPatchStream.Seek(data.Pointer, SeekOrigin.Begin);
                    if (length > staticData.Length)
                        staticData = new byte[length];
                    Utility.ReadBuffer(_staticPatchStream, staticData, length);
                    return true;
                }
                length = 0;
                return false;
            }
            catch (EndOfStreamException) { throw new Exception("End of stream in static patch block!"); }
        }

        unsafe int LoadStaticPatches(TileMatrixData tileMatrix, string dataPath, string indexPath, string lookupPath)
        {
            _staticPatchPtrs = new Dictionary<uint, StaticPatchData>();
            _staticPatchStream = FileManager.GetFile(dataPath);
            if (_staticPatchStream == null)
                return 0;
            using (var fsIndex = FileManager.GetFile(indexPath))
            {
                using (var fsLookup = FileManager.GetFile(lookupPath))
                {
                    var indexReader = new BinaryReader(fsIndex);
                    var lookupReader = new BinaryReader(fsLookup);
                    var count = (int)(indexReader.BaseStream.Length / 4);
                    for (uint i = 0; i < count; ++i)
                    {
                        var blockID = indexReader.ReadUInt32();
                        var blockX = blockID / tileMatrix.ChunkHeight;
                        var blockY = blockID % tileMatrix.ChunkHeight;
                        var key = MakeChunkKey(blockX, blockY);
                        var offset = lookupReader.ReadUInt32();
                        var length = lookupReader.ReadInt32();
                        lookupReader.ReadInt32();
                        if (_staticPatchPtrs.ContainsKey(key))
                        {
                            var current = _staticPatchPtrs[key];
                            while (current.Next != null)
                                current = current.Next;
                            current.Next = new StaticPatchData(i, offset, length);
                        }
                        else _staticPatchPtrs.Add(key, new StaticPatchData(i, offset, length));
                    }
                    indexReader.Close();
                    lookupReader.Close();
                    return count;
                }
            }
        }
    }
}