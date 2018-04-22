using System;
using System.Reflection;
using System.Threading.Tasks;

namespace OA
{
    public interface IGameAssetManager
    {
        Task<IAssetPack> GetAssetPack(Uri uri);
    }

    static class AssetReferences
    {
        static IGameAssetManager[] Statics = new IGameAssetManager[2];
        internal static Assembly TesAssembly;
        internal static Assembly ValveAssembly;
        // tes
        internal static Type TesAssetManagerType;
        // valve
        internal static Type ValveAssetManagerType;

        static AssetReferences()
        {
            try
            {
                TesAssembly = Assembly.Load("Object.Tes");
                TesAssetManagerType = TesAssembly.GetType("OA.Tes.TesAssetManager");
            }
            catch { }
            try
            {
                ValveAssembly = Assembly.Load("Object.Valve");
                ValveAssetManagerType = ValveAssembly.GetType("OA.Valve.ValveAssetManager");
            }
            catch { }
        }

        // loading
        internal static IGameAssetManager GetManager(EngineId engineId)
        {
            var manager = Statics[(int)engineId];
            if (manager != null)
                return manager;
            switch (engineId)
            {
                case EngineId.Tes: manager = Statics[(int)engineId] = (IGameAssetManager)Activator.CreateInstance(TesAssetManagerType, new object[] { }); break;
                case EngineId.Valve: manager = Statics[(int)engineId] = (IGameAssetManager)Activator.CreateInstance(ValveAssetManagerType, new object[] { }); break;
                default: throw new ArgumentOutOfRangeException("engineId", engineId.ToString());
            }
            return manager;
        }
    }

    public static class AssetManager
    {
        public static async Task<IAssetPack> GetAssetPack(EngineId engineId, string uri) { return await GetAssetPack(engineId, new Uri(uri)); }
        public static Task<IAssetPack> GetAssetPack(EngineId engineId, Uri uri)
        {
            var manager = AssetReferences.GetManager(engineId);
            return manager.GetAssetPack(uri);
        }
    }
}