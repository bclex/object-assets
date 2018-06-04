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

    init(filePaths: [URL], webPath: URL) {
        super.init(filePaths: filePaths)
        _textureManager = TextureManager(self)
        _materialManager = MaterialManager(texture: _textureManager)
        _nifManager = NifManager(self, material: _materialManager, marker: 0)
    }

    public func loadTexture(texturePath: String, method: Int = 0) -> Texture2D {
        return _textureManager.loadTexture(texturePath, method: method)
    }

    public func preloadTextureAsync(texturePath: String) {
        _textureManager.preloadTextureFileAsync(texturePath)
    }

    public func createObject(filePath: String) -> SCNNode {
        return _nifManager.InstantiateNif(filePath)
    }

    public func preloadObjectAsync(filePath: String) {
        _nifManager.PreloadNifFileAsync(filePath)
    }

    public override func containsFile(_ filePath: String) -> Bool {
        if _directory == nil {
            return super.containsFile(filePath)
        }
        if _directory != nil {
            // var path = Path.Combine(_directory, filePath.Replace("/", @"\"));
            // var r = File.Exists(path);
            // return r;
        }
        return false
    }

    public override func loadFileData(_ filePath: String) -> Data {
        if _directory == nil {
            return super.loadFileData(filePath)
        }
        //if _directory != nil {
        //    var path = Path.Combine(_directory, filePath);
        //    return File.ReadAllBytes(path);
        //}
        return nil
    }
}
