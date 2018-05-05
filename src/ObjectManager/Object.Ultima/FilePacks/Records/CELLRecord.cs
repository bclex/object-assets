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
            public short ItemId;
            public int Hue;
            public int SortInfluence;
            public Vector3 Position;
            public Vector3 EulerAngles;
            public float? Scale;
            public string Name
            {
                get { return $"sta{ItemId}"; }
            }
        }

        public string Name
        {
            get { return $"{GridX}x{GridY}"; }
        }

        public bool IsInterior
        {
            get { return false; }
        }

        public Vector2i GridCoords
        {
            get { return new Vector2i((int)GridX, (int)GridY); }
        }

        public Color? AmbientLight => null;

        public RefObj[] RefObjs;

        public void Load()
        {
            if (RefObjs != null) return;
            lock (this)
            {
                if (RefObjs != null) return;
                //Utils.Log($"CELL: {GridX}x{GridY}");
                var refObjs = new List<RefObj>();
                _land.Load();
                var heights = _land.Heights;
                for (uint y = 0; y < DataFile.CELL_PACK; y++)
                    for (uint x = 0; x < DataFile.CELL_PACK; x++)
                        LoadTile(heights, refObjs, x * DataFile.CELL_PACK, y * DataFile.CELL_PACK, GridX * DataFile.CELL_PACK + x, GridY * DataFile.CELL_PACK + y);
                RefObjs = refObjs.ToArray();
            }
        }

        const int STRIDE0 = 8 * DataFile.CELL_PACK;
        public void LoadTile(sbyte[] heights, List<RefObj> refObjs, uint offsetX, uint offsetY, uint chunkX, uint chunkY)
        {
            // load the statics data into the tiles
            var staticsData = _tileData.GetStaticChunk(chunkX, chunkY, out int staticLength);
            var countStatics = staticLength / 7;
            var staticDataIndex = 0;
            for (var i = 0; i < countStatics; i++)
            {
                var itemId = (short)(staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8));
                var x = (sbyte)staticsData[staticDataIndex++];
                var y = (sbyte)staticsData[staticDataIndex++];
                var z = (sbyte)staticsData[staticDataIndex++];
                var hue = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] * 256);
                //
                var itemData = TileData.ItemData[itemId];
                if (itemData.IsFoliage)
                    continue;
                var idx = ((offsetY + y) * STRIDE0) + offsetX + x;
                z += heights[idx];
                refObjs.Add(new RefObj
                {
                    ItemId = itemId,
                    Hue = hue,
                    SortInfluence = i,
                    Position = new Vector3((int)chunkX * 8 + x, (int)chunkY * 8 + y, z / ConvertUtils.MeterInUnits),
                });
            }
        }
    }
}