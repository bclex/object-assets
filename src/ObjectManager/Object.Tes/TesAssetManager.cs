using OA.Core;
using OA.Tes.FilePacks;
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
                        //if (!File.Exists(localPath) && !Directory.Exists(localPath))
                        //    throw new IndexOutOfRangeException("file or directory not found");
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

        public Task<IDataPack> GetDataPack(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "file":
                    {
                        var localPath = uri.LocalPath;
                        if (!File.Exists(localPath))
                            throw new IndexOutOfRangeException("file not found");
                        var gameId = StringToGameId(uri.Fragment);
                        var pack = (IDataPack)new TesDataPack(localPath, null, gameId);
                        return Task.FromResult(pack);
                    }
                case "game":
                    {
                        var localPath = uri.LocalPath.Substring(1);
                        var gameId = StringToGameId(uri.Host);
                        var filePath = FileManager.GetFilePath(localPath, gameId);
                        var pack = (IDataPack)new TesDataPack(filePath, null, gameId);
                        return Task.FromResult(pack);
                    }
                default:
                    {
                        var gameId = StringToGameId(uri.Fragment);
                        var pack = (IDataPack)new TesDataPack(null, null, gameId);
                        return Task.FromResult(pack);
                    }
            }
        }

        public ICellManager GetCellManager(IAssetPack asset, IDataPack data, TemporalLoadBalancer temporalLoadBalancer)
        {
            return new CellManager((TesAssetPack)asset, (TesDataPack)data, temporalLoadBalancer);
        }

        static GameId StringToGameId(string key)
        {
            if (key.StartsWith("#")) key = key.Substring(1);
            var game = Enum.GetNames(typeof(GameId)).FirstOrDefault(x => string.Compare(x, key, true) == 0);
            if (game == null)
                throw new ArgumentOutOfRangeException("key", key);
            return (GameId)Enum.Parse(typeof(GameId), game);
        }
    }
}
