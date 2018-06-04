//
//  TesDataPack.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class TesDataPack: EsmFile, IDataPack {
    let _webPath: String

    init(filePath: String, string webPath: String, for game: gameId) {
        super.init(filePath != nil && File.Exists(filePath) ? filePath : nil, gameId)
        _webPath = webPath
    }
}