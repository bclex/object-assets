using OA.Core;
using OA.Ultima.Resources;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.FilePacks.Records
{
    public class CELLRecord : Record, ICellRecord
    {
        readonly TileMatrixData _tileData;
        readonly LANDRecord _land;
        public readonly uint GridX;
        public readonly uint GridY;

        public CELLRecord(TileMatrixData tileData, LANDRecord land, uint gridX, uint gridY)
        {
            _tileData = tileData;
            _land = land;
            GridX = gridX;
            GridY = gridY;
            //Load();
        }

        public class RefObj
        {
            public bool Land;
            public short TileId;
            public int Hue;
            public int SortInfluence;
            public Vector3 Position;
            public Vector3 EulerAngles;
            public float? Scale;
            public string Name => Land ? $"lnd{TileId:000}" : $"sta{TileId:000}";
        }

        public string Name => $"{GridX}x{GridY}";
        public bool IsInterior => false;
        public Vector3Int GridId => new Vector3Int((int)GridX, (int)GridY, 0);
        public Color? AmbientLight => null;

        public RefObj[] RefObjs;

        public void Read()
        {
            if (RefObjs != null) return;
            lock (this)
            {
                if (RefObjs != null) return;
                //Utils.Log($"CELL: {GridX}x{GridY}");
                var refObjs = new List<RefObj>();
                _land.Read();
                var heights = _land.Heights;
                var tiles = _land.Tiles;
                for (uint y = 0; y < DataFile.CELL_PACK; y++)
                    for (uint x = 0; x < DataFile.CELL_PACK; x++)
                        ReadTile(heights, tiles, refObjs, x * DataFile.CELL_PACK, y * DataFile.CELL_PACK, GridX * DataFile.CELL_PACK + x, GridY * DataFile.CELL_PACK + y);
                RefObjs = refObjs.ToArray();
            }
        }

        const int STRIDE0 = 8 * DataFile.CELL_PACK;
        public void ReadTile(sbyte[] heights, LANDRecord.Tile[] tiles, List<RefObj> refObjs, uint offsetX, uint offsetY, uint chunkX, uint chunkY)
        {
#if false
            // load land tiles
            for (uint x = 0; x < 8; x++)
                for (uint y = 0; y < 8; y++)
                {
                    var idx = ((offsetY + y) * STRIDE0) + offsetX + x;
                    var tile = tiles[idx];
                    if (tile.Ignored)
                        continue;
                    var z = heights[idx];
                    refObjs.Add(new RefObj
                    {
                        Land = true,
                        TileId = tile.TileId,
                        Position = new Vector3((int)chunkX * 8 + x, (int)chunkY * 8 + y, z / ConvertUtils.MeterInUnits),
                    });
                }
#endif
            // load the statics data into the tiles
            var staticsData = _tileData.GetStaticChunk(chunkX, chunkY, out int staticLength);
            var countStatics = staticLength / 7;
            var staticDataIndex = 0;
            for (var i = 0; i < countStatics; i++)
            {
                var tileId = (short)(staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8));
                var x = (sbyte)staticsData[staticDataIndex++];
                var y = (sbyte)staticsData[staticDataIndex++];
                var z = (sbyte)staticsData[staticDataIndex++];
                var hue = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] * 256);
                var itemData = TileData.ItemData[tileId];
                if (itemData.Ignored)
                    continue;
                //var idx = ((offsetY + y) * STRIDE0) + offsetX + x;
                //z += heights[idx];
                refObjs.Add(new RefObj
                {
                    TileId = tileId,
                    Hue = hue,
                    SortInfluence = i,
                    Position = new Vector3((int)chunkX * 8 + x, (int)chunkY * 8 + y, z / ConvertUtils.MeterInUnits),
                });
            }
        }
    }
}