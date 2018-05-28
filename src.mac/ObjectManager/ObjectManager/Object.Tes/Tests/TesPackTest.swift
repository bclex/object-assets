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
        let assetUrl = URL(string: "game://Morrowind/Morrowind.bsa")
        //let dataUrl = URL(string: "game://Morrowind/Morrowind.esm")
    
        let assetManager = AssetManager.get(assetManager: .Tes)
        Asset = assetManager.get(assetPack: assetUrl)
    }
}
