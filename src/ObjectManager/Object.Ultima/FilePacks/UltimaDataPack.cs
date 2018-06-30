using OA.Core;

namespace OA.Ultima.FilePacks
{
    public class UltimaDataPack : DataFile, IDataPack
    {
        public UltimaDataPack(uint map)
            : base(map) { }

        ICellRecord IDataPack.FindExteriorCellRecord(Vector2i cellIndices)
        {
            return FindExteriorCellRecord(cellIndices);
        }

        //ICellRecord IDataPack.FindInteriorCellRecord(string cellName)
        //{
        //    return FindInteriorCellRecord(cellName);
        //}

        ICellRecord IDataPack.FindInteriorCellRecord(Vector2i gridCoords)
        {
            return FindInteriorCellRecord(gridCoords);
        }
    }
}
