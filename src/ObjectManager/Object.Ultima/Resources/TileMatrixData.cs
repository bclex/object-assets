using OA.Core;
using OA.Core.Diagnostics;
using OA.Ultima.Core;
using OA.Ultima.IO;
using System;
using System.IO;

namespace OA.Ultima.Resources
{
    public class TileMatrixData
    {
        // === Constant Data ==========================================================================================
        readonly uint[] MapChunkHeightList = { 512, 512, 200, 256, 181 };
        const int SizeOfLandChunk = 196;
        const int SizeOfLandChunkData = 192;
        const uint CountBufferedLandChunk = 256;
        const int SizeOfInitialStaticTileLoadingBuffer = 16384;
        static byte[] _emptyStaticsChunk = new byte[0];
        static byte[] _invalidLandChunk = new byte[SizeOfLandChunkData];

        // === Instance data ==========================================================================================
        public readonly uint ChunkHeight;
        public readonly uint ChunkWidth;
        public readonly uint MapIndex;
        readonly byte[][] _bufferedLandChunks;
        readonly uint[] _bufferedLandChunkKeys;
        byte[] _staticTileLoadingBuffer;
        readonly TileMatrixDataPatch _patch;
        readonly FileStream _mapDataStream;
        readonly FileStream _staticDataStream;
        readonly BinaryReader _staticIndexReader;
        readonly UOPIndex _UOPIndex;

        public TileMatrixData(uint index)
        {
            MapIndex = index;
            // Map file fallback order: mapX.mul => mapXLegacyMUL.uop => (if trammel / map index 1) => map0.mul => mapXLegacyMUL.uop
            if (!LoadMapStream(MapIndex, out _mapDataStream, out _UOPIndex))
            {
                if (MapIndex == 1 && LoadMapStream(0, out _mapDataStream, out _UOPIndex)) Utils.Log("Map file for index 1 did not exist, successfully loaded index 0 instead.");
                else Utils.Error($"Unable to load map index {MapIndex}");
            }
            ChunkHeight = MapChunkHeightList[MapIndex];
            ChunkWidth = (uint)_mapDataStream.Length / (ChunkHeight * SizeOfLandChunk);
            // load map patch and statics
            _patch = new TileMatrixDataPatch(this, MapIndex);
            if (!LoadStaticsStream(MapIndex, out _staticDataStream, out _staticIndexReader))
            {
                if (MapIndex == 1 && LoadStaticsStream(0, out _staticDataStream, out _staticIndexReader)) Utils.Log("Statics file for index 1 did not exist, successfully loaded index 0 instead.");
                else Utils.Error($"Unable to load static index {MapIndex}");
            }
            // load buffers
            _bufferedLandChunkKeys = new uint[CountBufferedLandChunk];
            _bufferedLandChunks = new byte[CountBufferedLandChunk][];
            for (var i = 0; i < CountBufferedLandChunk; i++)
                _bufferedLandChunks[i] = new byte[SizeOfLandChunkData];
            _staticTileLoadingBuffer = new byte[SizeOfInitialStaticTileLoadingBuffer];
        }

        bool LoadMapStream(uint index, out FileStream mapDataStream, out UOPIndex uopIndex)
        {
            mapDataStream = null;
            uopIndex = null;
            var path = FileManager.GetFilePath($"map{index}.mul");
            if (File.Exists(path))
            {
                mapDataStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return true;
            }
            path = FileManager.GetFilePath($"map{index}LegacyMUL.uop");
            if (File.Exists(path))
            {
                mapDataStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                uopIndex = new UOPIndex(_mapDataStream);
                return true;
            }
            return false;
        }

