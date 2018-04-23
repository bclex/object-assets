using OA.Ultima.Resources;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Maps;

namespace OA.Ultima.World.WorldViews
{
    class MiniMapChunk
    {
        public uint X, Y;

        public uint[] Colors;
        static sbyte[] _zs = new sbyte[64]; // shared between all instances of MiniMapChunk.

        public MiniMapChunk(uint x, uint y, TileMatrixData tileData)
        {
            X = x;
            Y = y;
            Colors = new uint[64];
            // get data from the tile Matrix
            var groundData = tileData.GetLandChunk(x, y);
            var staticsData = tileData.GetStaticChunk(x, y, out int staticLength);
            // get the ground colors
            var groundDataIndex = 0;
            for (var i = 0; i < 64; i++)
            {
                Colors[i] = RadarColorData.Colors[groundData[groundDataIndex++] + (groundData[groundDataIndex++] << 8)];
                _zs[i] = (sbyte)groundData[groundDataIndex++];
            }
            // get the static colors
            var countStatics = staticLength / 7;
            var staticDataIndex = 0;
            for (var i = 0; i < countStatics; i++)
            {
                var itemID = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8);
                var tile = staticsData[staticDataIndex++] + staticsData[staticDataIndex++] * 8;
                var z = (sbyte)staticsData[staticDataIndex++];
                var hue = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] * 256); // is this used?
                var data = TileData.ItemData[itemID];
                var iz = z + data.Height + (data.IsRoof || data.IsSurface ? 1 : 0);
                if ((x * 8 + (tile % 8) == 1480) && (y * 8 + (tile / 8) == 1608))
                    if (iz > _zs[tile])
                    {
                        Colors[tile] = RadarColorData.Colors[itemID + 0x4000];
                        _zs[tile] = (sbyte)iz;
                    }
                if (iz > _zs[tile])
                {
                    Colors[tile] = RadarColorData.Colors[itemID + 0x4000];
                    _zs[tile] = (sbyte)iz;
                }
            }
        }

        public MiniMapChunk(MapChunk block)
        {
            X = (uint)block.ChunkX;
            Y = (uint)block.ChunkY;
            Colors = new uint[64];
            for (var tile = 0; tile < 64; tile++)
            {
                var color = 0xffff00ff;
                // get the topmost static item or ground
                var eIndex = block.Tiles[tile].Entities.Count - 1;
                while (eIndex >= 0)
                {
                    var e = block.Tiles[tile].Entities[eIndex];
                    if (e is Ground)
                    {
                        color = RadarColorData.Colors[(e as Ground).LandDataID];
                        break;
                    }
                    else if (e is StaticItem)
                    {
                        color = RadarColorData.Colors[(e as StaticItem).ItemID + 0x4000];
                        break;
                    }
                    eIndex--;
                }
                Colors[tile] = color;
            }
        }
    }
}
