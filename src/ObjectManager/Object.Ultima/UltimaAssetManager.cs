using OA.Core;
using OA.Ultima.FilePacks;
using OA.Ultima.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OA.Ultima
{
    public class UltimaAssetManager : IAssetManager
    {
        public Task<IAssetPack> GetAssetPack(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "game":
                    {
                        var localPath = uri.LocalPath.Substring(1);
                        var filePath = FileManager.GetFilePath(localPath);
                        var pack = (IAssetPack)new UltimaAssetPack(filePath, null);
                        return Task.FromResult(pack);
                    }
                case "file":
                    {
                        var localPath = uri.LocalPath;
                        var pack = (IAssetPack)new UltimaAssetPack(localPath, null);
                        return Task.FromResult(pack);
                    }
                default:
                    {
                        var pack = (IAssetPack)new UltimaAssetPack(null, null);
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
                        var filePath = FileManager.GetFilePath(localPath);
                        var pack = (IDataPack)new UltimaDataPack(filePath, null);
                        return Task.FromResult(pack);
                    }
                case "file":
                    {
                        var localPath = uri.LocalPath;
                        if (!File.Exists(localPath))
                            throw new IndexOutOfRangeException("file not found");
                        var pack = (IDataPack)new UltimaDataPack(localPath, null);
                        return Task.FromResult(pack);
                    }
                default:
                    {
                        var pack = (IDataPack)new UltimaDataPack(null, null);
                        return Task.FromResult(pack);
                    }
            }
        }

        public ICellManager GetCellManager(IAssetPack asset, IDataPack data, TemporalLoadBalancer loadBalancer)
        {
            return new UltimaCellManager((UltimaAssetPack)asset, (UltimaDataPack)data, loadBalancer);
        }
    }
}
