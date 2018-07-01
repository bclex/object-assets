using OA.Core;
using System.IO;
using UnityEngine;

namespace OA.Tes.FilePacks
{
    public class TesDataPack : EsmFile, IDataPack
    {
        string _webPath;

        public TesDataPack(string filePath, string webPath, GameId gameId)
            : base(filePath != null && File.Exists(filePath) ? filePath : null, gameId)
        {
            _webPath = webPath;
        }

        ICellRecord IDataPack.FindCellRecord(Vector3Int cellId) { return FindCellRecord(cellId); }
        //ICellRecord IDataPack.FindInteriorCellRecord(string cellId) { return FindInteriorCellRecord(cellName); }
    }
}
