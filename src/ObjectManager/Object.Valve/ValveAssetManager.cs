using System;
using System.Threading.Tasks;
using OA.Core;

namespace OA.Valve
{
    public class ValveAssetManager : IGameAssetManager
    {
        public Task<IAssetPack> GetAssetPack(Uri uri)
        {
            throw new NotImplementedException();
        }

        public ICellManager GetCellManager(IAssetPack asset, IDataPack data, TemporalLoadBalancer temporalLoadBalancer)
        {
            throw new NotImplementedException();
        }

        public Task<IDataPack> GetDataPack(Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}
