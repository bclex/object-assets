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
