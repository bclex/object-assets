using OA.Tes.FilePacks;
using System;
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
        readonly BsaMultiFile r;
        readonly MaterialManager materialManager;
        GameObject prefabContainerObj;
        readonly Dictionary<string, Task<NiFile>> nifFilePreloadTasks = new Dictionary<string, Task<NiFile>>();
        readonly Dictionary<string, GameObject> nifPrefabs = new Dictionary<string, GameObject>();
        readonly int markerLayer;

        public NifManager(BsaMultiFile r, MaterialManager materialManager, int markerLayer)
        {
            this.r = r;
            this.materialManager = materialManager;
            this.markerLayer = markerLayer;
        }

        /// <summary>
        /// Instantiates a NIF file.
        /// </summary>
        public GameObject InstantiateNif(string filePath)
        {
            EnsurePrefabContainerObjectExists();
            // Get the prefab.
            GameObject prefab;
            if (!nifPrefabs.TryGetValue(filePath, out prefab))
            {
                // Load & cache the NIF prefab.
                prefab = LoadNifPrefabDontAddToPrefabCache(filePath);
                nifPrefabs[filePath] = prefab;
            }
            // Instantiate the prefab.
            return GameObject.Instantiate(prefab);
        }

        public void PreloadNifFileAsync(string filePath)
        {
            // If the NIF prefab has already been created we don't have to load the file again.
            if (nifPrefabs.ContainsKey(filePath)) { return; }
            Task<NiFile> nifFileLoadingTask;
            // Start loading the NIF asynchronously if we haven't already started.
            if (!nifFilePreloadTasks.TryGetValue(filePath, out nifFileLoadingTask))
            {
                nifFileLoadingTask = r.LoadNifAsync(filePath);
                nifFilePreloadTasks[filePath] = nifFileLoadingTask;
            }
        }

        private void EnsurePrefabContainerObjectExists()
        {
            if (prefabContainerObj == null)
            {
                prefabContainerObj = new GameObject("NIF Prefabs");
                prefabContainerObj.SetActive(false);
            }
        }

        private GameObject LoadNifPrefabDontAddToPrefabCache(string filePath)
        {
            Debug.Assert(!nifPrefabs.ContainsKey(filePath));
            PreloadNifFileAsync(filePath);
            var file = nifFilePreloadTasks[filePath].Result;
            nifFilePreloadTasks.Remove(filePath);
            // Start pre-loading all the NIF's textures.
            foreach (var anNiObject in file.blocks)
                if (anNiObject is NiSourceTexture)
                {
                    var anNiSourceTexture = (NiSourceTexture)anNiObject;
                    if (anNiSourceTexture.fileName != null && anNiSourceTexture.fileName != "")
                        materialManager.TextureManager.PreloadTextureFileAsync(anNiSourceTexture.fileName);
                }
            var objBuilder = new NifObjectBuilder(file, materialManager, markerLayer);
            var prefab = objBuilder.BuildObject();
            prefab.transform.parent = prefabContainerObj.transform;
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