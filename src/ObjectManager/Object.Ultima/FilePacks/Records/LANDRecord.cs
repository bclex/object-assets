using OA.Core;
using OA.Ultima.Data;
using OA.Ultima.Resources;

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
                // load the ground data into the tiles.
                var groundData = _tileData.GetLandChunk(GridX, GridY);
                var tiles = new Tile[64];
                var groundDataIndex = 0;
                for (var i = 0; i < 64; i++)
                {
                    var tileId = (short)(groundData[groundDataIndex++] + (groundData[groundDataIndex++] << 8));
                    var tileZ = (sbyte)groundData[groundDataIndex++];
                    var data = TileData.LandData[tileId & 0x3FFF];
                    tiles[i] = new Tile
                    {
                        //X = (int)GridX * 8 + i % 8,
                        //Y = (int)GridY * 8 + (i / 8),
                        Flags = data.Flags,
                        TextureId = data.TextureID,
                        Z = tileZ,
                    };
                }
                Tiles = tiles;
            }
        }
    }
}