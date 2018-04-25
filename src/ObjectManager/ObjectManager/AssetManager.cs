using OA.Core;
using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace OA
{
    public interface IRecord
    {
    }

    public interface IAssetPack : IDisposable
    {
        Task<Texture2DInfo> LoadTextureInfoAsync(string texturePath);
        Texture2D LoadTexture(string texturePath, bool flipVertically = false);
        void PreloadTextureAsync(string texturePath);
        Task<object> LoadObjectInfoAsync(string filePath);
        GameObject CreateObject(string filePath);
        void PreloadObjectAsync(string filePath);
    }

    public interface IDataPack : IDisposable
    {
        ICellRecord FindExteriorCellRecord(Vector2i cellIndices);
        ICellRecord FindInteriorCellRecord(string cellName);
        ICellRecord FindInteriorCellRecord(Vector2i gridCoords);
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
    }

    public static class AssetManager
    {
        static IAssetManager[] Statics = new IAssetManager[2];

        public static IAssetManager GetAssetManager(EngineId engineId)
        {
            var manager = Statics[(int)engineId];
            if (manager != null)
                return manager;
            switch (engineId)
            {
                case EngineId.Tes: manager = Statics[(int)engineId] = (IAssetManager)Activator.CreateInstance(AssetReferences.TesAssetManagerType, new object[] { }); break;
                case EngineId.Valve: manager = Statics[(int)engineId] = (IAssetManager)Activator.CreateInstance(AssetReferences.ValveAssetManagerType, new object[] { }); break;
                default: throw new ArgumentOutOfRangeException("engineId", engineId.ToString());
            }
            return manager;
        }
    }
}