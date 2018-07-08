//
//  TREERecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class TREERecord: Record {
    public struct SNAMField {
        public var values: [Int32]

        init(_ r: BinaryReader, _ dataSize: Int) {
            values = [Int32](); let capacity = dataSize >> 2; values.reserveCapacity(capacity)
            for _ in 0..<capacity { values.append(r.readLEInt32()) }
        }
    }

    public struct CNAMField {
        public let leafCurvature: Float
        public let minimumLeafAngle: Float
        public let maximumLeafAngle: Float
        public let branchDimmingValue: Float
        public let leafDimmingValue: Float
        public let shadowRadius: Int32
        public let rockSpeed: Float
        public let rustleSpeed: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            leafCurvature = r.readLESingle()
            minimumLeafAngle = r.readLESingle()
            maximumLeafAngle = r.readLESingle()
            branchDimmingValue = r.readLESingle()
            leafDimmingValue = r.readLESingle()
            shadowRadius = r.readLEInt32()
            rockSpeed = r.readLESingle()
            rustleSpeed = r.readLESingle()
        }
    }

    public struct BNAMField {
        public let width: Float
        public let height: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            width = r.readLESingle()
            height = r.readLESingle()
        }
    }

    public override var description: String { return "TREE: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var ICON: FILEField! // Leaf Texture
    public var SNAM: SNAMField! // SpeedTree Seeds, array of ints
    public var CNAM: CNAMField! // Tree Parameters
    public var BNAM: BNAMField! // Billboard Dimensions

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        case "ICON": ICON = r.readSTRV(dataSize)
        case "SNAM": SNAM = SNAMField(r, dataSize)
        case "CNAM": CNAM = CNAMField(r, dataSize)
        case "BNAM": BNAM = BNAMField(r, dataSize)
        default: return false
        }
        return true
    }
}
