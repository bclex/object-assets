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
