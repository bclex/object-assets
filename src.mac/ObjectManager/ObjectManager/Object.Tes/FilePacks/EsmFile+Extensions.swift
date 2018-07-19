//
//  EsmFile+Extensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

extension EsmFile {
    func process() {
        guard format != .TES3 else {
            let manyGroups = [groups!["STAT"]?.load(), nil]
            _MANYsById = Dictionary(uniqueKeysWithValues: manyGroups.compactMap { $0 }.flatMap { $0 }.compactMap { $0 as? IHaveEDID }
                .map { (key: $0.EDID, value: $0 as! Record) })
            _LTEXsById = Dictionary(uniqueKeysWithValues: groups!["LTEX"]!.load().map { $0 as! LTEXRecord }
                .map { (key: $0.INTV, value: $0) })
            let lands = groups!["LAND"]!.load().map { $0 as! LANDRecord }
            for land in lands {
                land.gridId = Vector3Int(Int(land.INTV.cellX), Int(land.INTV.cellY), 0)
            }
            _LANDsById = Dictionary(uniqueKeysWithValues: lands
                .map { (key: $0.gridId, value: $0) })
            let cells = groups!["CELL"]!.load().map { $0 as! CELLRecord }
            for cell in cells {
                cell.gridId = Vector3Int(Int(cell.XCLC!.gridX), Int(cell.XCLC!.gridY), !cell.isInterior ? 0 : -1)
            }
            _CELLsById = Dictionary(uniqueKeysWithValues: cells.filter { !$0.isInterior }
                .map { (key: $0.gridId, value: $0) })
            _CELLsByName = Dictionary(uniqueKeysWithValues: cells.filter { $0.isInterior }
                .map { (key: $0.EDID, value: $0) })
//            for a in _CELLsById.keys {
//                let b = _LANDsById[a]
//                if b?.VTEX != nil {
//                    debugPrint("\(a)")
//                }
//            }
            return
        }
        fatalError("NotImplemented")
        //_ltexsByEid = Groups["LTEX"].Load().Cast<LTEXRecord>().ToDictionary(x => x.EDID.Value)
    }

    func findLTEXRecord(index: Int64) -> LTEXRecord? {
        guard format != .TES3 else {
            return _LTEXsById[index]
        }
        fatalError("NotImplemented")
    }

    func findLANDRecord(cellId: Vector3Int) -> LANDRecord? {
        guard format != .TES3 else {
            return _LANDsById[cellId]
        }
        fatalError("NotImplemented")
    }

    public func findCellRecord(cellId: Vector3Int) -> CELLRecord? {
        guard format != .TES3 else {
            return _CELLsById[cellId]
        }
        fatalError("NotImplemented")
    }
    
    public func findCellRecordByName(world: Int, cellId: Int, cellName: String) -> CELLRecord? {
        guard format != .TES3 else {
            return _CELLsByName[cellName]
        }
        fatalError("NotImplemented")
    }
}
