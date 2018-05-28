//
//  AssetManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public protocol IRecord {
}

public protocol IAssetPack {
}

public protocol IDataPack {
}

public protocol IAssetManager {
    func get(assetPack: URL?) -> IAssetPack?
    func get(dataPack: URL?) -> IDataPack?
}

public class AssetManager {
    static let Statics: [IAssetManager] = [TesAssetManager()]
    
    static func get(assetManager: EngineId) -> IAssetManager {
        return Statics[0];
    }
}
