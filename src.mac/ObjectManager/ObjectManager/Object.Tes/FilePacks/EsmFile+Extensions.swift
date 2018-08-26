//
//  EsmFile+Extensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import simd

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
                land.gridId = int3(Int32(land.INTV.cellX), Int32(land.INTV.cellY), 0)
            }
            _LANDsById = Dictionary(uniqueKeysWithValues: lands
                .map { (key: $0.gridId, value: $0) })
            let cells = groups!["CELL"]!.load().map { $0 as! CELLRecord }
            for cell in cells {
                cell.gridId = int3(Int32(cell.XCLC!.gridX), Int32(cell.XCLC!.gridY), !cell.isInterior ? 0 : -1)
            }
            _CELLsById = Dictionary(uniqueKeysWithValues: cells.filter { !$0.isInterior }
                .map { (key: $0.gridId, value: $0) })
            _CELLsByName = Dictionary(uniqueKeysWithValues: cells.filter { $0.isInterior }
                .map { (key: $0.EDID, value: $0) })
            // temp
            for a in _CELLsById.keys {
                let b = _LANDsById[a]
                if b?.VTEX != nil {
                    debugPrint("\(a)")
                }
            }
            return
        }
        let wrldsByLabel = groups!["WRLD"]!.groupsByLabel!
        _WRLDsById = Dictionary(uniqueKeysWithValues: groups!["WRLD"]!.load().map { $0 as! WRLDRecord }
            .map { (key: $0.id, value: (x: $0, y: wrldsByLabel[$0.id]!)) })
        _LTEXsByEid = Dictionary(uniqueKeysWithValues: groups!["LTEX"]!.load().map { $0 as! LTEXRecord }
            .map { (key: $0.EDID, value: $0) })
    }

    func findLTEXRecord(index: Int64) -> LTEXRecord? {
        guard format != .TES3 else {
            return _LTEXsById[index]
        }
        fatalError("NotImplemented")
    }

    func findLANDRecord(cellId: int3) -> LANDRecord? {
        guard format != .TES3 else {
            return _LANDsById[cellId]
        }
        let world = _WRLDsById[UInt32(cellId.z)]!
        for wrld in world.y {
            for cellBlock in wrld.ensureWrldAndCell(cellId: cellId)! {
                if let land = cellBlock.LANDsById[cellId] {
                     return land
                }
            }
        }
        return nil
    }

    public func findCellRecord(cellId: int3) -> CELLRecord? {
        guard format != .TES3 else {
            return _CELLsById[cellId]
        }
        let world = _WRLDsById[UInt32(cellId.z)]!
        for wrld in world.y {
            for cellBlock in wrld.ensureWrldAndCell(cellId: cellId)! {
                if let cell = cellBlock.CELLsById[cellId] {
                    return cell
                }
            }
        }
        return nil
    }
    
    public func findCellRecordByName(world: Int32, cellId: Int, cellName: String) -> CELLRecord? {
        guard format != .TES3 else {
            return _CELLsByName[cellName]
        }
        fatalError("NotImplemented")
    }
}

extension RecordGroup {    
    public func ensureWrldAndCell(cellId: int3) -> [RecordGroup]? {
        let cellBlockX = Int16(cellId.x >> 5)
        let cellBlockY = Int16(cellId.y >> 5)
        let cellBlockId = UInt32((cellBlockY << 8) | cellBlockX)
        load()
        guard let cellBlocks = groupsByLabel![cellBlockId] else {
            return nil
        }
        return cellBlocks.compactMap { $0.ensureCell(cellId) }
    }
    
    func ensureCell(_ cellId: int3) -> RecordGroup? {
        if _ensureCELLsByLabel == nil {
            _ensureCELLsByLabel = Set()
        }
//        let cellBlockX = Int16(cellId.x >> 5)
//        let cellBlockY = Int16(cellId.y >> 5)
        let cellSubBlockX = Int16(cellId.x >> 3)
        let cellSubBlockY = Int16(cellId.y >> 3)
        let cellSubBlockId = UInt32((cellSubBlockY << 8) | cellSubBlockX)
        guard !_ensureCELLsByLabel!.contains(cellSubBlockId) else {
            return self
        }
        load()
        if CELLsById == nil {
            CELLsById = [int3 : CELLRecord]()
        }
        if LANDsById == nil && cellId.z >= 0 {
            LANDsById = [int3 : LANDRecord]()
        }
        guard let cellSubBlocks = groupsByLabel![cellSubBlockId] else {
            return nil
        }
        // find cell
        let cellSubBlock = cellSubBlocks.first!
        cellSubBlock.load(loadAll: true)
        for cell in (cellSubBlock.records.map { $0 as! CELLRecord }) {
            cell.gridId = int3(Int32(cell.XCLC!.gridX), Int32(cell.XCLC!.gridY), !cell.isInterior ? cellId.z : -1)
            CELLsById[cell.gridId] = cell
            // find children
            if let cellChildren = cellSubBlock.groupsByLabel![cell.id] {
                let cellChild = cellChildren.first!
                let cellTemporaryChildren = (cellChild.groups!.filter { $0.headers.first!.groupType == .cellTemporaryChildren }).first!
                for land in (cellTemporaryChildren.records.map { $0 as! LANDRecord }) {
                    land.gridId = int3(cell.XCLC!.gridX, cell.XCLC!.gridY, !cell.isInterior ? cellId.z : -1);
                    LANDsById[land.gridId] = land
                }
            }
        }
        _ensureCELLsByLabel.insert(cellSubBlockId)
        return self
    }
}
