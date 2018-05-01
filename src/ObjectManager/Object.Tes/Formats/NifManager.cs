using OA.Tes.FilePacks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace OA.Tes.Formats
{
    /// <summary>
    /// Manages loading and instantiation of NIF models.
    /// </summary>
    public class NifManager
    {
        readonly BsaMultiFile _asset;
        readonly MaterialManager _materialManager;
        GameObject _prefabContainerObj;
        readonly Dictionary<string, Task<object>> _nifFilePreloadTasks = new Dictionary<string, Task<object>>();
        readonly Dictionary<string, GameObject> _nifPrefabs = new Dictionary<string, GameObject>();
        readonly int _markerLayer;

        public NifManager(BsaMultiFile asset, MaterialManager materialManager, int markerLayer)
        {
            _asset = asset;
            _materialManager = materialManager;
            _markerLayer = markerLayer;
        }

        /// <summary>
        /// Instantiates a NIF file.
        /// </summary>
        public GameObject InstantiateNif(string filePath)
        {
            EnsurePrefabContainerObjectExists();
            // Get the prefab.
            if (!_nifPrefabs.TryGetValue(filePath, out GameObject prefab))
            {
                // Load & cache the NIF prefab.
                prefab = LoadNifPrefabDontAddToPrefabCache(filePath);
                _nifPrefabs[filePath] = prefab;
            }
            // Instantiate the prefab.
            return GameObject.Instantiate(prefab);
        }

        public void PreloadNifFileAsync(string filePath)
        {
            // If the NIF prefab has already been created we don't have to load the file again.
            if (_nifPrefabs.ContainsKey(filePath)) return;
            // Start loading the NIF asynchronously if we haven't already started.
            if (!_nifFilePreloadTasks.TryGetValue(filePath, out Task<object> nifFileLoadingTask))
            {
                nifFileLoadingTask = _asset.LoadObjectInfoAsync(filePath);
                _nifFilePreloadTasks[filePath] = nifFileLoadingTask;
            }
        }

        private void EnsurePrefabContainerObjectExists()
        {
            if (_prefabContainerObj == null)
            {
                _prefabContainerObj = new GameObject("NIF Prefabs");
                _prefabContainerObj.SetActive(false);
            }
        }

        private GameObject LoadNifPrefabDontAddToPrefabCache(string filePath)
        {
            Debug.Assert(!_nifPrefabs.ContainsKey(filePath));
            PreloadNifFileAsync(filePath);
            var file = (NiFile)_nifFilePreloadTasks[filePath].Result;
            _nifFilePreloadTasks.Remove(filePath);
            // Start pre-loading all the NIF's textures.
            foreach (var niObject in file.Blocks)
                if (niObject is NiSourceTexture niSourceTexture)
                    if (!string.IsNullOrEmpty(niSourceTexture.FileName))
                        _materialManager.TextureManager.PreloadTextureFileAsync(niSourceTexture.FileName);
            var objBuilder = new NifObjectBuilder(file, _materialManager, _markerLayer);
            var prefab = objBuilder.BuildObject();
            prefab.transform.parent = _prefabContainerObj.transform;
            // Add LOD support to the prefab.
            var LODComponent = prefab.AddComponent<LODGroup>();
            var LODs = new LOD[1]
            {
                new LOD(0.015f, prefab.GetComponentsInChildren<Renderer>())
            };
            LODComponent.SetLODs(LODs);
            return prefab;
        }
    }
}