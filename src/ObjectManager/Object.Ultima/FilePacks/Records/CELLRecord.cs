using OA.Core;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Ultima.FilePacks.Records
{
    public class CELLRecord : Record, ICellRecord
    {
        readonly TileMatrixData _tileData;
        //public uint Flags;
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
            public int ItemId;
            public int Hue;
            public int SortInfluence;
            public int X;
            public int Y;
            public int Z;
            public string Name
            {
                get { return $"obj{ItemId}"; }
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
                // load the statics data into the tiles
                var staticsData = _tileData.GetStaticChunk(GridX, GridY, out int staticLength);
                var countStatics = staticLength / 7;
                var refObjs = new RefObj[countStatics];
                var staticDataIndex = 0;
                for (var i = 0; i < countStatics; i++)
                {
                    var itemId = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] << 8);
                    var itemX = staticsData[staticDataIndex++];
                    var itemY = staticsData[staticDataIndex++];
                    var itemZ = (sbyte)staticsData[staticDataIndex++];
                    var hue = staticsData[staticDataIndex++] + (staticsData[staticDataIndex++] * 256);
                    refObjs[i] = new RefObj
                    {
                        ItemId = itemId,
                        Hue = hue,
                        SortInfluence = i,
                        X = (int)GridX * 8 + itemX,
                        Y = (int)GridY * 8 + itemY,
                        Z = itemZ,
                    };
                }
                RefObjs = refObjs;
            }
        }
    }
}