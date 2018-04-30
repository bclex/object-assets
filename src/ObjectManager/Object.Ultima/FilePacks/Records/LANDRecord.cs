using OA.Core;
using OA.Ultima.Data;
using OA.Ultima.Resources;
using System.Collections.Generic;

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
            //public int X;
            //public int Y;
            public TileFlag Flags;
            public short TextureId;
            public int Z;
        }

        public Vector2i GridCoords
        {
            get { return new Vector2i((int)GridX, (int)GridY); }
        }

        public Tile[] Tiles;

        public void Load()
        {
            if (Tiles != null) return;
            lock (this)
            {
                if (Tiles != null) return;
                //Utils.Log($"LAND: {GridX}x{GridY}");
                var tiles = new Tile[64 * DataFile.CELL_PACK * DataFile.CELL_PACK];
                for (uint y = 0; y < DataFile.CELL_PACK; y++)
                    for (uint x = 0; x < DataFile.CELL_PACK; x++)
                        LoadTile(tiles, x * DataFile.CELL_PACK, y * DataFile.CELL_PACK, (GridX * DataFile.CELL_PACK) + x, (GridY * DataFile.CELL_PACK) + y);
                Tiles = tiles;
            }
        }

        const int STRIDE = 8 * DataFile.CELL_PACK;
        public void LoadTile(Tile[] tiles, uint offsetX, uint offsetY, uint chunkX, uint chunkY)
        {
            // load the ground data into the tiles.
            var groundData = _tileData.GetLandChunk(chunkX, chunkY);
            var groundDataIndex = 0;
            for (uint y = 0; y < 8; y++)
                for (uint x = 0; x < 8; x++)
                {
                    var tileId = (short)(groundData[groundDataIndex++] + (groundData[groundDataIndex++] << 8));
                    var tileZ = (sbyte)groundData[groundDataIndex++];
                    var data = TileData.LandData[tileId & 0x3FFF];
                    var idx = ((offsetY + y) * STRIDE) + offsetX + x;
                    tiles[idx] = new Tile
                    {
                        //X = (int)chunkX * 8 + x,
                        //Y = (int)chunkY * 8 + y,
                        Flags = data.Flags,
                        TextureId = data.TextureID,
                        Z = tileZ,
                    };
                }

        }
    }
}