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
    var _textureManager: TextureManager? = nil
    var _materialManager: MaterialManager? = nil
    var _nifManager: NifManager? = nil

    convenience init(_ filePath: URL) {
        self.init([filePath])
    }
    init(_ filePaths: [URL]) {
        super.init(filePaths)
        _textureManager = TextureManager(asset: self)
        _materialManager = MaterialManager(textureManager: _textureManager!)
        _nifManager = NifManager(asset: self, materialManager: _materialManager!, markerLayer: 0)
    }

    public func loadTexture(texturePath: String, method: Int = 0) -> Texture2D {
        return _textureManager!.loadTexture(texturePath, method: method)
    }

    public func preloadTextureAsync(texturePath: String) {
        _textureManager!.preloadTextureFileAsync(texturePath)
    }

    public func createObject(filePath: String) -> GameObject {
        return _nifManager!.instantiateNif(filePath)
    }

    public func preloadObjectAsync(filePath: String) {
        _nifManager!.preloadNifFileAsync(filePath)
    }

    public override func containsFile(_ filePath: String) -> Bool {
        return super.containsFile(filePath)
    }

    public override func loadFileData(_ filePath: String) -> Data {
        return super.loadFileData(filePath)
    }
}
