//
//  TesDataPack.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class TesDataPack: EsmFile, IDataPack {
    let _webPath: String?

    init(_ filePath: String?, string webPath: String?, for game: GameId) {
        let fileManager = FileManager.default
        _webPath = webPath
        super.init(filePath: filePath != nil && fileManager.fileExists(atPath: filePath!) ? filePath : nil, for: game)
    }
}
