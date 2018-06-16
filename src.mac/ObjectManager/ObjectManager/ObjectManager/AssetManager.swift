//
//  AssetManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public enum EngineId: Int {
    case tes = 0, unk
}

public protocol IRecord {
}

public protocol IAssetPack {
    func loadTextureInfoAsync(texturePath: String) -> Task<Texture2DInfo?>
    func loadTexture(texturePath: String, method: Int) -> Texture2D
    func preloadTextureAsync(texturePath: String)
    func loadObjectInfoAsync(filePath: String) -> Task<Any>
    func createObject(filePath: String) -> GameObject
    func preloadObjectAsync(filePath: String)
}

public protocol IDataPack {
}

public protocol IAssetManager {
    func getAssetPack(_ url: URL?) -> IAssetPack?
    func getDataPack(_ url: URL?) -> IDataPack?
    func getCellManager(asset: IAssetPack, data: IDataPack, loadBalancer: TemporalLoadBalancer) -> ICellManager?
}

public class AssetManager {
    static let statics: [IAssetManager] = [] //TesAssetManager()]
    
    static func getAssetManager(_ engine: EngineId) -> IAssetManager {
        return statics[engine.rawValue]
    }
}
