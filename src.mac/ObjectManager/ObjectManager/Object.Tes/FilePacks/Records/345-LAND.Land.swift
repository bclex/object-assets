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
        public var vertexs: [Vector3Int] // XYZ 8 bit floats

        init(_ r: BinaryReader, _ dataSize: Int) {
            vertexs = [Vector3Int](); let capacity = dataSize / 3; vertexs.reserveCapacity(capacity)
            for _ in 0..<capacity { vertexs.append(Vector3Int(Int(r.readByte()), Int(r.readByte()), Int(r.readByte()))) }
        }
    }

    public struct VHGTField {
        public let referenceHeight: Float // A height offset for the entire cell. Decreasing this value will shift the entire cell land down.
        public var heightData: [Int8] // HeightData

        init(_ r: BinaryReader, _ dataSize: Int) {
            referenceHeight = r.readLESingle()
            heightData = [Int8](); let capacity = dataSize - 4 - 3; heightData.reserveCapacity(capacity)
            for _ in 0..<capacity { heightData.append(r.readSByte()) }
            r.skipBytes(3) // Unused
        }
    }

    public struct VCLRField {
        public var colors: [ColorRef] // 24-bit RGB

        init(_ r: BinaryReader, _ dataSize: Int) {
            colors = [ColorRef](); let capacity = dataSize / 24; colors.reserveCapacity(capacity)
            for _ in 0..<capacity { colors.append(ColorRef(red: r.readByte(), green: r.readByte(), blue: r.readByte())) }
        }
    }

    public struct VTEXField {
        public var textureIndices: [UInt32]

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                textureIndices = [UInt32](); let capacity = dataSize >> 1; textureIndices.reserveCapacity(capacity)
                for _ in 0..<capacity { textureIndices.append(UInt32(r.readLEUInt16())) }
                return
            }
            textureIndices = [UInt32](); let capacity = dataSize >> 2; textureIndices.reserveCapacity(capacity)
            for _ in 0..<capacity { textureIndices.append(r.readLEUInt32()) }
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
            for _ in 0..<heightCount { _ = r.readByte() }
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
    public var VHGT: VHGTField! // Height data
    public var VCLR: VNMLField? = nil // Vertex color array, looks like another RBG image 65x65 pixels in size. (Optional)
    public var VTEX: VTEXField? = nil // A 16x16 array of short texture indices. (Optional)
    // TES3
    public var INTV: CORDField! // The cell coordinates of the cell
    public var WNAM: WNAMField! // Unknown byte data.
    // TES4
    public var BTXTs = [BTXTField]() // Base Layer
    public var ATXTs: [ATXTGroup?]! // Alpha Layer
    var _lastATXT: ATXTGroup!

    public var GridCoords: Vector2Int { return Vector2Int(Int(INTV!.cellX), Int(INTV!.cellY)) }

    override init(_ header: Header) {
        BTXTs.reserveCapacity(4)
        super.init(header)
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
        case "BTXT": let btxt = BTXTField(r, dataSize); BTXTs[Int(btxt.quadrant)] = btxt
        case "ATXT":
            if ATXTs == nil { ATXTs = [ATXTGroup?](repeating: nil, count: 4) }
            let atxt = BTXTField(r, dataSize); _lastATXT = ATXTGroup(ATXT: atxt); ATXTs![Int(atxt.quadrant)] = _lastATXT
        case "VTXT":
            var vtxt = [VTXTField](); let capacity = dataSize >> 3; vtxt.reserveCapacity(capacity)
            for _ in 0..<capacity { vtxt.append(VTXTField(r, dataSize)) }; _lastATXT.VTXTs = vtxt
        default: return false
        }
        return true
    }
}
