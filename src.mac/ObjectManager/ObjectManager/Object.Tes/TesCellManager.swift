//
//  TesCellManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class TesCellManager, ICellManager {    
    let _cellRadius: Int = 1 //4
    let _detailRadius: Int = 1 //3
    let _defaultLandTextureFilePath: String = "textures/_land_default.dds"

    let _asset: TesAssetPack
    let _data: TesDataPack
    let _loadBalancer: TemporalLoadBalancer
    //_cellObjects: Dictionary<Vector2i, InRangeCellInfo> = new Dictionary<Vector2i, InRangeCellInfo>();

    init(asset: TesAssetPack, data: TesDataPack, loadBalancer: TemporalLoadBalancer) {
        _asset = asset
        _data = data
        _loadBalancer = loadBalancer
    }
}
