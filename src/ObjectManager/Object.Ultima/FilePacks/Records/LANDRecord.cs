using OA.Core;
using OA.Ultima.Data;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Ultima.FilePacks.Records
{
    public class LANDRecord : Record
    {
        readonly TileMatrixData _tileData;
        public uint GridX;
        public uint GridY;

        public LANDRecord(TileMatrixData tileData, uint gridX, uint gridY)
        {
            _tileData = tileData;
            GridX = gridX;
            GridY = gridY;
        }

        public class Tile
        {
            public short TileId;
            public TileFlag Flags;
            public short TextureId;
            public bool Ignored;
            public bool IsWet => (Flags & TileFlag.Wet) != 0;
            public bool IsImpassible => (Flags & TileFlag.Impassable) != 0;
        }

        public Vector3Int GridId => new Vector3Int((int)GridX, (int)GridY, 0);

        public sbyte[] Heights;
        public Tile[] Tiles;

        public void Read()
        {
            if (Heights != null) return;
            lock (this)
            {
                if (Heights != null) return;
                //Utils.Log($"LAND: {GridX}x{GridY}");
                var heights = new sbyte[STRIDE0 * STRIDE0];
                var tiles = new Tile[STRIDE0 * STRIDE0];
                for (uint y = 0; y < DataFile.CELL_PACK; y++)
                    for (uint x = 0; x < DataFile.CELL_PACK; x++)
                        ReadTile(heights, tiles, x * DataFile.CELL_PACK, y * DataFile.CELL_PACK, (GridX * DataFile.CELL_PACK) + x, (GridY * DataFile.CELL_PACK) + y);
                Heights = heights;
                Tiles = tiles;
            }
        }

        //const int STRIDE1 = (8 * DataFile.CELL_PACK) + 1;
        const int STRIDE0 = 8 * DataFile.CELL_PACK;
        public void ReadTile(sbyte[] heights, Tile[] tiles, uint offsetX, uint offsetY, uint chunkX, uint chunkY)
        {
            // load the ground data into the tiles.
            var groundData = _tileData.GetLandChunk(chunkX, chunkY);
            var groundDataIndex = 0;
            for (uint x = 0; x < 8; x++)
                for (uint y = 0; y < 8; y++)
                {
                    var tileId = (short)(groundData[groundDataIndex++] + (groundData[groundDataIndex++] << 8));
                    var tileZ = (sbyte)groundData[groundDataIndex++];
                    var data = TileData.LandData[tileId & 0x3FFF];
                    var idx = ((offsetY + y) * STRIDE0) + offsetX + x;
                    heights[idx] = tileZ;
                    tiles[idx] = new Tile
                    {
                        //X = (int)chunkX * 8 + x,
                        //Y = (int)chunkY * 8 + y,
                        TileId = tileId,
                        Flags = data.Flags,
                        TextureId = data.TextureID,
                        Ignored = tileId == 2 || tileId == 0x1DB || (tileId >= 0x1AE && tileId <= 0x1B5),
                    };
                }
        }
    }
}