//
//  NifManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class NifManager {
    let _asset: BsaMultiFile
    let _materialManager: MaterialManager 
    var _prefabContainerObj: GameObject? = nil
    var _nifFilePreloadTasks = [String : Task<Any>]()
    var _nifPrefabs = [String : GameObject]()
    let _markerLayer: Int

    init(asset: BsaMultiFile, materialManager: MaterialManager, markerLayer: Int = 0) {
        _asset = asset
        _materialManager = materialManager
        _markerLayer = markerLayer
    }

    public func instantiateNif(_ filePath: String) -> GameObject {
        if let prefab = _nifPrefabs[filePath] {
            return GameObject.instantiate(prefab)
        }
        ensurePrefabContainerObjectExists()
        // Load & cache the NIF prefab.
        let prefab = loadNifPrefabDontAddToPrefabCache(filePath: filePath)
        _nifPrefabs[filePath] = prefab
        // Instantiate the prefab.
        return GameObject.instantiate(prefab)
    }

    public func preloadNifFileAsync(_ filePath: String) {
        // If the NIF prefab has already been created we don't have to load the file again.
        guard _nifPrefabs[filePath] == nil else { return }
        // Start loading the NIF asynchronously if we haven't already started.
        var nifFileLoadingTask = _nifFilePreloadTasks[filePath]
        if nifFileLoadingTask == nil {
            nifFileLoadingTask = _asset.loadObjectInfoAsync(filePath: filePath)
            _nifFilePreloadTasks[filePath] = nifFileLoadingTask
        }
    }

    func ensurePrefabContainerObjectExists() {
        if _prefabContainerObj == nil {
            _prefabContainerObj = GameObject(name: "NIF Prefabs")
            _prefabContainerObj!.setActive(false)
        }
    }

    func loadNifPrefabDontAddToPrefabCache(filePath: String) -> GameObject {
        assert(_nifPrefabs[filePath] == nil, "Invalid parameter")
        preloadNifFileAsync(filePath)
        let file = _nifFilePreloadTasks[filePath]?.result as! NiFile
        _nifFilePreloadTasks.removeValue(forKey: filePath)
        // Start pre-loading all the NIF's textures.
        for niObject in file.blocks {
            guard let niSourceTexture = niObject as? NiSourceTexture, !niSourceTexture.fileName.isEmpty else {
                continue
            }
            _materialManager.textureManager.preloadTextureFileAsync(niSourceTexture.fileName)
        }
        let objBuilder = NifObjectBuilder(file: file, materialManager: _materialManager, markerLayer: _markerLayer)
        let prefab = objBuilder.buildObject()
        prefab.simdTransform = _prefabContainerObj!.simdTransform
        // Add LOD support to the prefab.
        // let lodComponent = prefab.addComponent<LODGroup>()
        // let lods = [LOD(0.015, prefab.getComponentsInChildren<Renderer>())]
        // lodComponent.setLODs(lods)
        return prefab
    }
}
