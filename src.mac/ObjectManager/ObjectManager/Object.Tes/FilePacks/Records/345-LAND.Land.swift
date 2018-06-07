//
//  LANDRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LANDRecord: Record {
    // TESX
    public struct VNMLField {
        public let vertexs: [Vector3Int] // XYZ 8 bit floats

        init(_ r: BinaryReader, _ dataSize: Int) {
            vertexs = [Vector3Int](); vertexs.reserveCapacity(dataSize / 3)
            for i in 0..<vertexs.capacity {
                Vertexs[i] = Vector3Int(r.readByte(), r.readByte(), r.readByte())
            }
        }
    }

    public struct VHGTField {
        public let referenceHeight: Float // A height offset for the entire cell. Decreasing this value will shift the entire cell land down.
        public let heightData: [Int8] // HeightData

        init(_ r: BinaryReader, _ dataSize: Int) {
            referenceHeight = r.readLESingle()
            heightData = [Int8](); heightData.reserveCapacity(dataSize - 4 - 3)
            for i in 0..<heightData.capacity {
                heightData[i] = r.readSByte()
            }
            r.skipBytes(3) // Unused
        }
    }

    public struct VCLRField {
        public let colors: [ColorRef] // 24-bit RGB

        init(_ r: BinaryReader, _ dataSize: Int) {
            colors = [ColorRef](); colors.reserveCapacity(dataSize / 24)
            for i in 0..<colors.capacity {
                colors[i] = ColorRef(r.readByte(), r.readByte(), r.readByte())
            }
        }
    }

    public struct VTEXField {
        public let textureIndices: [UInt32]

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                textureIndices = [UInt32](); textureIndices.reserveCapacity(dataSize >> 1)
                for i in 0..<textureIndices.capacity {
                    textureIndices[i] = r.readLEUInt16()
                }
                return;
            }
            textureIndices = [UInt32](); textureIndices.reserveCapacity(dataSize >> 2)
            for i in 0..<textureIndices.capacity {
                textureIndices[i] = r.readLEUInt32()
            }
        }
    }

    // TES3
    public struct CORDField: CustomStringConvertible {
        public var description: String { return "\(cellX), \(cellY)" }
        public var cellX: Int32, cellY: Int32

        init(_ r: BinaryReader, _ dataSize: Int) {
            cellX = r.readLEInt32()
            cellY = r.readLEInt32()
        }
    }

    public struct WNAMField {
        // Low-LOD heightmap (signed chars)
        init(_ r: BinaryReader, _ dataSize: Int) {
            let heightCount = dataSize
            for i in 0..< heightCount {
                let height = r.readByte()
            }
        }
    }

    // TES4
    public struct BTXTField {
        public let texture: UInt32
        public let quadrant: UInt8
        public let layer: Int16

        init(_ r: BinaryReader, _ dataSize: Int) {
            texture = r.readLEUInt32()
            quadrant = r.readByte()
            _ = r.readByte() // Unused
            layer = r.readLEInt16()
        }
    }

    public struct VTXTField {
        public let position: UInt16
        public let opacity: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            position = r.readLEUInt16()
            r.skipBytes(2) // Unused
            opacity = r.readLESingle()
        }
    }

    public class ATXTGroup {
        public var ATXT: BTXTField
        public var VTXTs: [VTXTField]
    }

    public var description: String { return "LAND: \(INTV)" }
    public var DATA: IN32Field // Unknown (default of 0x09) Changing this value makes the land 'disappear' in the editor.
    // A RGB color map 65x65 pixels in size representing the land normal vectors.
    // The signed value of the 'color' represents the vector's component. Blue
    // is vertical(Z), Red the X direction and Green the Y direction.Note that
    // the y-direction of the data is from the bottom up.
    public var VNML: VNMLField
    public var VHGT: VHGTField // Height data
    public var VCLR: VNMLField? // Vertex color array, looks like another RBG image 65x65 pixels in size. (Optional)
    public var VTEX: VTEXField? // A 16x16 array of short texture indices. (Optional)
    // TES3
    public var INTV: CORDField // The cell coordinates of the cell
    public var WNAM: WNAMField // Unknown byte data.
    // TES4
    public var BTXTs = BTXTField() // Base Layer
    public var ATXTs: [ATXTGroup] // Alpha Layer
    var _lastATXT: ATXTGroup

    public var GridCoords: Vector2Int { return Vector2Int(INTV.cellX, INTV.cellY) }

    init() {
        BTXTs.reserveCapacity(4);
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "DATA": DATA = IN32Field(r, dataSize)
        case "VNML": VNML = VNMLField(r, dataSize)
        case "VHGT": VHGT = VHGTField(r, dataSize)
        case "VCLR": VCLR = VNMLField(r, dataSize)
        case "VTEX": VTEX = VTEXField(r, dataSize, format)
        // TES3
        case "INTV": INTV = CORDField(r, dataSize)
        case "WNAM": WNAM = WNAMField(r, dataSize)
        // TES4
        case "BTXT": var btxt = BTXTField(r, dataSize); BTXTs[btxt.Quadrant] = btxt
        case "ATXT": if ATXTs == nil { ATXTs = [ATXTGroup](); ATXTs.reserveCapacity(4) }; let atxt = BTXTField(r, dataSize); _lastATXT = ATXTs[atxt.Quadrant] = ATXTGroup(ATXT: atxt)
        case "VTXT": var vtxt = [VTXTField](); vtx.reserveCapacity(dataSize >> 3); for i in 0..<vtxt.capacity { vtxt[i] = VTXTField(r, dataSize) }; _lastATXT.VTXTs = vtxt
        default: return false
        }
        return true
    }
}
