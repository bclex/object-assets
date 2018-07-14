//
//  TesDataPack.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class TesDataPack: EsmFile, IDataPack {
    override init(_ filePath: URL?, for game: GameId) {
        let fileManager = FileManager.default
        super.init(filePath != nil && fileManager.fileExists(atPath: filePath!.path) ? filePath : nil, for: game)
    }
    
    public func findCellRecord(_ cellId: Vector3Int) -> ICellRecord? {
        return super.findCellRecord(cellId: cellId)
    }
}
