//
//  TesAssetPack.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class TesAssetPack: BsaMultiFile, IAssetPack
{
    let _textureManager: TextureManager
    let _materialManager: MaterialManager
    let _nifManager: NifManager

    init(filePaths: [URL], webPath: URL)
    {
        super.init(filePaths: filePaths)
        _textureManager = TextureManager(self)
        _materialManager = MaterialManager(texture: _textureManager)
        _nifManager = NifManager(self, material: _materialManager, marker: 0)
    }

    public func loadTexture(texturePath: String, method: Int = 0) -> Texture2D {
        return _textureManager.loadTexture(texturePath, method: method)
    }

    public preloadTextureAsync(texturePath: String) {
        _textureManager.preloadTextureFileAsync(texturePath)
    }

    public createObject(string filePath) -> GameObject {
        return _nifManager.InstantiateNif(filePath);
    }

    public void PreloadObjectAsync(string filePath)
    {
        _nifManager.PreloadNifFileAsync(filePath);
    }

    public override bool ContainsFile(string filePath)
    {
        if (_directory == null && _webPath == null)
            return base.ContainsFile(filePath);
        if (_directory != null)
        {
            var path = Path.Combine(_directory, filePath.Replace("/", @"\"));
            var r = File.Exists(path);
            return r;
        }
        return false;
    }

    public override byte[] LoadFileData(string filePath)
    {
        if (_directory == null && _webPath == null)
            return base.LoadFileData(filePath);
        if (_directory != null)
        {
            var path = Path.Combine(_directory, filePath);
            return File.ReadAllBytes(path);
        }
        return null;
    }
}