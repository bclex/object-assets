//
//  LANDRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import simd

public class LANDRecord: Record {
    // TESX
    public struct VNMLField {
        public var vertexs: [byte3] // XYZ 8 bit floats
        
        init(_ r: BinaryReader, _ dataSize: Int) {
            vertexs = r.readTArray(dataSize, count: dataSize / 3)
        }
    }

    public struct VHGTField {
        public let referenceHeight: Float // A height offset for the entire cell. Decreasing this value will shift the entire cell land down.
        public var heightData = [Int8]() // HeightData

        init(_ r: BinaryReader, _ dataSize: Int) {
            referenceHeight = r.readLESingle()
            let count = dataSize - 4 - 3
            heightData = r.readTArray(count, count: count)
            r.skipBytes(3) // Unused
        }
    }

    public struct VCLRField {
        public var colors: [ColorRef3] // 24-bit RGB

        init(_ r: BinaryReader, _ dataSize: Int) {
            colors = r.readTArray(dataSize, count: dataSize / 24)
        }
    }

    public struct VTEXField {
        public var textureIndicesT3: [UInt16]!
        public var textureIndicesT4: [UInt32]!

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                textureIndicesT3 = r.readTArray(dataSize, count: dataSize >> 1)
                return
            }
            textureIndicesT4 = r.readTArray(dataSize, count: dataSize >> 2)
        }
    }

    // TES3
    public typealias CORDField = (
        cellX: Int32,
        cellY: Int32
    )

    public struct WNAMField {
        // Low-LOD heightmap (signed chars)
        init(_ r: BinaryReader, _ dataSize: Int) {
            r.skipBytes(dataSize)
        }
    }

    // TES4
    public typealias BTXTField = (
        texture: UInt32,
        quadrant: UInt8,
        pad01: UInt8,
        layer: Int16
    )

    public typealias VTXTField = (
        position: UInt16,
        pad01: UInt16,
        opacity: Float
    )

    public class ATXTGroup {
        public let ATXT: BTXTField
        public var VTXTs: [VTXTField]!
        
        init(ATXT: BTXTField) {
            self.ATXT = ATXT
        }
    }

    public override var description: String { return "LAND: \(INTV!)" }
    public var DATA: IN32Field! // Unknown (default of 0x09) Changing this value makes the land 'disappear' in the editor.
    // A RGB color map 65x65 pixels in size representing the land normal vectors.
    // The signed value of the 'color' represents the vector's component. Blue
    // is vertical(Z), Red the X direction and Green the Y direction.Note that
    // the y-direction of the data is from the bottom up.
    public var VNML: VNMLField!
    public var VHGT: VHGTField? = nil // Height data
    public var VCLR: VNMLField? = nil // Vertex color array, looks like another RBG image 65x65 pixels in size. (Optional)
    public var VTEX: VTEXField? = nil // A 16x16 array of short texture indices. (Optional)
    // TES3
    public var INTV: CORDField! // The cell coordinates of the cell
    public var WNAM: WNAMField! // Unknown byte data.
    // TES4
    public var BTXTs = [BTXTField]() // Base Layer
    public var ATXTs: [ATXTGroup?]! // Alpha Layer
    var _lastATXT: ATXTGroup!

    public var gridId: int3!

    override init(_ header: Header) {
        BTXTs.reserveCapacity(4)
        super.init(header)
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "DATA": DATA = r.readT(dataSize)
        case "VNML": VNML = VNMLField(r, dataSize)
        case "VHGT": VHGT = VHGTField(r, dataSize)
        case "VCLR": VCLR = VNMLField(r, dataSize)
        case "VTEX": VTEX = VTEXField(r, dataSize, format)
        // TES3
        case "INTV": INTV = r.readT(dataSize)
        case "WNAM": WNAM = WNAMField(r, dataSize)
        // TES4
        case "BTXT": let btxt: BTXTField = r.readT(dataSize); BTXTs[Int(btxt.quadrant)] = btxt
        case "ATXT":
            if ATXTs == nil { ATXTs = [ATXTGroup?](repeating: nil, count: 4) }
            let atxt: BTXTField = r.readT(dataSize); _lastATXT = ATXTGroup(ATXT: atxt); ATXTs![Int(atxt.quadrant)] = _lastATXT
        case "VTXT": _lastATXT.VTXTs = r.readTArray(dataSize, count: dataSize >> 3)
        default: return false
        }
        return true
    }
}
