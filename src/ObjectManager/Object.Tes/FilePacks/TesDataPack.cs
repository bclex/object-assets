using OA.Core;
using System.IO;

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

        ICellRecord IDataPack.FindExteriorCellRecord(Vector2i cellId) { return FindExteriorCellRecord(cellId); }
        //ICellRecord IDataPack.FindInteriorCellRecord(string cellId) { return FindInteriorCellRecord(cellName); }
        ICellRecord IDataPack.FindInteriorCellRecord(Vector2i gridId) { return FindInteriorCellRecord(gridId); }
    }
}
