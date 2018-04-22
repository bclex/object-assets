using OA.Tes.IO;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OA.Tes
{
    public class TesAssetManager : IGameAssetManager
    {
        public Task<IAssetPack> GetAssetPack(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "file":
                    {
                        var localPath = uri.LocalPath;
                        if (!File.Exists(localPath) && !Directory.Exists(localPath))
                            throw new IndexOutOfRangeException("file or directory not found");
                        var pack = (IAssetPack)new TesAssetPack(localPath, null);
                        return Task.FromResult(pack);
                    }
                case "game":
                    {
                        var localPath = uri.LocalPath.Substring(1);
                        var gameId = StringToGameId(uri.Host);
                        var filePath = FileManager.GetFilePath(localPath, gameId);
                        var pack = (IAssetPack)new TesAssetPack(filePath, null);
                        return Task.FromResult(pack);
                    }
                default:
                    {
                        var pack = (IAssetPack)new TesAssetPack(null, null);
                        return Task.FromResult(pack);
                    }
            }
        }

        static GameId StringToGameId(string key)
        {
            var game = Enum.GetNames(typeof(GameId)).FirstOrDefault(x => string.Compare(x, key, true) == 0);
            if (game == null)
                throw new ArgumentOutOfRangeException("key", key);
            return (GameId)Enum.Parse(typeof(GameId), game);
        }
    }
}
