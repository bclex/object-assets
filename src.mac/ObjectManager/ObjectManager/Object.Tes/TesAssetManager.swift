//
//  TesAssetManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public enum GameId: Int, CustomStringConvertible {
    // tes
    case morrowind = 0
    case oblivion
    case skyrim, skyrimSE, skyrimVR
    // fallout
    case fallout3
    case falloutNV
    case fallout4, fallout4VR

    public var description: String {
        switch self {
        case .morrowind: return "morrowind"
        case .oblivion: return "oblivion"
        case .skyrim: return "skyrim"
        case .skyrimSE: return "skyrimSE"
        case .skyrimVR: return "skyrimVR"
        case .fallout3: return "fallout3"
        case .falloutNV: return "falloutNV"
        case .fallout4: return "fallout4"
        case .fallout4VR: return "fallout4VR"
        default: return "?"
        }
    }
    
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

    //func list() -> Range<GameId> {
    //    return [GameId.morrowind...]
    //}
}

public class TesAssetManager: IAssetManager {    
    public func getAssetPack(_ url: URL?) -> IAssetPack? {
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
    
    public func getDataPack(_ url: URL?) -> IDataPack? {
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
    
    public func getCellManager(asset: IAssetPack, data: IDataPack, loadBalancer: TemporalLoadBalancer) -> ICellManager? {
        return TesCellManager(asset as! TesAssetPack, data as! TesDataPack, loadBalancer)
    }

    static func stringToGameId(key: String) -> GameId? {
        if key.starts(with: "#") {
            key.remove(at: key.startIndex)
        }
        return GameId(key)
    }
}
