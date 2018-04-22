﻿using OA.Core;
using System;
using System.Collections;
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
        Texture2D LoadTexture(string texturePath, bool flipVertically = false);
        void PreloadTextureAsync(string path);
        GameObject CreateObject(string path);
        void PreloadObjectAsync(string path);
    }

    public interface IDataPack : IDisposable
    {
        IRecord FindExteriorCellRecord(Vector2i cellIndices);
        IRecord FindInteriorCellRecord(string cellName);
        IRecord FindInteriorCellRecord(Vector2i gridCoords);
    }

    public interface ICellManager
    {
        InRangeCellInfo StartCreatingExteriorCell(Vector2i cellIndices);
    }

    public interface IGameAssetManager
    {
        Task<IAssetPack> GetAssetPack(Uri uri);
        Task<IDataPack> GetDataPack(Uri uri);
        ICellManager GetCellManager(IAssetPack asset, IDataPack data, TemporalLoadBalancer temporalLoadBalancer);
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
        public static TemporalLoadBalancer TemporalLoadBalancer = new TemporalLoadBalancer();

        public static async Task<IAssetPack> GetAssetPack(EngineId engineId, string uri) { return await GetAssetPack(engineId, new Uri(uri)); }

        public static void WaitForTask(IEnumerator taskCoroutine)
        {
            TemporalLoadBalancer.WaitForTask(taskCoroutine);
        }

        public static Task<IAssetPack> GetAssetPack(EngineId engineId, Uri uri)
        {
            var manager = AssetReferences.GetManager(engineId);
            return manager.GetAssetPack(uri);
        }

        public static async Task<IDataPack> GetDataPack(EngineId engineId, string uri) { return await GetDataPack(engineId, new Uri(uri)); }
        public static Task<IDataPack> GetDataPack(EngineId engineId, Uri uri)
        {
            var manager = AssetReferences.GetManager(engineId);
            return manager.GetDataPack(uri);
        }

        public static ICellManager GetCellManager(EngineId engineId, IAssetPack asset, IDataPack data)
        {
            var manager = AssetReferences.GetManager(engineId);
            return manager.GetCellManager(asset, data, TemporalLoadBalancer);
        }
    }
}