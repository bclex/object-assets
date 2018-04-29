using OA.Ultima.UI;
using System;
using UnityEngine;

namespace OA.Ultima
{
    public class UltimaEngine : BaseEngine
    {
        public UltimaUIManager UIManager;

        public UltimaEngine(IAssetManager assetManager, Uri asset, Uri data, UltimaUIManager uiManager)
            : this(assetManager, assetManager.GetAssetPack(asset).Result, assetManager.GetDataPack(data).Result, uiManager) { }
        public UltimaEngine(IAssetManager assetManager, IAssetPack asset, IDataPack data, UltimaUIManager uiManager)
        : base(assetManager, asset, data)
        {
            UIManager = uiManager;
            //UIManager.Active = true;
        }

        #region Player Spawn

        protected override GameObject CreatePlayer(GameObject playerPrefab, Vector3 position, out GameObject playerCamera)
        {
            var player = base.CreatePlayer(playerPrefab, position, out playerCamera);
            return player;
        }

        #endregion
    }
}
