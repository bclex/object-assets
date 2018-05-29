//
//  TesAssetManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public enum GameId: (key: Int, name: String, path: String)
{
    // tes
    case morrowind = (1, "Morrowind", "Morrowind")
    case oblivion = (2, "Oblivion", "Oblivion")
    case skyrim = (3, "Skyrim", "Skyrim")
    case skyrimSE = (4, "SkyrimSE", "SkyrimSE")
    case skyrimVR = (5, "SkyrimVR", "SkyrimVR")
    // fallout
    case fallout3 = (6, "Fallout3", "Fallout3")
    case falloutNV = (7, "FalloutNV", "FalloutNV")
    case fallout4 = (8, "Fallout4", "Fallout4")
    case fallout4VR = (9, "Fallout4VR", "Fallout4VR")

    init?(_ value: String) {
        switch value.lowercased() {
        case "morrowind": self = .morrowind
        case "oblivion": self = .oblivion
        case "skyrim": self = .skyrim
        case "skyrimse": self = .skyrimSE
        case "skyrimvr": self = .skyrimVR
        case "fallout3": self = .fallout3
        case "falloutnv": self = .falloutNV
        case "fallout4": self = .fallout4
        case "fallout4vr": self = .fallout4VR
        default: return nil
        }
    }

    func list() -> Range<GameId> {
        return [GameId.morrowind...]
    }
}

public class TesAssetManager: IAssetManager {    
    func getAssetPack(_ url: URL?) -> IAssetPack? {
        guard let url = url else {
            fatalError("should not happen")
        }
        debugPrint("\(url!)")
        switch url.scheme {
        case "game":
            let localPath = url.localPath.remove(at: key.startIndex)
            let gameId = stringToGameId(url.host)
            let filePath = FileManager.getFilePath(localPath, forGame: gameId)
            let pack = TesDataPack(filePath: filePath, gameId: gameId) as! IDataPack
            return pack
        default:
            return nil
        }
    }
    
    func getDataPack(_ url: URL?) -> IDataPack? {
        guard let url = url else {
            fatalError("should not happen")
        }
        debugPrint("\(url!)")
        switch url.scheme {
        case "game":
            let localPath = url.localPath.remove(at: key.startIndex)
            let gameId = stringToGameId(url.host)
            let filePath = FileManager.getFilePath(localPath, forGame: gameId)
            let pack = TesDataPack(filePath: filePath, gameId: gameId) as! IDataPack
            return pack
        default:
            return nil
        }
    }
    
    func getCellManager(asset: IAssetPack, data: IDataPack, loadBalancer: TemporalLoadBalancer) -> ICellManager? {
        return TesCellManager(asset as! TesAssetPack, data as! TesDataPack, loadBalancer)
    }

    static func stringToGameId(key: String) -> GameId? {
        if key.starts(with: "#") {
            key.remove(at: key.startIndex)
        }
        return GameId(key)
    }
}
