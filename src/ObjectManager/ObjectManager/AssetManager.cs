using OA.Core;
using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace OA
{
    public enum EngineId
    {
        Tes = 0,
        Valve,
        Ultima,
    }

    public interface IRecord
    {
    }

    public interface IAssetPack : IDisposable
    {
        Task<Texture2DInfo> LoadTextureInfoAsync(string texturePath);
        Texture2D LoadTexture(string texturePath, int method = 0);
        void PreloadTextureAsync(string texturePath);
        Task<object> LoadObjectInfoAsync(string filePath);
        GameObject CreateObject(string filePath);
        void PreloadObjectAsync(string filePath);
    }

    public interface IDataPack : IDisposable
    {
        ICellRecord FindCellRecord(Vector3Int cellId);
        //ICellRecord FindInteriorCellRecord(string cellId);
    }

    public interface IAssetManager
    {
        Task<IAssetPack> GetAssetPack(Uri uri);
        Task<IDataPack> GetDataPack(Uri uri);
        ICellManager GetCellManager(IAssetPack asset, IDataPack data, TemporalLoadBalancer loadBalancer);
    }

    static class AssetReferences
    {
        internal static Assembly TesAssembly;
        internal static Assembly ValveAssembly;
        internal static Assembly UltimaAssembly;
        // tes
        internal static Type TesAssetManagerType;
        // valve
        internal static Type ValveAssetManagerType;
        // ultima
        internal static Type UltimaAssetManagerType;

        static AssetReferences()
        {
            try
            {
                TesAssembly = Assembly.Load("Object.Tes");
                TesAssetManagerType = TesAssembly.GetType("OA.Tes.TesAssetManager");
            }
            catch { }
            //try
            //{
            //    ValveAssembly = Assembly.Load("Object.Valve");
            //    ValveAssetManagerType = ValveAssembly.GetType("OA.Valve.ValveAssetManager");
            //}
            //catch { }
            //try
            //{
            //    UltimaAssembly = Assembly.Load("Object.Ultima");
            //    UltimaAssetManagerType = UltimaAssembly.GetType("OA.Ultima.UltimaAssetManager");
            //}
            //catch { }
        }
    }

    public static class AssetManager
    {
        static IAssetManager[] Statics = new IAssetManager[3];

        public static IAssetManager GetAssetManager(EngineId engineId)
        {
            var manager = Statics[(int)engineId];
            if (manager != null)
                return manager;
            switch (engineId)
            {
                case EngineId.Tes: manager = Statics[(int)engineId] = (IAssetManager)Activator.CreateInstance(AssetReferences.TesAssetManagerType, new object[] { }); break;
                case EngineId.Valve: manager = Statics[(int)engineId] = (IAssetManager)Activator.CreateInstance(AssetReferences.ValveAssetManagerType, new object[] { }); break;
                case EngineId.Ultima: manager = Statics[(int)engineId] = (IAssetManager)Activator.CreateInstance(AssetReferences.UltimaAssetManagerType, new object[] { }); break;
                default: throw new ArgumentOutOfRangeException("engineId", engineId.ToString());
            }
            return manager;
        }
    }
}