//
//  TesDataPackTest.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class TesPackTest {
    static var Asset: IAssetPack? = nil
    static var Data: IDataPack? = nil
    
    public static func start() {
        //let assetUrl = URL(string: "game://Morrowind/Morrowind.bsa")
        let dataUrl = URL(string: "game://Morrowind/Morrowind.esm")
    
        //let assetUrl = URL(string: "game://Oblivion/Oblivion*")
        //let dataUrl = URL(string: "game://Oblivion/Oblivion.esm")
        
        //let assetUrl = URL(string: "game://SkyrimVR/Skyrim*")
        //let dataUrl = URL(string: "game://SkyrimVR/Skyrim.esm")
        
        //let assetUrl = URL(string: "game://Fallout4/Fallout4*")
        //let dataUrl = URL(string: "game://Fallout4/Fallout4.esm")
        
        //let assetUrl = URL(string: "game://Fallout4VR/Fallout4*")
        //let dataUrl = URL(string: "game://Fallout4VR/Fallout4.esm")
        
        let assetManager = AssetManager.getAssetManager(.tes)
        //Asset = assetManager.getAssetPack(assetUrl)
        Data = assetManager.getDataPack(dataUrl)
    }
}
