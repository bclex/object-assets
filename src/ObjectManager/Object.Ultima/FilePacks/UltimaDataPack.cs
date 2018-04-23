using OA.Core;
using System;

namespace OA.Ultima.FilePacks
{
    public class UltimaDataPack : DataFile, IDataPack
    {
        string _webPath;

        public UltimaDataPack(string filePath, string webPath)
            : base(filePath)
        {
            _webPath = webPath;
        }

        ICellRecord IDataPack.FindExteriorCellRecord(Vector2i cellIndices)
        {
            return FindExteriorCellRecord(cellIndices);
        }

        ICellRecord IDataPack.FindInteriorCellRecord(string cellName)
        {
            return FindInteriorCellRecord(cellName);
        }

        ICellRecord IDataPack.FindInteriorCellRecord(Vector2i gridCoords)
        {
            return FindInteriorCellRecord(gridCoords);
        }
    }
}
