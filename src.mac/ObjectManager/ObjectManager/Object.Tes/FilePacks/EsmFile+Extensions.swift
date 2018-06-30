//
//  EsmFile+Extensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

extension EsmFile {
    // TES3
    var _LTEXsById: [Int : LTEXRecord]
    var _CELLsById: [Vector2Int : CELLRecord]
    var _LANDsById: [Vector2Int : LANDRecord]

    func process() {
        guard format != .TES3 else {
            // _LTEXsById = Groups.ContainsKey("LTEX") ? Groups["LTEX"].Load().Cast<LTEXRecord>().ToDictionary(x => x.INTV.Value) : null;
            // _CELLsById = Groups.ContainsKey("CELL") ? Groups["CELL"].Load().Cast<CELLRecord>().Where(x => !x.IsInterior).ToDictionary(x => x.GridCoords) : null;
            // _LANDsById = Groups.ContainsKey("LAND") ? Groups["LAND"].Load().Cast<LANDRecord>().ToDictionary(x => x.GridCoords) : null;
            return
        }
        fatalError("NotImplemented")
        //_ltexsByEid = Groups["LTEX"].Load().Cast<LTEXRecord>().ToDictionary(x => x.EDID.Value)
    }

    func findLTEXRecord(_ index: Int) -> LTEXRecord {
        if format != .TES3 else {
            return _LTEXsById[index]
        }
        fatalError("NotImplemented")
    }

    func findLANDRecord(_ cellId: Vector2Int) -> LANDRecord {
        if format != .TES3 else {
            return _LANDsById[cellId]
        }
        fatalError("NotImplemented")
    }

    func findExteriorCellRecord(_ cellId: Vector2Int) -> CELLRecord {
        if format != .TES3 else {
            return _CELLsById[cellId]
        }
        fatalError("NotImplemented")
    }
}