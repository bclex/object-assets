using OA.Ultima.Data;
using OA.Ultima.Resources;
using System;
using UnityEngine;

namespace OA.Ultima.World.Maps
{
    public class Map
    {
        readonly MapChunk[] _chunks;
        public TileMatrixData MapData { get; private set; }

        Vector2Int _center = new Vector2Int(int.MinValue, int.MinValue); // player position.

        public readonly uint Index;
        public readonly uint TileHeight, TileWidth;

        // Any mobile / item beyond this range is removed from the client. RunUO's range is 24 tiles, which would equal 3 cells.
        // We keep 4 cells in memory to allow for drawing further, and also as a safety precaution - don't want to unload an 
        // entity at the edge of what we keep in memory just because of being slightly out of sync with the server.
        const int c_CellsInMemory = 5;
        const int c_CellsInMemorySpan = c_CellsInMemory * 2 + 1;

        public Map(uint index)
        {
            Index = index;
            MapData = new TileMatrixData(Index);
            TileHeight = MapData.ChunkHeight * 8;
            TileWidth = MapData.ChunkWidth * 8;
            _chunks = new MapChunk[c_CellsInMemorySpan * c_CellsInMemorySpan];
        }

        public void Dispose()
        {
            for (var i = 0; i < _chunks.Length; i++)
                if (_chunks[i] != null)
                {
                    _chunks[i].Dispose();
                    _chunks[i] = null;
                }
        }

        public Vector2Int CenterPosition
        {
            get { return _center; }
            set
            {
                if (value != _center)
                    _center = value;
                InternalCheckCellsInMemory();
            }
        }

        public MapChunk GetMapChunk(uint x, uint y)
        {
            var cellIndex = (y % c_CellsInMemorySpan) * c_CellsInMemorySpan + (x % c_CellsInMemorySpan);
            var cell = _chunks[cellIndex];
            if (cell == null)
                return null;
            if (cell.ChunkX != x || cell.ChunkY != y)
                return null;
            return cell;
        }

        public MapTile GetMapTile(int x, int y)
        {
            return GetMapTile((uint)x, (uint)y);
        }

        public MapTile GetMapTile(uint x, uint y)
        {
            uint cellX = (uint)x / 8, cellY = (uint)y / 8;
            uint cellIndex = (cellY % c_CellsInMemorySpan) * c_CellsInMemorySpan + (cellX % c_CellsInMemorySpan);
            var cell = _chunks[cellIndex];
            if (cell == null)
                return null;
            if (cell.ChunkX != cellX || cell.ChunkY != cellY)
                return null;
            return cell.Tiles[(y % 8) * 8 + (x % 8)];
        }

        private void InternalCheckCellsInMemory()
        {
            var centerX = ((uint)CenterPosition.x / 8);
            var centerY = ((uint)CenterPosition.y / 8);
            for (var y = -c_CellsInMemory; y <= c_CellsInMemory; y++)
            {
                var cellY = (uint)(centerY + y) % MapData.ChunkHeight;
                for (var x = -c_CellsInMemory; x <= c_CellsInMemory; x++)
                {
                    var cellX = (uint)(centerX + x) % MapData.ChunkWidth;
                    var cellIndex = (cellY % c_CellsInMemorySpan) * c_CellsInMemorySpan + (cellX % c_CellsInMemorySpan);
                    if (_chunks[cellIndex] == null || _chunks[cellIndex].ChunkX != cellX || _chunks[cellIndex].ChunkY != cellY)
                    {
                        if (_chunks[cellIndex] != null)
                            _chunks[cellIndex].Dispose();
                        _chunks[cellIndex] = new MapChunk(cellX, cellY);
                        _chunks[cellIndex].LoadStatics(MapData, this);
                        // if we have a translator and it's not spring, change some statics!
                        if (Season != Seasons.Spring && SeasonalTranslator != null)
                            SeasonalTranslator(_chunks[cellIndex], Season);
                        // let any active multis know that a new map chunk is ready, so they can load in their pieces.
                        Multi.AnnounceMapChunkLoaded(_chunks[cellIndex]);
                    }
                }
            }
        }

        public float GetTileZ(int x, int y)
        {
            var t = GetMapTile(x, y);
            if (t != null) return t.Ground.Z;
            else
            {
                // THIS IS VERY INEFFICIENT :(
                MapData.GetLandTile((uint)x, (uint)y, out ushort tileID, out sbyte alt);
                return alt;
            }
        }

        public int GetAverageZ(int top, int left, int right, int bottom, ref int low, ref int high)
        {
            high = top;
            if (left > high) high = left;
            if (right > high) high = right;
            if (bottom > high) high = bottom;
            low = high;
            if (left < low) low = left;
            if (right < low) low = right;
            if (bottom < low) low = bottom;
            if (Math.Abs(top - bottom) > Math.Abs(left - right)) return FloorAverage(left, right);
            else return FloorAverage(top, bottom);
        }

        public int GetAverageZ(int x, int y, ref int low, ref int top)
        {
            return GetAverageZ(
                (int)GetTileZ(x, y),
                (int)GetTileZ(x, y + 1),
                (int)GetTileZ(x + 1, y),
                (int)GetTileZ(x + 1, y + 1),
                ref low, ref top);
        }

        private static int FloorAverage(int a, int b)
        {
            int v = a + b;
            if (v < 0)
                --v;
            return (v / 2);
        }

        Seasons _season = Seasons.Summer;
        public Seasons Season
        {
            get { return _season; }
            set
            {
                if (_season != value)
                {
                    _season = value;
                    if (SeasonalTranslator != null)
                        foreach (var chunk in _chunks)
                            SeasonalTranslator(chunk, Season);
                }
            }
        }

        public static Action<MapChunk, Seasons> SeasonalTranslator;

        public void ReloadStatics()
        {
            foreach (var chunk in _chunks)
                if (chunk != null)
                {
                    chunk.UnloadStatics();
                    chunk.LoadStatics(MapData, this);
                }
        }
    }
}