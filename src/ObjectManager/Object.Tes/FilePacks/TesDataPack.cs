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
    }
}
