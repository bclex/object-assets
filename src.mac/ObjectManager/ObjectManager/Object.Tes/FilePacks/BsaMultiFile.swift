//
//  BsaMultiFile.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class BsaMultiFile {
    public var packs = [BsaFile]()

    init(_ filePaths: [URL]?) {
        guard let filePaths = filePaths else {
            fatalError("Requires filePaths")
        }
        filePaths.filter { ["bsa", "ba2"].index(of: $0.pathExtension.lowercased()) != nil }
            .map { BsaFile($0.path) }
            .forEach { packs.append($0) }        
//        let file1 = loadFileData("trees\\treecottonwoodsu.spt")
//        debugPrint("file1", file1)
    }

    deinit {
        close()
    }

    public func close() {
        for pack in packs {
            pack.close()
        }
    }

    public func containsFile(_ filePath: String) -> Bool {
        return packs.contains { $0.containsFile(filePath) }
    }

    public func loadFileData(_ filePath: String) -> Data {
        guard let pack = packs.first(where: { $0.containsFile(filePath) }) else {
            fatalError("Could not find file '\(filePath)' in a BSA file.")
        }
        return pack.loadFileData(filePath: filePath)
    }
}
