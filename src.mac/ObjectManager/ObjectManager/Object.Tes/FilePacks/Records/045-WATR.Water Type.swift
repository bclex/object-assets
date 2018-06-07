//
//  WATRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class WATRRecord: Record {
    public class DATAField {
        public let windVelocity: Float
        public let windDirection: Float
        public let waveAmplitude: Float
        public let waveFrequency: Float
        public let sunPower: Float
        public let reflectivityAmount: Float
        public let fresnelAmount: Float
        public let scrollXSpeed: Float
        public let scrollYSpeed: Float
        public let fogDistance_nearPlane: Float
        public let fogDistance_farPlane: Float
        public let shallowColor: ColorRef
        public let deepColor: ColorRef
        public let reflectionColor: ColorRef
        public let textureBlend: UInt8
        public let rainSimulator_force: Float
        public let rainSimulator_velocity: Float
        public let rainSimulator_falloff: Float
        public let rainSimulator_dampner: Float
        public let rainSimulator_startingSize: Float
        public let displacementSimulator_force: Float
        public let displacementSimulator_velocity: Float
        public let displacementSimulator_falloff: Float
        public let displacementSimulator_dampner: Float
        public let displacementSimulator_startingSize: Float
        public let damage: UInt16

        init(_ r: BinaryReader, _ dataSize: Int) {
            guard dataSize != 2 else {
                damage = r.readLEUInt16()
                return
            }
            windVelocity = r.readLESingle()
            windDirection = r.readLESingle()
            waveAmplitude = r.readLESingle()
            waveFrequency = r.readLESingle()
            sunPower = r.readLESingle()
            reflectivityAmount = r.readLESingle()
            fresnelAmount = r.readLESingle()
            scrollXSpeed = r.readLESingle()
            scrollYSpeed = r.readLESingle()
            fogDistance_nearPlane = r.readLESingle()
            guard dataSize != 42 else {
                damage = r.readLEUInt16()
                return
            }
            fogDistance_farPlane = r.readLESingle()
            shallowColor = ColorRef(r)
            deepColor = ColorRef(r)
            reflectionColor = ColorRef(r)
            textureBlend = r.readByte()
            r.skipBytes(3) // Unused
            guard dataSize != 62 else {
                damage = r.readLEUInt16()
                return
            }
            rainSimulator_force = r.readLESingle()
            rainSimulator_velocity = r.readLESingle()
            rainSimulator_falloff = r.readLESingle()
            rainSimulator_dampner = r.readLESingle()
            rainSimulator_startingSize = r.readLESingle()
            displacementSimulator_force = r.readLESingle()
            guard dataSize != 86 {
                //displacementSimulator_velocity = displacementSimulator_falloff = displacementSimulator_dampner = displacementSimulator_startingSize = 0
                damage = r.readLEUInt16()
                return
            }
            displacementSimulator_velocity = r.readLESingle()
            displacementSimulator_falloff = r.readLESingle()
            displacementSimulator_dampner = r.readLESingle()
            displacementSimulator_startingSize = r.readLESingle()
            damage = r.readLEUInt16()
        }
    }

    public struct GNAMField {
        public let daytime: FormId<WATRRecord> 
        public let nighttime: FormId<WATRRecord> 
        public let underwater: FormId<WATRRecord> 

        init(_ r: BinaryReader, _ dataSize: Int) {
            daytime = FormId<WATRRecord>(r.readLEUInt32())
            nighttime = FormId<WATRRecord>(r.readLEUInt32())
            underwater = FormId<WATRRecord>(r.readLEUInt32())
        }
    }

    public var description: String { return "WATR: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var TNAM: STRVField // Texture
    public var ANAM: BYTEField // Opacity
    public var FNAM: BYTEField // Flags
    public var MNAM: STRVField // Material ID
    public var SNAM: FMIDField<SOUNRecord> // Sound
    public var DATA: DATAField // DATA
    public var GNAM: GNAMField // GNAM

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
            case "EDID": EDID = STRVField(r, dataSize)
            case "TNAM": TNAM = STRVField(r, dataSize)
            case "ANAM": ANAM = BYTEField(r, dataSize)
            case "FNAM": FNAM = BYTEField(r, dataSize)
            case "MNAM": MNAM = STRVField(r, dataSize)
            case "SNAM": SNAM = FMIDField<SOUNRecord>(r, dataSize)
            case "DATA": DATA = DATAField(r, dataSize)
            case "GNAM": GNAM = GNAMField(r, dataSize)
            default: return false
        }
        return true
    }
}
