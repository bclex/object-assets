//
//  TesAssetManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

class TesAssetManager : NSObject, IAssetManager {    
    func get(assetPack: URL?) -> IAssetPack? {
        return nil;
    }
    
    func get(dataPack: URL?) -> IDataPack? {
        return nil;
    }
    
    func getCellManager(asset: IAssetPack, data: IDataPack, loadBalancer: TemporalLoadBalancer) {
        
    }
}
