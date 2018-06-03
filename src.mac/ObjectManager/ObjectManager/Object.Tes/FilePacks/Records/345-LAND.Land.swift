//
//  LANDRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LANDRecord: Record {
    // TESX
    public struct VNMLField
    {
        public Vector3Int[] Vertexs // XYZ 8 bit floats

        public VNMLField(UnityBinaryReader r, uint dataSize)
        {
            Vertexs = Vector3Int[dataSize / 3];
            for (var i = 0; i < Vertexs.Length; i++)
                Vertexs[i] = Vector3Int(r.readByte(), r.readByte(), r.readByte());
        }
    }

    public struct VHGTField
    {
        public float ReferenceHeight // A height offset for the entire cell. Decreasing this value will shift the entire cell land down.
        public sbyte[] HeightData // HeightData

        public VHGTField(UnityBinaryReader r, uint dataSize)
        {
            ReferenceHeight = r.readLESingle();
            HeightData = sbyte[dataSize - 4 - 3];
            for (var i = 0; i < HeightData.Length; i++)
                HeightData[i] = r.readSByte();
            r.skipBytes(3) // Unused
        }
    }

    public struct VCLRField
    {
        public ColorRef[] Colors // 24-bit RGB

        public VCLRField(UnityBinaryReader r, uint dataSize)
        {
            Colors = ColorRef[dataSize / 24];
            for (var i = 0; i < Colors.Length; i++)
                Colors[i] = ColorRef(r.readByte(), r.readByte(), r.readByte());
        }
    }

    public struct VTEXField
    {
        public uint[] TextureIndices;

        public VTEXField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                TextureIndices = uint[dataSize >> 1];
                for (var i = 0; i < TextureIndices.Length; i++)
                    TextureIndices[i] = r.readLEUInt16();
            }
            else
            {
                TextureIndices = uint[dataSize >> 2];
                for (var i = 0; i < TextureIndices.Length; i++)
                    TextureIndices[i] = r.readLEUInt32();
            }
        }
    }

    // TES3
    public struct CORDField
    {
        public override string ToString() => $"{CellX}, {CellY}";
        public int CellX, CellY;

        public CORDField(UnityBinaryReader r, uint dataSize)
        {
            CellX = r.readLEInt32();
            CellY = r.readLEInt32();
        }
    }

    public struct WNAMField
    {
        // Low-LOD heightmap (signed chars)
        public WNAMField(UnityBinaryReader r, uint dataSize)
        {
            var heightCount = dataSize;
            for (var i = 0; i < heightCount; i++)
            {
                var height = r.readByte();
            }
        }
    }

    // TES4
    public struct BTXTField
    {
        public uint Texture;
        public byte Quadrant;
        public short Layer;

        public BTXTField(UnityBinaryReader r, uint dataSize)
        {
            Texture = r.readLEUInt32();
            Quadrant = r.readByte();
            r.readByte() // Unused
            Layer = r.readLEInt16();
        }
    }

    public struct VTXTField
    {
        public ushort Position;
        public float Opacity;

        public VTXTField(UnityBinaryReader r, uint dataSize)
        {
            Position = r.readLEUInt16();
            r.skipBytes(2) // Unused
            Opacity = r.readLESingle();
        }
    }

    public class ATXTGroup
    {
        public BTXTField ATXT;
        public VTXTField[] VTXTs;
    }

    public var description: String { return "LAND: \(INTV)" }
    public IN32Field DATA // Unknown (default of 0x09) Changing this value makes the land 'disappear' in the editor.
    public VNMLField VNML // A RGB color map 65x65 pixels in size representing the land normal vectors.
                            // The signed value of the 'color' represents the vector's component. Blue
                            // is vertical(Z), Red the X direction and Green the Y direction.Note that
                            // the y-direction of the data is from the bottom up.
    public VHGTField VHGT // Height data
    public VNMLField? VCLR // Vertex color array, looks like another RBG image 65x65 pixels in size. (Optional)
    public VTEXField? VTEX // A 16x16 array of short texture indices. (Optional)
    // TES3
    public CORDField INTV // The cell coordinates of the cell
    public WNAMField WNAM // Unknown byte data.
    // TES4
    public BTXTField[] BTXTs = BTXTField[4] // Base Layer
    public ATXTGroup[] ATXTs // Alpha Layer
    ATXTGroup _lastATXT;

    public Vector2i GridCoords => Vector2i(INTV.CellX, INTV.CellY);

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
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