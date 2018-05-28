using OA.Core;
using OA.Tes.FilePacks;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OA.Tes
{
    public enum GameId
    {
        // tes
        Morrowind = 1,
        Oblivion,
        Skyrim,
        SkyrimSE,
        SkyrimVR,
        // fallout
        Fallout3,
        FalloutNV,
        Fallout4,
        Fallout4VR,
    }

    public class TesAssetManager : IAssetManager
    {
        public Task<IAssetPack> GetAssetPack(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "game":
                    {
                        var localPath = uri.LocalPath.Substring(1);
                        var gameId = StringToGameId(uri.Host);
                        var filePaths = FileManager.GetFilePaths(localPath, gameId);
                        if (filePaths == null)
                            throw new InvalidOperationException($"{gameId} not available");
                        var pack = (IAssetPack)new TesAssetPack(filePaths, null);
                        return Task.FromResult(pack);
                    }
                case "file":
                    {
                        var localPath = uri.LocalPath;
                        var gameId = StringToGameId(uri.Host);
                        var filePaths = FileManager.GetFilePaths(localPath, gameId);
                        var pack = (IAssetPack)new TesAssetPack(filePaths, null);
                        return Task.FromResult(pack);
                    }
                default:
                    {
                        var pack = (IAssetPack)new TesAssetPack((string[])null, null);
                        return Task.FromResult(pack);
                    }
            }
        }

        public Task<IDataPack> GetDataPack(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "game":
                    {
                        var localPath = uri.LocalPath.Substring(1);
                        var gameId = StringToGameId(uri.Host);
                        var filePath = FileManager.GetFilePath(localPath, gameId);
                        var pack = (IDataPack)new TesDataPack(filePath, null, gameId);
                        return Task.FromResult(pack);
                    }
                case "file":
                    {
                        var localPath = uri.LocalPath;
                        if (!File.Exists(localPath))
                            throw new IndexOutOfRangeException("file not found");
                        var gameId = StringToGameId(uri.Fragment);
                        var pack = (IDataPack)new TesDataPack(localPath, null, gameId);
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

        public ICellManager GetCellManager(IAssetPack asset, IDataPack data, TemporalLoadBalancer loadBalancer)
        {
            return new TesCellManager((TesAssetPack)asset, (TesDataPack)data, loadBalancer);
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
