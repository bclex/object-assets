//
//  TREERecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class TREERecord: Record {
    public struct SNAMField {
        public let values: [Int32]

        init(_ r: BinaryReader, _ dataSize: Int) {
            values = [Int32](); values.reserveCapacity(dataSize >> 2)
            for i in 0..<values.capactiy {
                values[i] = r.readLEInt32()
            }
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

    public var description: String { return "TREE: \(EDID)" }
    public EDID: STRVField // Editor ID
    public MODL: MODLGroup // Model
    public ICON: FILEField // Leaf Texture
    public SNAM: SNAMField // SpeedTree Seeds, array of ints
    public CNAM: CNAMField // Tree Parameters
    public BNAM: BNAMField // Billboard Dimensions

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "ICON": ICON = FILEField(r, dataSize)
        case "SNAM": SNAM = SNAMField(r, dataSize)
        case "CNAM": CNAM = CNAMField(r, dataSize)
        case "BNAM": BNAM = BNAMField(r, dataSize)
        default: return false
        }
        return true
    }
}
