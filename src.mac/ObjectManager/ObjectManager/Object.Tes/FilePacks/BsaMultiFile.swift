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

    init(filePaths: [URL]?) {
        guard let filePaths = filePaths else {
            fatalError("name me")
        }
        filePaths.filter { [".bsa", ".ba2"].contains { $0.pathExtension.lowercased() } }
            .map { BsaFile($0) }
            .forEach { packs.append($0) }
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
        return pack.loadFileData(filePath)
    }
}
