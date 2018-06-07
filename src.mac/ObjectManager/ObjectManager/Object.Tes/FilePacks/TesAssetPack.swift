//
//  TesAssetPack.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import SceneKit

public class TesAssetPack: BsaMultiFile, IAssetPack {
    let _textureManager: TextureManager
    let _materialManager: MaterialManager
    let _nifManager: NifManager

    init(_ filePaths: [URL], webPath: URL) {
        _textureManager = TextureManager(asset: self)
        _materialManager = MaterialManager(textureManager: _textureManager)
        _nifManager = NifManager(asset: self, materialManager: _materialManager, markerLayer: 0)
        super.init(filePaths)
    }

    public func loadTexture(texturePath: String, method: Int = 0) -> Texture2D {
        return _textureManager.loadTexture(texturePath, method: method)
    }

    public func preloadTextureAsync(texturePath: String) {
        _textureManager.preloadTextureFileAsync(texturePath)
    }

    public func createObject(filePath: String) -> GameObject {
        return _nifManager.instantiateNif(filePath: filePath)
    }

    public func preloadObjectAsync(filePath: String) {
        _nifManager.preloadNifFileAsync(filePath: filePath)
    }

    public override func containsFile(_ filePath: String) -> Bool {
        return super.containsFile(filePath)
    }

    public override func loadFileData(_ filePath: String) -> Data {
        return super.loadFileData(filePath)
    }
}
