
using OA.Ultima.Resources;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items;

namespace OA.Ultima.World.Maps
{
    public class MapChunk
    {
        public MapTile[] Tiles;

        public readonly uint ChunkX;
        public readonly uint ChunkY;

        public MapChunk(uint x, uint y)
        {
            ChunkX = x;
            ChunkY = y;
            Tiles = new MapTile[64];
            for (var i = 0; i < 64; i++)
                Tiles[i] = new MapTile();
        }

        /// <summary>
        /// Unloads all tiles and entities from memory.
        /// </summary>
        public void Dispose()
        {
            for (var i = 0; i < 64; i++)
                if (Tiles[i] != null)
                {
                    for (var j = 0; j < Tiles[i].Entities.Count; j++)
                    {
                        if (Tiles[i].Entities[j].IsClientEntity) { } // Never dispose of the client entity.
                        else
                        {
                            var entityCount = Tiles[i].Entities.Count;
                            Tiles[i].Entities[j].Dispose();
                            if (entityCount == Tiles[i].Entities.Count)
                                Tiles[i].Entities.RemoveAt(j);
                            j--; // entity will dispose, removing it from collection.
                        }
                    }
                    Tiles[i] = null;
                }
            Tiles = null;
        }

        public void LoadStatics(TileMatrixData tileData, Map map)
        {
            // get data from the tile Matrix
            var groundData = tileData.GetLandChunk(ChunkX, ChunkY);
            var staticsData = tileData.GetStaticChunk(ChunkX, ChunkY, out int staticLength);
            // load the ground data into the tiles.
            var groundDataIndex = 0;
            for (var i = 0; i < 64; i++)
            {
                var tileID = groundData[groundDataIndex++] + (groundData[groundDataIndex++] << 8);
                var tileZ = (sbyte)groundData[groundDataIndex++];
                var ground = new Ground(tileID, map);
                ground.Position.Set((int)ChunkX * 8 + i % 8, (int)ChunkY * 8 + (i / 8), tileZ);
            }
            // load the statics data into the tiles
            var countStatics = staticLength / 7;
            var staticDataIndex = 0;
            for (var i = 0; i < countStatics; i++)
            {
                var tileID = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8);
                var x = staticsData[staticDataIndex++];
                var y = staticsData[staticDataIndex++];
                var tileZ = (sbyte)staticsData[staticDataIndex++];
                var hue = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] * 256);
                var item = new StaticItem(tileID, hue, i, map);
                item.Position.Set((int)ChunkX * 8 + x, (int)ChunkY * 8 + y, tileZ);
            }
        }

        public void UnloadStatics()
        {
            for (var i = 0; i < 64; i++)
                if (Tiles[i] != null)
                    for (var j = 0; j < Tiles[i].Entities.Count; j++)
                        if (Tiles[i].Entities[j] is Ground || Tiles[i].Entities[j] is StaticItem)
                        {
                            var entityCount = Tiles[i].Entities.Count;
                            Tiles[i].Entities[j].Dispose();
                            if (entityCount == Tiles[i].Entities.Count)
                                Tiles[i].Entities.RemoveAt(j);
                            j--; // entity will dispose, removing it from collection.
                        }
        }
    }
}
