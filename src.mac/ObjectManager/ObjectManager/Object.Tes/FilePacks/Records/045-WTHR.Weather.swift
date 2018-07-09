//
//  WTHRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class WTHRRecord: Record, IHaveEDID, IHaveMODL {
    public struct FNAMField {
        public let dayNear: Float
        public let dayFar: Float
        public let nightNear: Float
        public let nightFar: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            dayNear = r.readLESingle()
            dayFar = r.readLESingle()
            nightNear = r.readLESingle()
            nightFar = r.readLESingle()
        }
    }

    public struct HNAMField {
        public let eyeAdaptSpeed: Float
        public let blurRadius: Float
        public let blurPasses: Float
        public let emissiveMult: Float
        public let targetLUM: Float
        public let upperLUMClamp: Float
        public let brightScale: Float
        public let brightClamp: Float
        public let lumRampNoTex: Float
        public let lumRampMin: Float
        public let lumRampMax: Float
        public let sunlightDimmer: Float
        public let grassDimmer: Float
        public let treeDimmer: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            eyeAdaptSpeed = r.readLESingle()
            blurRadius = r.readLESingle()
            blurPasses = r.readLESingle()
            emissiveMult = r.readLESingle()
            targetLUM = r.readLESingle()
            upperLUMClamp = r.readLESingle()
            brightScale = r.readLESingle()
            brightClamp = r.readLESingle()
            lumRampNoTex = r.readLESingle()
            lumRampMin = r.readLESingle()
            lumRampMax = r.readLESingle()
            sunlightDimmer = r.readLESingle()
            grassDimmer = r.readLESingle()
            treeDimmer = r.readLESingle()
        }
    }

    public struct DATAField {
        public let windSpeed: UInt8
        public let cloudSpeed_lower: UInt8
        public let cloudSpeed_upper: UInt8
        public let transDelta: UInt8
        public let sunGlare: UInt8
        public let sunDamage: UInt8
        public let precipitation_beginFadeIn: UInt8
        public let precipitation_endFadeOut: UInt8
        public let thunderLightning_beginFadeIn: UInt8
        public let thunderLightning_endFadeOut: UInt8
        public let thunderLightning_frequency: UInt8
        public let weatherClassification: UInt8
        public let lightningColor: ColorRef4

        init(_ r: BinaryReader, _ dataSize: Int) {
            windSpeed = r.readByte()
            cloudSpeed_lower = r.readByte()
            cloudSpeed_upper = r.readByte()
            transDelta = r.readByte()
            sunGlare = r.readByte()
            sunDamage = r.readByte()
            precipitation_beginFadeIn = r.readByte()
            precipitation_endFadeOut = r.readByte()
            thunderLightning_beginFadeIn = r.readByte()
            thunderLightning_endFadeOut = r.readByte()
            thunderLightning_frequency = r.readByte()
            weatherClassification = r.readByte()
            lightningColor = ColorRef4(red: r.readByte(), green: r.readByte(), blue: r.readByte(), null: 255)
        }
    }

    public struct SNAMField {
        public let sound: FormId<SOUNRecord> // Sound FormId
        public let type: UInt32 // Sound Type - 0=Default, 1=Precipitation, 2=Wind, 3=Thunder

        init(_ r: BinaryReader, _ dataSize: Int) {
            sound = FormId<SOUNRecord>(r.readLEUInt32())
            type = r.readLEUInt32()
        }
    }

    public override var description: String { return "WTHR: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var CNAM: FILEField! // Lower Cloud Layer
    public var DNAM: FILEField! // Upper Cloud Layer
    public var NAM0: BYTVField! // Colors by Types/Times
    public var FNAM: FNAMField! // Fog Distance
    public var HNAM: HNAMField! // HDR Data
    public var DATA: DATAField! // Weather Data
    public var SNAMs = [SNAMField]() // Sounds

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "CNAM": CNAM = r.readSTRV(dataSize)
        case "DNAM": DNAM = r.readSTRV(dataSize)
        case "NAM0": NAM0 = r.readBYTV(dataSize)
        case "FNAM": FNAM = FNAMField(r, dataSize)
        case "HNAM": HNAM = HNAMField(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "SNAM": SNAMs.append(SNAMField(r, dataSize))
        default: return false
        }
        return true
    }
}
