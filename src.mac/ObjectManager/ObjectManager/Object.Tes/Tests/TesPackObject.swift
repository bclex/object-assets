//
//  TesDataPackTest.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import simd

public class TesPackTest: GameSegment {
    
    var Asset: IAssetPack? = nil
    var Data: IDataPack? = nil
    
    public init() {
    }
    
    public func start(player: GameObject) {
        let assetUrl = URL(string: "game://Morrowind/Morrowind.bsa")
        let dataUrl = URL(string: "game://Morrowind/Morrowind.esm")
//        let assetUrl = URL(string: "game://Oblivion/Oblivion*")
//        let dataUrl = URL(string: "game://Oblivion/Oblivion.esm")
//        let assetUrl = URL(string: "game://SkyrimVR/Skyrim*")
//        let dataUrl = URL(string: "game://SkyrimVR/Skyrim.esm")
//        let assetUrl = URL(string: "game://Fallout4/Fallout4*")
//        let dataUrl = URL(string: "game://Fallout4/Fallout4.esm")
//        let assetUrl = URL(string: "game://Fallout4VR/Fallout4*")
//        let dataUrl = URL(string: "game://Fallout4VR/Fallout4.esm")
        
        let assetManager = AssetManager.getAssetManager(.tes)
        Asset = assetManager.getAssetPack(assetUrl)
        Data = assetManager.getDataPack(dataUrl)
        
        makeObject("meshes/x/ex_common_balcony_01.nif")
    }
    
    public func makeObject(_ path: String) {
        let obj = Asset!.createObject(path: path)
        debugPrint("obj: \(obj)")
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
    }
}
