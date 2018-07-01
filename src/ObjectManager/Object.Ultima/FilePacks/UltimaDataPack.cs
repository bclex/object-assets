using UnityEngine;

namespace OA.Ultima.FilePacks
{
    public class UltimaDataPack : DataFile, IDataPack
    {
        public UltimaDataPack(uint map)
            : base(map) { }

        ICellRecord IDataPack.FindCellRecord(Vector3Int cellId) { return FindCellRecord(cellId); }
    }
}
