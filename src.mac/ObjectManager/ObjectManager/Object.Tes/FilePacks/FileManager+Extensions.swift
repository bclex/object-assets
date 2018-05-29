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

    static lazy var _fileDirectories: [GameId : URL] = {
        // var game = TesSettings.Game;
        debugPrint("TES Installation(s):")
        // if (game.DataDirectory != null && Directory.Exists(game.DataDirectory))
        // {
        //     var gameId = (GameId)Enum.Parse(typeof(GameId), game.GameId);
        //     _fileDirectories.Add(gameId, game.DataDirectory); Utils.Log($"Settings: {game.DataDirectory}");
        //     _isDataPresent = true;
        // }
        // else
        var r = [GameId : URL]()
        let fileManager = FileManager.default
        let documentsURL = try fileManager.url(for: .documentDirectory, in: .userDomainMask, appropriateFor: nil, create: false)
        for key in _knownkeys {
            let url = documentsURL.appendingPathComponent(key.value)
            let isDirectory: ObjCBool
            guard fileManager.fileExists(atPath: url.path, isDirectory: &isDirectory), isDirectory.boolValue else {
                continue
            }
            r[gameId] = dataPath
            debugPrint("GameId: \(gameId)")
        }
        return r
    }

    public func getFilePath(_ path: String, forGame: GameId) -> URL?
    {
        guard let fileDirectory = _fileDirectories[gameId] else {
            return nil
        }
        path = fileDirectory.appendingPathComponent(path)
        return fileExists(atPath: path) ? path : nil
    }

    public func getFilePaths(searchPattern: String, forGame: GameId) -> [URL]?
    {
        guard let fileDirectory = _fileDirectories[gameId] else {
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
