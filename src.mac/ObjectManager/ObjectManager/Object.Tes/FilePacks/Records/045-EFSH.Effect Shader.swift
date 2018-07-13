//
//  EFSHRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class EFSHRecord: Record {
    public struct DATAField {
        public let flags: UInt8
        public let membraneShader_sourceBlendMode: UInt32
        public let membraneShader_blendOperation: UInt32
        public let membraneShader_ztestFunction: UInt32
        public let fillTextureEffect_color: ColorRef4
        public let fillTextureEffect_alphaFadeInTime: Float
        public let fillTextureEffect_fullAlphaTime: Float
        public let fillTextureEffect_alphaFadeOutTime: Float
        public let fillTextureEffect_presistentAlphaRatio: Float
        public let fillTextureEffect_alphaPulseAmplitude: Float
        public let fillTextureEffect_alphaPulseFrequency: Float
        public let fillTextureEffect_textureAnimationSpeed_u: Float
        public let fillTextureEffect_textureAnimationSpeed_v: Float
        public let edgeEffect_fallOff: Float
        public let edgeEffect_color: ColorRef4
        public let edgeEffect_alphaFadeInTime: Float
        public let edgeEffect_fullAlphaTime: Float
        public let edgeEffect_alphaFadeOutTime: Float
        public let edgeEffect_presistentAlphaRatio: Float
        public let edgeEffect_alphaPulseAmplitude: Float
        public let edgeEffect_alphaPulseFrequency: Float
        public let fillTextureEffect_fullAlphaRatio: Float
        public let edgeEffect_fullAlphaRatio: Float
        public let membraneShader_destBlendMode: UInt32
        public var particleShader_sourceBlendMode: UInt32? = nil
        public var particleShader_blendOperation: UInt32? = nil
        public var particleShader_ztestFunction: UInt32? = nil
        public var particleShader_destBlendMode: UInt32? = nil
        public var particleShader_particleBirthRampUpTime: Float? = nil
        public var particleShader_fullParticleBirthTime: Float? = nil
        public var particleShader_particleBirthRampDownTime: Float? = nil
        public var particleShader_fullParticleBirthRatio: Float? = nil
        public var particleShader_persistantParticleBirthRatio: Float? = nil
        public var particleShader_particleLifetime: Float? = nil
        public var particleShader_particleLifetime_Delta: Float? = nil
        public var particleShader_initialSpeedAlongNormal: Float? = nil
        public var particleShader_accelerationAlongNormal: Float? = nil
        public var particleShader_initialVelocity1: Float? = nil
        public var particleShader_initialVelocity2: Float? = nil
        public var particleShader_initialVelocity3: Float? = nil
        public var particleShader_acceleration1: Float? = nil
        public var particleShader_acceleration2: Float? = nil
        public var particleShader_acceleration3: Float? = nil
        public var particleShader_scaleKey1: Float? = nil
        public var particleShader_scaleKey2: Float? = nil
        public var particleShader_scaleKey1Time: Float? = nil
        public var particleShader_scaleKey2Time: Float? = nil
        public var colorKey1_color: ColorRef4? = nil
        public var colorKey2_color: ColorRef4? = nil
        public var colorKey3_color: ColorRef4? = nil
        public var colorKey1_colorAlpha: Float? = nil
        public var colorKey2_colorAlpha: Float? = nil
        public var colorKey3_colorAlpha: Float? = nil
        public var colorKey1_colorKeyTime: Float? = nil
        public var colorKey2_colorKeyTime: Float? = nil
        public var colorKey3_colorKeyTime: Float? = nil

        init(_ r: BinaryReader, _ dataSize: Int) {
            flags = r.readByte()
            r.skipBytes(3) // Unused
            membraneShader_sourceBlendMode = r.readLEUInt32()
            membraneShader_blendOperation = r.readLEUInt32()
            membraneShader_ztestFunction = r.readLEUInt32()
            fillTextureEffect_color = r.readT(4)
            fillTextureEffect_alphaFadeInTime = r.readLESingle()
            fillTextureEffect_fullAlphaTime = r.readLESingle()
            fillTextureEffect_alphaFadeOutTime = r.readLESingle()
            fillTextureEffect_presistentAlphaRatio = r.readLESingle()
            fillTextureEffect_alphaPulseAmplitude = r.readLESingle()
            fillTextureEffect_alphaPulseFrequency = r.readLESingle()
            fillTextureEffect_textureAnimationSpeed_u = r.readLESingle()
            fillTextureEffect_textureAnimationSpeed_v = r.readLESingle()
            edgeEffect_fallOff = r.readLESingle()
            edgeEffect_color = r.readT(4)
            edgeEffect_alphaFadeInTime = r.readLESingle()
            edgeEffect_fullAlphaTime = r.readLESingle()
            edgeEffect_alphaFadeOutTime = r.readLESingle()
            edgeEffect_presistentAlphaRatio = r.readLESingle()
            edgeEffect_alphaPulseAmplitude = r.readLESingle()
            edgeEffect_alphaPulseFrequency = r.readLESingle()
            fillTextureEffect_fullAlphaRatio = r.readLESingle()
            edgeEffect_fullAlphaRatio = r.readLESingle()
            membraneShader_destBlendMode = r.readLEUInt32()
            guard dataSize != 96 else {
                return
            }
            particleShader_sourceBlendMode = r.readLEUInt32()
            particleShader_blendOperation = r.readLEUInt32()
            particleShader_ztestFunction = r.readLEUInt32()
            particleShader_destBlendMode = r.readLEUInt32()
            particleShader_particleBirthRampUpTime = r.readLESingle()
            particleShader_fullParticleBirthTime = r.readLESingle()
            particleShader_particleBirthRampDownTime = r.readLESingle()
            particleShader_fullParticleBirthRatio = r.readLESingle()
            particleShader_persistantParticleBirthRatio = r.readLESingle()
            particleShader_particleLifetime = r.readLESingle()
            particleShader_particleLifetime_Delta = r.readLESingle()
            particleShader_initialSpeedAlongNormal = r.readLESingle()
            particleShader_accelerationAlongNormal = r.readLESingle()
            particleShader_initialVelocity1 = r.readLESingle()
            particleShader_initialVelocity2 = r.readLESingle()
            particleShader_initialVelocity3 = r.readLESingle()
            particleShader_acceleration1 = r.readLESingle()
            particleShader_acceleration2 = r.readLESingle()
            particleShader_acceleration3 = r.readLESingle()
            particleShader_scaleKey1 = r.readLESingle()
            particleShader_scaleKey2 = r.readLESingle()
            particleShader_scaleKey1Time = r.readLESingle()
            particleShader_scaleKey2Time = r.readLESingle()
            colorKey1_color = r.readT(4)
            colorKey2_color = r.readT(4)
            colorKey3_color = r.readT(4)
            colorKey1_colorAlpha = r.readLESingle()
            colorKey2_colorAlpha = r.readLESingle()
            colorKey3_colorAlpha = r.readLESingle()
            colorKey1_colorKeyTime = r.readLESingle()
            colorKey2_colorKeyTime = r.readLESingle()
            colorKey3_colorKeyTime = r.readLESingle()
        }
    }

    public override var description: String { return "EFSH: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var ICON: FILEField! // Fill Texture
    public var ICO2: FILEField! // Particle Shader Texture
    public var DATA: DATAField! // Data
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "ICON": ICON = r.readSTRV(dataSize)
        case "ICO2": ICO2 = r.readSTRV(dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        default: return false
        }
        return true
    }
}
