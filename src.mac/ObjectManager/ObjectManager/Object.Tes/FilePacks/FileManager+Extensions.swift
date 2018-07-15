//
//  FileManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

extension FileManager {
    static let _knownkeys = [
        GameId.oblivion : "Oblivion",
        GameId.skyrim : "Skyrim",
        GameId.fallout3 : "Fallout 3",
        GameId.falloutNV : "Fallout NV",
        GameId.morrowind : "Morrowind",
        GameId.fallout4 : "Fallout4",
        GameId.skyrimSE : "Skyrim SE",
        GameId.fallout4VR : "Fallout4 VR",
        GameId.skyrimVR : "Skyrim VR"
    ]

    static var _fileDirectories: [GameId : URL] = {
        var game = BaseSettings.game
        var r = [GameId : URL]()
        let fileManager = FileManager.default
        var isDirectory: ObjCBool = false
        guard game.dataDirectory == nil || !fileManager.fileExists(atPath: game.dataDirectory!, isDirectory: &isDirectory) || !isDirectory.boolValue else {
            let key = GameId.morrowind
            r[key] = URL(string: game.dataDirectory!)
            debugPrint("Game: \(key)")
            return r
        }
        //debugPrint("TES Installation(s):")
        let documentsURL = try! fileManager.url(for: .documentDirectory, in: .userDomainMask, appropriateFor: nil, create: false)
        for x in _knownkeys {
            let url = documentsURL.appendingPathComponent(x.value)
            guard fileManager.fileExists(atPath: url.path, isDirectory: &isDirectory), isDirectory.boolValue else {
                continue
            }
            r[x.key] = url
            debugPrint("Game: \(x.key)")
        }
        return r
    }()

    public func getFilePath(_ path: String, for game: GameId) -> URL? {
        guard let fileDirectory = FileManager._fileDirectories[game] else {
            return nil
        }
        let url = fileDirectory.appendingPathComponent(path)
        return fileExists(atPath: url.path) ? url : nil
    }

    public func getFilePaths(searchPattern: String, for game: GameId) -> [URL]? {
        guard let fileDirectory = FileManager._fileDirectories[game] else {
            return nil
        }
        let files: [URL]
        do {
            files = try FileManager.default.contentsOfDirectory(at: fileDirectory,
                includingPropertiesForKeys: [],
                options: [.skipsSubdirectoryDescendants, .skipsPackageDescendants, .skipsHiddenFiles])
        }
        catch {
            fatalError("define me later \(fileDirectory)")
        }
        return files
    }
}
