using OA.Core;
using OA.Ultima.Resources;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.FilePacks.Records
{
    public class CELLRecord : Record, ICellRecord
    {
        readonly TileMatrixData _tileData;
        public readonly uint GridX;
        public readonly uint GridY;

        public CELLRecord(TileMatrixData tileData, uint gridX, uint gridY)
        {
            _tileData = tileData;
            GridX = gridX;
            GridY = gridY;
        }

        public class RefObj
        {
            public short ItemId;
            public int Hue;
            public int SortInfluence;
            public Vector3 Position;
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
                for (uint y = 0; y < DataFile.CELL_PACK; y++)
                    for (uint x = 0; x < DataFile.CELL_PACK; x++)
                        LoadTile(refObjs, (GridX * DataFile.CELL_PACK) + x, (GridY * DataFile.CELL_PACK) + y);
                RefObjs = refObjs.ToArray();
            }
        }

        public void LoadTile(List<RefObj> refObjs, uint chunkX, uint chunkY)
        {
            // load the statics data into the tiles
            var staticsData = _tileData.GetStaticChunk(chunkX, chunkY, out int staticLength);
            var countStatics = staticLength / 7;
            var staticDataIndex = 0;
            for (var i = 0; i < countStatics; i++)
            {
                var itemId = (short)(staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8));
                var itemX = (sbyte)staticsData[staticDataIndex++];
                var itemY = (sbyte)staticsData[staticDataIndex++];
                var itemZ = (sbyte)staticsData[staticDataIndex++];
                var hue = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] * 256);
                refObjs.Add(new RefObj
                {
                    ItemId = itemId,
                    Hue = hue,
                    SortInfluence = i,
                    Position = new Vector3((int)chunkY * 8 + itemX, itemZ, (int)chunkY * 8 + itemY),
                });
            }
        }
    }
}