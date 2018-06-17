//
//  WATRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class WATRRecord: Record {
    public class DATAField {
        public var windVelocity: Float = 0
        public var windDirection: Float = 0
        public var waveAmplitude: Float = 0
        public var waveFrequency: Float = 0
        public var sunPower: Float = 0
        public var reflectivityAmount: Float = 0
        public var fresnelAmount: Float = 0
        public var scrollXSpeed: Float = 0
        public var scrollYSpeed: Float = 0
        public var fogDistance_nearPlane: Float = 0
        public var fogDistance_farPlane: Float = 0
        public var shallowColor: ColorRef = ColorRef.empty
        public var deepColor: ColorRef = ColorRef.empty
        public var reflectionColor: ColorRef = ColorRef.empty
        public var textureBlend: UInt8 = 0
        public var rainSimulator_force: Float = 0
        public var rainSimulator_velocity: Float = 0
        public var rainSimulator_falloff: Float = 0
        public var rainSimulator_dampner: Float = 0
        public var rainSimulator_startingSize: Float = 0
        public var displacementSimulator_force: Float = 0
        public var displacementSimulator_velocity: Float = 0
        public var displacementSimulator_falloff: Float = 0
        public var displacementSimulator_dampner: Float = 0
        public var displacementSimulator_startingSize: Float = 0
        public var damage: UInt16 = 0

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
            guard dataSize != 86 else {
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

    public override var description: String { return "WATR: \(EDID)" }
    public var EDID: STRVField = STRVField.empty // Editor ID
    public var TNAM: STRVField! // Texture
    public var ANAM: BYTEField! // Opacity
    public var FNAM: BYTEField! // Flags
    public var MNAM: STRVField! // Material ID
    public var SNAM: FMIDField<SOUNRecord>! // Sound
    public var DATA: DATAField! // DATA
    public var GNAM: GNAMField! // GNAM

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
