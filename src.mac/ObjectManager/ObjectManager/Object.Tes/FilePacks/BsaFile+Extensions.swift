//
//  BsaFile+Extensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

extension BsaFile {
    func testContainsFile() {
        for file in _files {
            debugPrint(file.path, file.pathHash)
            guard containsFile(file.path) else {
                fatalError("Hash Invalid")
            }
            guard _filesByHash[hashFilePath(file.path)]!.contains(where: { $0.path == file.path }) else {
                fatalError("Hash Invalid")
            }
        }
    }

    func testLoadFileData() {
        for file in _files {
            debugPrint(file.path)
            _ = loadFileData(file: file)
        }
    }
}
