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
            var pack = (IAssetPack)new UltimaAssetPack();
            return Task.FromResult(pack);
        }

        public Task<IDataPack> GetDataPack(Uri uri)
        {
            var pack = (IDataPack)new UltimaDataPack(0);
            return Task.FromResult(pack);
        }

        public ICellManager GetCellManager(IAssetPack asset, IDataPack data, TemporalLoadBalancer loadBalancer)
        {
            return new UltimaCellManager((UltimaAssetPack)asset, (UltimaDataPack)data, loadBalancer);
        }
    }
}