        bool LoadStaticsStream(uint index, out FileStream dataStream, out BinaryReader indexReader)
        {
            dataStream = null;
            indexReader = null;
            var pathData = FileManager.GetFilePath($"statics{index}.mul");
            var pathIndex = FileManager.GetFilePath($"staidx{index}.mul");
            if (File.Exists(pathData) && File.Exists(pathIndex))
            {
                dataStream = new FileStream(pathData, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                indexReader = new BinaryReader(new FileStream(pathIndex, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            _mapDataStream?.Close();
            _UOPIndex?.Close();
            _staticIndexReader?.Close();
            _staticDataStream?.Close();
        }

        public byte[] GetLandChunk(uint chunkX, uint chunkY)
        {
            return _mapDataStream == null ? _invalidLandChunk : ReadLandChunk(chunkX, chunkY);
        }

        /// <summary>
        /// Retrieve the tileID and altitude of a specific land tile. VERY INEFFECIENT.
        /// </summary>
        public void GetLandTile(uint tileX, uint tileY, out ushort TileID, out sbyte altitude)
        {
            var index = (((tileX % 8) + (tileY % 8) * 8) * 3);
            var data = ReadLandChunk(tileX >> 3, tileY >> 3);
            TileID = BitConverter.ToUInt16(data, (int)index);
            altitude = (sbyte)data[index + 2];
        }

        public byte[] GetStaticChunk(uint chunkX, uint chunkY, out int length)
        {
            chunkX %= ChunkWidth;
            chunkY %= ChunkHeight;
            if (_staticDataStream == null || _staticIndexReader.BaseStream == null)
            {
                length = 0;
                return _emptyStaticsChunk;
            }
            return ReadStaticChunk(chunkX, chunkY, out length);
        }

        unsafe byte[] ReadStaticChunk(uint chunkX, uint chunkY, out int length)
        {
            // bounds check: keep chunk index within bounds of map
            chunkX %= ChunkWidth;
            chunkY %= ChunkHeight;
            // load the map chunk from a file. Check the patch file first (mapdif#.mul), then the base file (map#.mul).
            if (_patch.TryGetStaticChunk(MapIndex, chunkX, chunkY, ref _staticTileLoadingBuffer, out length))
                return _staticTileLoadingBuffer;
            try
            {
                _staticIndexReader.BaseStream.Seek(((chunkX * ChunkHeight) + chunkY) * 12, SeekOrigin.Begin);
                var lookup = _staticIndexReader.ReadInt32();
                length = _staticIndexReader.ReadInt32();
                if (lookup < 0 || length <= 0)
                    return _emptyStaticsChunk;
                _staticDataStream.Seek(lookup, SeekOrigin.Begin);
                if (length > _staticTileLoadingBuffer.Length)
                {
                    _staticTileLoadingBuffer = new byte[length];
                }
                Utility.ReadBuffer(_staticDataStream, _staticTileLoadingBuffer, length);
                return _staticTileLoadingBuffer;
            }
            catch (EndOfStreamException) { throw new Exception("End of stream in static chunk!"); }
        }

        unsafe byte[] ReadLandChunk(uint chunkX, uint chunkY)
        {
            // bounds check: keep chunk index within bounds of map
            chunkX %= ChunkWidth;
            chunkY %= ChunkHeight;
            // if this chunk is cached in the buffer, return the cached chunk.
            var key = (chunkX << 16) + chunkY;
            var index = chunkX % 16 + ((chunkY % 16) * 16);
            if (_bufferedLandChunkKeys[index] == key)
                return _bufferedLandChunks[index];
            // if it was not cached in the buffer, we will be loading it.
            _bufferedLandChunkKeys[index] = key;
            // load the map chunk from a file. Check the patch file first (mapdif#.mul), then the base file (map#.mul).
            if (_patch.TryGetLandPatch(MapIndex, chunkX, chunkY, ref _bufferedLandChunks[index]))
                return _bufferedLandChunks[index];
            var ptr = (int)((chunkX * ChunkHeight) + chunkY) * SizeOfLandChunk + 4;
            if (_UOPIndex != null)
                ptr = _UOPIndex.Lookup(ptr);
            _mapDataStream.Seek(ptr, SeekOrigin.Begin);
            Utility.ReadBuffer(_mapDataStream, _bufferedLandChunks[index], SizeOfLandChunkData);
            Metrics.ReportDataRead(SizeOfLandChunkData);
            return _bufferedLandChunks[index];
        }
    }
}
