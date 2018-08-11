//
//  TesEngineTest.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import simd

public class TesEngineTest: GameSegment {
    
    var Asset: IAssetPack? = nil
    var Data: IDataPack? = nil
    var Engine: BaseEngine!
    
    public init() {
    }
    
    public func start(player: GameObject) {
        let assetUrl = URL(string: "game://Morrowind/Morrowind.bsa")
        let dataUrl = URL(string: "game://Morrowind/Morrowind.esm")
    
        let assetManager = AssetManager.getAssetManager(.tes)
        Asset = assetManager.getAssetPack(assetUrl)
        Data = assetManager.getDataPack(dataUrl)
        
        Engine = BaseEngine(assetManager: assetManager, asset: Asset!, data: Data!)
//        Engine.spawnPlayer(player: player, position: float3(-137.94, 2.30, -1037.6)) // gridId: int3(-2, -9, 0), 
        
        let newX = 23 * ConvertUtils.exteriorCellSideLengthInMeters
        let newZ = (-5 * ConvertUtils.exteriorCellSideLengthInMeters) + 100
        Engine.spawnPlayer(player: player, position: float3(newX, 2.30, newZ))
    }
    
    public func onDestroy() {
        if Asset != nil {
            Asset!.close()
            Asset = nil
        }
        if Data != nil {
            Data!.close()
            Data = nil
        }
    }
    
    public func update() {
//        Engine.update()
    }
}
