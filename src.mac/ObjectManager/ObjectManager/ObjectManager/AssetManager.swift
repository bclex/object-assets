//
//  AssetManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public enum EngineId: Int {
    case tes = 0
    case unk = 1
}

public protocol IRecord {
}

public protocol IAssetPack {
    func loadTextureInfoAsync(texturePath: String, cb: @escaping (Texture2DInfo?) -> Void)
    func loadTexture(texturePath: String, method: Int) -> Texture2D
    func preloadTextureAsync(texturePath: String)
    func loadObjectInfoAsync(filePath: String, cb: @escaping (Any) -> Void)
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
    static let statics: [IAssetManager] = [TesAssetManager()]
    
    static func getAssetManager(_ forEngine: EngineId) -> IAssetManager {
        return statics[forEngine.rawValue]
    }
}
