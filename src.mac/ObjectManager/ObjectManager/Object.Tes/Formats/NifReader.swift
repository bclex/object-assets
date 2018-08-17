//
//  NifReader.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import simd

public struct Ptr<T> {
    public var value: Int32
    public var isNull: Bool { return value < 0 }

    init(_ r: BinaryReader) {
        value = r.readLEInt32()
    }
}

// Refers to an object after the current one in the hierarchy.
public struct Ref<T> {
    public var value: Int32
    public var isNull: Bool { return value < 0 }

    init(_ r: BinaryReader) {
        value = r.readLEInt32()
    }
}

public class NiReaderUtils
{
    public static func readLengthPrefixedRefs32<T>(_ r: BinaryReader) -> [Ref<T>] {
        let count = Int(r.readLEUInt32())
        var refs = [Ref<T>](); refs.reserveCapacity(count)
        for _ in 0..<count { refs.append(Ref<T>(r)) }
        return refs
    }
    
    public static func readFlags(_ r: BinaryReader) -> UInt16 {
        return r.readLEUInt16()
    }
    
    public static func read<T>(_ r: BinaryReader) -> T {
        if T.self == Float.self { return r.readLESingle() as! T }
        else if T.self == UInt8.self { return r.readByte() as! T }
        else if T.self == String.self { return r.readLELength32PrefixedASCIIString() as! T }
        else if T.self == float3.self { return r.readLEFloat3() as! T }
        else if T.self == simd_quatf.self { return r.readLEQuaternionWFirst() as! T }
        else if T.self == Color4.self { let t: Color4 = r.readT(4 << 2); return t as! T }
        else { fatalError("Tried to read an unsupported type.") }
    }
    
    public static func readNiObject(_ r: BinaryReader) -> NiObject? {
        let nodeType = r.readLELength32PrefixedASCIIString()
        if nodeType == "NiNode" { return NiNode(r) }
        else if nodeType == "NiTriShape" { return NiTriShape(r) }
        else if nodeType == "NiTexturingProperty" { return NiTexturingProperty(r) }
        else if nodeType == "NiSourceTexture" { return NiSourceTexture(r) }
        else if nodeType == "NiMaterialProperty" { return NiMaterialProperty(r) }
        else if nodeType == "NiMaterialColorController" { return NiMaterialColorController(r) }
        else if nodeType == "NiTriShapeData" { return NiTriShapeData(r) }
        else if nodeType == "RootCollisionNode" { return RootCollisionNode(r) }
        else if nodeType == "NiStringExtraData" { return NiStringExtraData(r) }
        else if nodeType == "NiSkinInstance" { return NiSkinInstance(r) }
        else if nodeType == "NiSkinData" { return NiSkinData(r) }
        else if nodeType == "NiAlphaProperty" { return NiAlphaProperty(r) }
        else if nodeType == "NiZBufferProperty" { return NiZBufferProperty(r) }
        else if nodeType == "NiVertexColorProperty" { return NiVertexColorProperty(r) }
        else if nodeType == "NiBSAnimationNode" { return NiBSAnimationNode(r) }
        else if nodeType == "NiBSParticleNode" { return NiBSParticleNode(r) }
        else if nodeType == "NiParticles" { return NiParticles(r) }
        else if nodeType == "NiParticlesData" { return NiParticlesData(r) }
        else if nodeType == "NiRotatingParticles" { return NiRotatingParticles(r) }
        else if nodeType == "NiRotatingParticlesData" { return NiRotatingParticlesData(r) }
        else if nodeType == "NiAutoNormalParticles" { return NiAutoNormalParticles(r) }
        else if nodeType == "NiAutoNormalParticlesData" { return NiAutoNormalParticlesData(r) }
        else if nodeType == "NiUVController" { return NiUVController(r) }
        else if nodeType == "NiUVData" { return NiUVData(r) }
        else if nodeType == "NiTextureEffect" { return NiTextureEffect(r) }
        else if nodeType == "NiTextKeyExtraData" { return NiTextKeyExtraData(r) }
        else if nodeType == "NiVertWeightsExtraData" { return NiVertWeightsExtraData(r) }
        else if nodeType == "NiParticleSystemController" { return NiParticleSystemController(r) }
        else if nodeType == "NiBSPArrayController" { return NiBSPArrayController(r) }
        else if nodeType == "NiGravity" { return NiGravity(r) }
        else if nodeType == "NiParticleBomb" { return NiParticleBomb(r) }
        else if nodeType == "NiParticleColorModifier" { return NiParticleColorModifier(r) }
        else if nodeType == "NiParticleGrowFade" { return NiParticleGrowFade(r) }
        else if nodeType == "NiParticleMeshModifier" { return NiParticleMeshModifier(r) }
        else if nodeType == "NiParticleRotation" { return NiParticleRotation(r) }
        else if nodeType == "NiKeyframeController" { return NiKeyframeController(r) }
        else if nodeType == "NiKeyframeData" { return NiKeyframeData(r) }
        else if nodeType == "NiColorData" { return NiColorData(r) }
        else if nodeType == "NiGeomMorpherController" { return NiGeomMorpherController(r) }
        else if nodeType == "NiMorphData" { return NiMorphData(r) }
        else if nodeType == "AvoidNode" { return AvoidNode(r) }
        else if nodeType == "NiVisController" { return NiVisController(r) }
        else if nodeType == "NiVisData" { return NiVisData(r) }
        else if nodeType == "NiAlphaController" { return NiAlphaController(r) }
        else if nodeType == "NiFloatData" { return NiFloatData(r) }
        else if nodeType == "NiPosData" { return NiPosData(r) }
        else if nodeType == "NiBillboardNode" { return NiBillboardNode(r) }
        else if nodeType == "NiShadeProperty" { return NiShadeProperty(r) }
        else { debugPrint("Tried to read an unsupported NiObject type (\(nodeType))."); return nil }
    }
    
    public static func read3x3RotationMatrix(_ r: BinaryReader) -> simd_float4x4 {
        return r.readLERowMajorMatrix3x3()
    }
}

public class NiFile {
    public let name: String
    public var header: NiHeader!
    public var blocks: [NiObject]!
    public var footer: NiFooter!

    init(name: String) {
        self.name = name
    }
    convenience init(_ r: BinaryReader, name: String) {
        self.init(name: name)
        header = NiHeader(r)
        blocks = [NiObject](); let capacity = Int(header.numBlocks); blocks.reserveCapacity(capacity)
        for _ in 0..<capacity { blocks.append(NiReaderUtils.readNiObject(r)!) }
        footer = NiFooter(r)
    }
}

// MARK: Enums

// texture enums
public enum ApplyMode: UInt32 {
    case APPLY_REPLACE = 0, APPLY_DECAL, APPLY_MODULATE, APPLY_HILIGHT, APPLY_HILIGHT2
}

public enum TexClampMode: UInt32 {
    case CLAMP_S_CLAMP_T = 0, CLAMP_S_WRAP_T, WRAP_S_CLAMP_T, WRAP_S_WRAP_T3
}

public enum TexFilterMode: UInt32 {
    case FILTER_NEAREST = 0, FILTER_BILERP, FILTER_TRILERP, FILTER_NEAREST_MIPNEAREST, FILTER_NEAREST_MIPLERP, FILTER_BILERP_MIPNEAREST
}

public enum PixelLayout: UInt32 {
    case PIX_LAY_PALETTISED = 0, PIX_LAY_HIGH_COLOR_16, PIX_LAY_TRUE_COLOR_32, PIX_LAY_COMPRESSED, PIX_LAY_BUMPMAP, PIX_LAY_PALETTISED_4, PIX_LAY_DEFAULT
}

public enum MipMapFormat: UInt32 {
    case MIP_FMT_NO = 0, MIP_FMT_YES, MIP_FMT_DEFAULT
}

public enum AlphaFormat: UInt32 {
    case ALPHA_NONE = 0, ALPHA_BINARY, ALPHA_SMOOTH, ALPHA_DEFAULT
}

// miscellaneous
public enum VertMode: UInt32 {
    case VERT_MODE_SRC_IGNORE = 0, VERT_MODE_SRC_EMISSIVE, VERT_MODE_SRC_AMB_DIF
}

public enum LightMode: UInt32 {
    case LIGHT_MODE_EMISSIVE = 0, LIGHT_MODE_EMI_AMB_DIF
}

public enum KeyType: UInt32 {
    case LINEAR_KEY = 1, QUADRATIC_KEY, TBC_KEY, XYZ_ROTATION_KEY, CONST_KEY
}

public enum EffectType: UInt32 {
    case EFFECT_PROJECTED_LIGHT = 0, EFFECT_PROJECTED_SHADOW, EFFECT_ENVIRONMENT_MAP, EFFECT_FOG_MAP
}

public enum CoordGenType: UInt32 {
    case CG_WORLD_PARALLEL = 0, CG_WORLD_PERSPECTIVE, CG_SPHERE_MAP, CG_SPECULAR_CUBE_MAP, CG_DIFFUSE_CUBE_MAP
}

public enum FieldType: UInt32 {
    case FIELD_WIND = 0, FIELD_POINT
}

public enum DecayType: UInt32 {
    case DECAY_NONE = 0, DECAY_LINEAR, DECAY_EXPONENTIAL
}


// MARK: Misc Classes

public struct BoundingBox {
    public let unknownInt: UInt32
    public let translation: float3
    public let rotation: simd_float4x4
    public let radius: float3

    init(_ r: BinaryReader) {
        unknownInt = r.readLEUInt32()
        translation = r.readLEFloat3()
        rotation = NiReaderUtils.read3x3RotationMatrix(r)
        radius = r.readLEFloat3()
    }
}

public typealias Color3 = (
    r: Float,
    g: Float,
    b: Float
)

public typealias Color4 = (
    r: Float,
    g: Float,
    b: Float,
    a: Float
)

public struct TexDesc {
    public let source: Ref<NiSourceTexture>
    public let clampMode: TexClampMode
    public let filterMode: TexFilterMode
    public let uvSet: UInt32
    public let ps2L: Int16
    public let ps2K: Int16
    public let unknown1: UInt16

    init(_ r: BinaryReader) {
        source = Ref<NiSourceTexture>(r)
        clampMode = TexClampMode(rawValue: r.readLEUInt32())!
        filterMode = TexFilterMode(rawValue: r.readLEUInt32())!
        uvSet = r.readLEUInt32()
        ps2L = r.readLEInt16()
        ps2K = r.readLEInt16()
        unknown1 = r.readLEUInt16()
    }
}

//public struct TexCoord {
//    public let u: Float
//    public let v: Float
//
//    init() { u = 0; v = 0 }
//    init(_ r: BinaryReader) {
//        u = r.readLESingle()
//        v = r.readLESingle()
//    }
//}

public typealias Triangle = (
    v1: UInt16,
    v2: UInt16,
    v3: UInt16
)

public struct MatchGroup {
    public let numVertices: UInt16
    public let vertexIndices: [UInt16]

    init(_ r: BinaryReader) {
        numVertices = r.readLEUInt16()
        let count = Int(numVertices)
        vertexIndices = r.readTArray(count << 1, count: count)
    }
}

public typealias TBC = (
    t: Float,
    b: Float,
    c: Float
)

public class Key<T> {
    public let time: Float
    public let value: T
    public var forward: T!
    public var backward: T!
    public var tbc: TBC!

    init(_ r: BinaryReader, keyType: KeyType) {
        time = r.readLESingle()
        value = NiReaderUtils.read(r)
        if keyType == .QUADRATIC_KEY {
            forward = NiReaderUtils.read(r)
            backward = NiReaderUtils.read(r)
        }
        else if keyType == .TBC_KEY {
            tbc = r.readT(3 << 2)
        }
    }
}

public class KeyGroup<T> {
    public let numKeys: UInt32
    public let interpolation: KeyType
    public var keys: [Key<T>]

    init(_ r: BinaryReader) {
        numKeys = r.readLEUInt32(); let count = Int(numKeys)
        interpolation = KeyType(rawValue: numKeys != 0 ? r.readLEUInt32() : 0)!
        keys = [Key<T>](); keys.reserveCapacity(count)
        for _ in 0..<count { keys.append(Key<T>(r, keyType: interpolation)) }
    }
}

public class QuatKey<T> {
    public let time: Float
    public var value: T!
    public var tbc: TBC!

    init(_ r: BinaryReader, keyType: KeyType) {
        time = r.readLESingle()
        if keyType != .XYZ_ROTATION_KEY {
            value = NiReaderUtils.read(r)
        }
        if keyType == .TBC_KEY {
            tbc = r.readT(3 << 2)
        }
    }
}

public struct SkinData {
    public let skinTransform: SkinTransform
    public let boundingSphereOffset: float3
    public let boundingSphereRadius: Float
    public let numVertices: UInt16
    public var vertexWeights: [SkinWeight]

    init(_ r: BinaryReader) {
        skinTransform = SkinTransform(r)
        boundingSphereOffset = r.readLEFloat3()
        boundingSphereRadius = r.readLESingle()
        numVertices = r.readLEUInt16(); let count = Int(numVertices)
        vertexWeights = r.readTArray(count * 6, count: count)
    }
}

public typealias SkinWeight = (
    index: UInt16,
    weight: Float
)


public struct SkinTransform {
    public let rotation: simd_float4x4
    public let translation: float3
    public let scale: Float

    init(_ r: BinaryReader) {
        rotation = NiReaderUtils.read3x3RotationMatrix(r)
        translation = r.readLEFloat3()
        scale = r.readLESingle()
    }
}

public typealias Particle = (
    velocity: Float3,
    unknownVector: Float3,
    lifetime: Float,
    lifespan: Float,
    timestamp: Float,
    unknownShort: UInt16,
    vertexId: UInt16
)

public struct Morph {
    public let numKeys: UInt32
    public let interpolation: KeyType
    public var keys: [Key<Float>]
    public var vectors: [Float3]

    init(_ r: BinaryReader, numVertices: UInt32) {
        numKeys = r.readLEUInt32(); var count = Int(numKeys)
        interpolation = KeyType(rawValue: r.readLEUInt32())!
        keys = [Key<Float>](); keys.reserveCapacity(count)
        for _ in 0..<count { keys.append(Key<Float>(r, keyType: interpolation)) }
        count = Int(numVertices)
        vectors = r.readTArray(count << 4, count: count)
    }
}

// MARK: A

public struct NiHeader {
    public let str: Data // 40 bytes (including \n)
    public let version: UInt32
    public let numBlocks: UInt32

    init(_ r: BinaryReader) {
        str = r.readBytes(40)
        version = r.readLEUInt32()
        numBlocks = r.readLEUInt32()
    }
}

public struct NiFooter {
    public let numRoots: UInt32
    public var roots: [Int32]

    init(_ r: BinaryReader) {
        numRoots = r.readLEUInt32(); let count = Int(numRoots)
        roots = r.readTArray(count << 2, count: count)
    }
}

public class NiObject {
}

public class NiObjectNET: NiObject {
    public let name: String
    public let extraData: Ref<NiExtraData>
    public let controller: Ref<NiTimeController>

    init(_ r: BinaryReader) {
        name = r.readLELength32PrefixedASCIIString()
        extraData = Ref<NiExtraData>(r)
        controller = Ref<NiTimeController>(r)
    }
}

public class NiAVObject: NiObjectNET {
    public struct NiFlags: OptionSet {
        public let rawValue: UInt16
        public static let hidden = NiFlags(rawValue: 0x1)
        
        public init(rawValue: UInt16) {
            self.rawValue = rawValue
        }
    }

    public var flags: NiFlags!
    public var translation: float3!
    public var rotation: simd_float4x4!
    public var scale: Float!
    public var velocity: float3!
    //public var numProperties: uint!
    public var properties: [Ref<NiProperty>]!
    public var hasBoundingBox: Bool!
    public var boundingBox: BoundingBox!

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = NiFlags(rawValue: NiReaderUtils.readFlags(r))
        translation = r.readLEFloat3()
        rotation = NiReaderUtils.read3x3RotationMatrix(r)
        scale = r.readLESingle()
        velocity = r.readLEFloat3()
        properties = NiReaderUtils.readLengthPrefixedRefs32(r)
        hasBoundingBox = r.readLEBool32()
        if hasBoundingBox {
            boundingBox = BoundingBox(r)
        }
    }
}

// Nodes
public class NiNode: NiAVObject {
    //public let numChildren: UInt32
    public var children: [Ref<NiAVObject>]!
    //public let numEffects: UInt32
    public var effects: [Ref<NiDynamicEffect>]!

    override init(_ r: BinaryReader) {
        super.init(r)
        children = NiReaderUtils.readLengthPrefixedRefs32(r)
        effects = NiReaderUtils.readLengthPrefixedRefs32(r)
    }
}

public class RootCollisionNode: NiNode { }

public class NiBSAnimationNode: NiNode { }

public class NiBSParticleNode: NiNode { }

public class NiBillboardNode: NiNode { }

public class AvoidNode: NiNode { }

// Geometry
public class NiGeometry: NiAVObject {
    public var data: Ref<NiGeometryData>!
    public var skinInstance: Ref<NiSkinInstance>!

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiGeometryData>(r)
        skinInstance = Ref<NiSkinInstance>(r)
    }
}

public class NiGeometryData: NiObject {
    public let numVertices: UInt16
    public let hasVertices: Bool
    public var vertices: [Float3]!
    public let hasNormals: Bool
    public var normals: [Float3]!
    public let center: float3
    public let radius: Float
    public let hasVertexColors: Bool
    public var vertexColors: [Color4]!
    public let numUVSets: UInt16
    public let hasUV: Bool
    public var uvSets: [[float2]]!

    init(_ r: BinaryReader) {
        numVertices = r.readLEUInt16(); let count = Int(numVertices)
        hasVertices = r.readLEBool32()
        if hasVertices {
            vertices = r.readTArray(count * 12, count: count)
        }
        hasNormals = r.readLEBool32()
        if hasNormals {
            normals = r.readTArray(count * 12, count: count)
        }
        center = r.readLEFloat3()
        radius = r.readLESingle()
        hasVertexColors = r.readLEBool32()
        if hasVertexColors {
            vertexColors = r.readTArray(count << 4, count: count)
        }
        numUVSets = r.readLEUInt16()
        hasUV = r.readLEBool32()
        if hasUV {
            let count2 = Int(numUVSets)
            uvSets = Array(repeating: Array(repeating: float2(), count: count), count: count2)
            for i in 0..<count2 {
                for j in 0..<count {
                    uvSets[i][j] = float2(r.readLESingle(), r.readLESingle())
                }
            }
        }
    }
}

public class NiTriBasedGeom: NiGeometry {
    override init(_ r: BinaryReader) {
        super.init(r)
    }
}

public class NiTriBasedGeomData: NiGeometryData {
    public var numTriangles: UInt16!

    override init(_ r: BinaryReader) {
        super.init(r)
        numTriangles = r.readLEUInt16()
    }
}

public class NiTriShape: NiTriBasedGeom {
    override init(_ r: BinaryReader) {
        super.init(r)
    }
}

public class NiTriShapeData: NiTriBasedGeomData {
    public var numTrianglePoints: UInt32!
    public var triangles: [Triangle]!
    public var numMatchGroups: UInt16!
    public var matchGroups: [MatchGroup]!

    override init(_ r: BinaryReader) {
        super.init(r)
        numTrianglePoints = r.readLEUInt32(); var count = Int(numTriangles)
        triangles = r.readTArray(count * 6, count: count)
        numMatchGroups = r.readLEUInt16(); count = Int(numMatchGroups)
        matchGroups = [MatchGroup](); matchGroups.reserveCapacity(count)
        for _ in 0..<count { matchGroups.append(MatchGroup(r)) }
    }
}

// Properties
public class NiProperty: NiObjectNET {
    override init(_ r: BinaryReader) {
        super.init(r)
    }
}

public class NiTexturingProperty: NiProperty {
    public var flags: UInt16!
    public var applyMode: ApplyMode!
    public var textureCount: UInt32!
    //public var hasBaseTexture: Bool!
    public var baseTexture: TexDesc?
    //public var hasDarkTexture: Bool!
    public var darkTexture: TexDesc?
    //public var hasDetailTexture: Bool!
    public var detailTexture: TexDesc?
    //public var hasGlossTexture: Bool!
    public var glossTexture: TexDesc?
    //public var hasGlowTexture: Bool!
    public var glowTexture: TexDesc?
    //public var hasBumpMapTexture: Bool!
    public var bumpMapTexture: TexDesc?
    //public var hasDecal0Texture: Bool!
    public var decal0Texture: TexDesc?

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = NiReaderUtils.readFlags(r)
        applyMode = ApplyMode(rawValue: r.readLEUInt32())!
        textureCount = r.readLEUInt32()
        let hasBaseTexture = r.readLEBool32()
        if hasBaseTexture {
            baseTexture = TexDesc(r)
        }
        let hasDarkTexture = r.readLEBool32()
        if hasDarkTexture {
            darkTexture = TexDesc(r)
        }
        let hasDetailTexture = r.readLEBool32()
        if hasDetailTexture {
            detailTexture = TexDesc(r)
        }
        let hasGlossTexture = r.readLEBool32()
        if hasGlossTexture {
            glossTexture = TexDesc(r)
        }
        let hasGlowTexture = r.readLEBool32()
        if hasGlowTexture {
            glowTexture = TexDesc(r)
        }
        let hasBumpMapTexture = r.readLEBool32()
        if hasBumpMapTexture {
            bumpMapTexture = TexDesc(r)
        }
        let hasDecal0Texture = r.readLEBool32()
        if hasDecal0Texture {
            decal0Texture = TexDesc(r)
        }
    }
}

public class NiAlphaProperty: NiProperty {
    public var flags: UInt16!
    public var threshold: UInt8!

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = r.readLEUInt16()
        threshold = r.readByte()
    }
}

public class NiZBufferProperty: NiProperty {
    public var flags: UInt16!

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = r.readLEUInt16()
    }
}

public class NiVertexColorProperty: NiProperty {
    public var flags: UInt16!
    public var vertexMode: VertMode!
    public var lightingMode: LightMode!

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = NiReaderUtils.readFlags(r)
        vertexMode = VertMode(rawValue: r.readLEUInt32())!
        lightingMode = LightMode(rawValue: r.readLEUInt32())!
    }
}

public class NiShadeProperty: NiProperty {
    public var flags: UInt16!

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = NiReaderUtils.readFlags(r)
    }
}

// Data
public class NiUVData: NiObject {
    public var uvGroups: [KeyGroup<Float>]

    init(_ r: BinaryReader) {
        uvGroups = [KeyGroup<Float>](); uvGroups.reserveCapacity(4)
        for _ in 0..<4 { uvGroups.append(KeyGroup<Float>(r)) }
    }
}

public class NiKeyframeData: NiObject {
    public let numRotationKeys: UInt32
    public var rotationType: KeyType!
    public var quaternionKeys: [QuatKey<simd_quatf>]!
    public var unknownFloat: Float!
    public var xyzRotations: [KeyGroup<Float>]!
    public var translations: KeyGroup<float3>
    public var scales: KeyGroup<Float>

    init(_ r: BinaryReader) {
        numRotationKeys = r.readLEUInt32(); let count = Int(numRotationKeys)
        if numRotationKeys != 0 {
            rotationType = KeyType(rawValue: r.readLEUInt32())!
            if rotationType != .XYZ_ROTATION_KEY {
                quaternionKeys = [QuatKey<simd_quatf>](); quaternionKeys.reserveCapacity(count)
                for _ in 0..<count { quaternionKeys.append(QuatKey<simd_quatf>(r, keyType: rotationType)) }
            }
            else {
                unknownFloat = r.readLESingle()
                xyzRotations = [KeyGroup<Float>](); xyzRotations.reserveCapacity(3)
                for _ in 0..<3 { xyzRotations.append(KeyGroup<Float>(r)) }
            }
        }
        translations = KeyGroup<float3>(r)
        scales = KeyGroup<Float>(r)
    }
}

public class NiColorData: NiObject {
    public let data: KeyGroup<Color4>

    init(_ r: BinaryReader) {
        data = KeyGroup<Color4>(r)
    }
}

public class NiMorphData: NiObject {
    public let numMorphs: UInt32
    public let numVertices: UInt32
    public let relativeTargets: UInt8
    public var morphs: [Morph]

    init(_ r: BinaryReader) {
        numMorphs = r.readLEUInt32(); let count = Int(numMorphs)
        numVertices = r.readLEUInt32()
        relativeTargets = r.readByte()
        morphs = [Morph](); morphs.reserveCapacity(count)
        for _ in 0..<count { morphs.append(Morph(r, numVertices: numVertices)) }
    }
}

public class NiVisData: NiObject {
    public let numKeys: UInt32
    public var keys: [Key<UInt8>]

    init(_ r: BinaryReader) {
        numKeys = r.readLEUInt32(); let count = Int(numKeys)
        keys = [Key<UInt8>](); keys.reserveCapacity(count)
        for _ in 0..<count { keys.append(Key<UInt8>(r, keyType: .LINEAR_KEY)) }
    }
}

public class NiFloatData: NiObject {
    public let data: KeyGroup<Float>

    init(_ r: BinaryReader) {
        data = KeyGroup<Float>(r)
    }
}

public class NiPosData: NiObject {
    public let data: KeyGroup<float3>

    init(_ r: BinaryReader) {
        data = KeyGroup<float3>(r)
    }
}

public class NiExtraData: NiObject {
    public let nextExtraData: Ref<NiExtraData>

    init(_ r: BinaryReader) {
        nextExtraData = Ref<NiExtraData>(r)
    }
}

public class NiStringExtraData: NiExtraData {
    public var bytesRemaining: UInt32!
    public var str: String!

    override init(_ r: BinaryReader) {
        super.init(r)
        bytesRemaining = r.readLEUInt32()
        str = r.readLELength32PrefixedASCIIString()
    }
}

public class NiTextKeyExtraData: NiExtraData {
    public var unknownInt1: UInt32!
    public var numTextKeys: UInt32!
    public var textKeys: [Key<String>]!

    override init(_ r: BinaryReader) {
        super.init(r)
        unknownInt1 = r.readLEUInt32()
        numTextKeys = r.readLEUInt32(); let count = Int(numTextKeys)
        textKeys = [Key<String>](); textKeys.reserveCapacity(count)
        for _ in 0..<count { textKeys.append(Key<String>(r, keyType: .LINEAR_KEY)) }
    }
}

public class NiVertWeightsExtraData: NiExtraData {
    public var numBytes: UInt32!
    public var numVertices: UInt16!
    public var weights: [Float]!

    override init(_ r: BinaryReader) {
        super.init(r)
        numBytes = r.readLEUInt32()
        numVertices = r.readLEUInt16(); let count = Int(numVertices)
        weights = r.readTArray(count << 2, count: count)
    }
}

// Particles
public class NiParticles: NiGeometry { }

public class NiParticlesData: NiGeometryData {
    public var numParticles: UInt16!
    public var particleRadius: Float!
    public var numActive: UInt16!
    public var hasSizes: Bool!
    public var sizes: [Float]!

    override init(_ r: BinaryReader) {
        super.init(r)
        numParticles = r.readLEUInt16()
        particleRadius = r.readLESingle()
        numActive = r.readLEUInt16()
        hasSizes = r.readLEBool32()
        if hasSizes {
            let count = Int(numVertices)
            sizes = r.readTArray(count << 2, count: count)
        }
    }
}

public class NiRotatingParticles: NiParticles { }

public class NiRotatingParticlesData: NiParticlesData {
    public var hasRotations: Bool!
    public var rotations: [simd_quatf]!

    override init(_ r: BinaryReader) {
        super.init(r)
        hasRotations = r.readLEBool32()
        if hasRotations {
            let count = Int(numVertices);
            rotations = [simd_quatf](); rotations.reserveCapacity(count)
            for _ in 0..<count { rotations.append(r.readLEQuaternionWFirst()) }
        }
    }
}

public class NiAutoNormalParticles: NiParticles { }

public class NiAutoNormalParticlesData: NiParticlesData { }

public class NiParticleSystemController: NiTimeController {
    public var speed: Float!
    public var speedRandom: Float!
    public var verticalDirection: Float!
    public var verticalAngle: Float!
    public var horizontalDirection: Float!
    public var horizontalAngle: Float!
    public var unknownNormal: float3!
    public var unknownColor: Color4!
    public var size: Float!
    public var emitStartTime: Float!
    public var emitStopTime: Float!
    public var unknownByte: UInt8!
    public var emitRate: Float!
    public var lifetime: Float!
    public var lifetimeRandom: Float!
    public var emitFlags: UInt16!
    public var startRandom: float3!
    public var emitter: Ptr<NiObject>!
    public var unknownShort2: UInt16!
    public var unknownFloat13: Float!
    public var unknownInt1: UInt32!
    public var unknownInt2: UInt32!
    public var unknownShort3: UInt16!
    public var numParticles: UInt16!
    public var numValid: UInt16!
    public var particles: [Particle]!
    public var unknownLink: Ref<NiObject>!
    public var particleExtra: Ref<NiParticleModifier>!
    public var unknownLink2: Ref<NiObject>!
    public var trailer: UInt8!

    override init(_ r: BinaryReader) {
        super.init(r)
        speed = r.readLESingle()
        speedRandom = r.readLESingle()
        verticalDirection = r.readLESingle()
        verticalAngle = r.readLESingle()
        horizontalDirection = r.readLESingle()
        horizontalAngle = r.readLESingle()
        unknownNormal = r.readLEFloat3()
        unknownColor = r.readT(4 << 2)
        size = r.readLESingle()
        emitStartTime = r.readLESingle()
        emitStopTime = r.readLESingle()
        unknownByte = r.readByte()
        emitRate = r.readLESingle()
        lifetime = r.readLESingle()
        lifetimeRandom = r.readLESingle()
        emitFlags = r.readLEUInt16()
        startRandom = r.readLEFloat3()
        emitter = Ptr<NiObject>(r)
        unknownShort2 = r.readLEUInt16()
        unknownFloat13 = r.readLESingle()
        unknownInt1 = r.readLEUInt32()
        unknownInt2 = r.readLEUInt32()
        unknownShort3 = r.readLEUInt16()
        numParticles = r.readLEUInt16(); let count = Int(numParticles)
        numValid = r.readLEUInt16()
        particles = r.readTArray(count * MemoryLayout<Particle>.size, count: count)
        unknownLink = Ref<NiObject>(r)
        particleExtra = Ref<NiParticleModifier>(r)
        unknownLink2 = Ref<NiObject>(r)
        trailer = r.readByte()
    }
}

public class NiBSPArrayController: NiParticleSystemController { }

// Particle Modifiers
public class NiParticleModifier: NiObject {
    public let nextModifier: Ref<NiParticleModifier>
    public let controller: Ptr<NiParticleSystemController>

    init(_ r: BinaryReader) {
        nextModifier = Ref<NiParticleModifier>(r)
        controller = Ptr<NiParticleSystemController>(r)
    }
}

public class NiGravity: NiParticleModifier {
    public var unknownFloat1: Float!
    public var force: Float!
    public var type: FieldType!
    public var position: float3!
    public var direction: float3!

    override init(_ r: BinaryReader) {
        super.init(r)
        unknownFloat1 = r.readLESingle()
        force = r.readLESingle()
        type = FieldType(rawValue: r.readLEUInt32())!
        position = r.readLEFloat3()
        direction = r.readLEFloat3()
    }
}

public class NiParticleBomb: NiParticleModifier {
    public var decay: Float!
    public var duration: Float!
    public var deltaV: Float!
    public var start: Float!
    public var decayType: DecayType!
    public var position: float3!
    public var direction: float3!

    override init(_ r: BinaryReader) {
        super.init(r)
        decay = r.readLESingle()
        duration = r.readLESingle()
        deltaV = r.readLESingle()
        start = r.readLESingle()
        decayType = DecayType(rawValue: r.readLEUInt32())!
        position = r.readLEFloat3()
        direction = r.readLEFloat3()
    }
}

public class NiParticleColorModifier: NiParticleModifier {
    public var colorData: Ref<NiColorData>!

    override init(_ r: BinaryReader) {
        super.init(r)
        colorData = Ref<NiColorData>(r)
    }
}

public class NiParticleGrowFade: NiParticleModifier {
    public var grow: Float!
    public var fade: Float!

    override init(_ r: BinaryReader) {
        super.init(r)
        grow = r.readLESingle()
        fade = r.readLESingle()
    }
}

public class NiParticleMeshModifier: NiParticleModifier {
    public var numParticleMeshes: UInt32!
    public var particleMeshes: [Ref<NiAVObject>]!

    override init(_ r: BinaryReader) {
        super.init(r)
        numParticleMeshes = r.readLEUInt32(); let count = Int(numParticleMeshes)
        particleMeshes = [Ref<NiAVObject>](); particleMeshes.reserveCapacity(count)
        for _ in 0..<count { particleMeshes.append(Ref<NiAVObject>(r)) }
    }
}

public class NiParticleRotation: NiParticleModifier {
    public var randomInitialAxis: UInt8!
    public var initialAxis: float3!
    public var rotationSpeed: Float!

    override init(_ r: BinaryReader) {
        super.init(r)
        randomInitialAxis = r.readByte()
        initialAxis = r.readLEFloat3()
        rotationSpeed = r.readLESingle()
    }
}

// Controllers
public class NiTimeController: NiObject {
    public let nextController: Ref<NiTimeController>
    public let flags: UInt16
    public let frequency: Float
    public let phase: Float
    public let startTime: Float
    public let stopTime: Float
    public let target: Ptr<NiObjectNET>

    init(_ r: BinaryReader) {
        nextController = Ref<NiTimeController>(r)
        flags = r.readLEUInt16()
        frequency = r.readLESingle()
        phase = r.readLESingle()
        startTime = r.readLESingle()
        stopTime = r.readLESingle()
        target = Ptr<NiObjectNET>(r)
    }
}

public class NiUVController: NiTimeController {
    public var unknownShort: UInt16!
    public var data: Ref<NiUVData>!

    override init(_ r: BinaryReader) {
        super.init(r)
        unknownShort = r.readLEUInt16()
        data = Ref<NiUVData>(r)
    }
}

public class NiInterpController: NiTimeController { }

public class NiSingleInterpController: NiInterpController { }

public class NiKeyframeController: NiSingleInterpController {
    public var data: Ref<NiKeyframeData>!

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiKeyframeData>(r)
    }
}

public class NiGeomMorpherController: NiInterpController {
    public var data: Ref<NiMorphData>!
    public var alwaysUpdate: UInt8!

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiMorphData>(r)
        alwaysUpdate = r.readByte()
    }
}

public class NiBoolInterpController: NiSingleInterpController { }

public class NiVisController: NiBoolInterpController {
    public var data: Ref<NiVisData>!

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiVisData>(r)
    }
}

public class NiFloatInterpController: NiSingleInterpController { }

public class NiAlphaController: NiFloatInterpController {
    public var data: Ref<NiFloatData>!

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiFloatData>(r)
    }
}

// Skin Stuff
public class NiSkinInstance: NiObject {
    public let data: Ref<NiSkinData>
    public let skeletonRoot: Ptr<NiNode>
    public let numBones: UInt32
    public var bones: [Ptr<NiNode>]

    init(_ r: BinaryReader) {
        data = Ref<NiSkinData>(r)
        skeletonRoot = Ptr<NiNode>(r)
        numBones = r.readLEUInt32(); let count = Int(numBones)
        bones = [Ptr<NiNode>](); bones.reserveCapacity(count)
        for _ in 0..<count { bones.append(Ptr<NiNode>(r)) }
    }
}

public class NiSkinData: NiObject {
    public let skinTransform: SkinTransform
    public let numBones: UInt32
    public let skinPartition: Ref<NiSkinPartition>
    public var boneList: [SkinData]

    init(_ r: BinaryReader) {
        skinTransform = SkinTransform(r)
        numBones = r.readLEUInt32(); let count = Int(numBones)
        skinPartition = Ref<NiSkinPartition>(r)
        boneList = [SkinData](); boneList.reserveCapacity(count)
        for _ in 0..<count { boneList.append(SkinData(r)) }
    }
}

public class NiSkinPartition: NiObject { }

// Miscellaneous
public class NiTexture: NiObjectNET {
    override init(_ r: BinaryReader) {
        super.init(r)
    }
}

public class NiSourceTexture: NiTexture {
    public var useExternal: UInt8!
    public var fileName: String!
    public var pixelLayout: PixelLayout!
    public var useMipMaps: MipMapFormat!
    public var alphaFormat: AlphaFormat!
    public var isStatic: UInt8!

    override init(_ r: BinaryReader) {
        super.init(r)
        useExternal = r.readByte()
        fileName = r.readLELength32PrefixedASCIIString()
        pixelLayout = PixelLayout(rawValue: r.readLEUInt32())!
        useMipMaps = MipMapFormat(rawValue: r.readLEUInt32())!
        alphaFormat = AlphaFormat(rawValue: r.readLEUInt32())!
        isStatic = r.readByte()
    }
}

public class NiPoint3InterpController: NiSingleInterpController {
    public var data: Ref<NiPosData>!

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiPosData>(r)
    }
}

public class NiMaterialProperty: NiProperty {
    public var flags: UInt16!
    public var ambientColor: Color3!
    public var diffuseColor: Color3!
    public var specularColor: Color3!
    public var emissiveColor: Color3!
    public var glossiness: Float!
    public var alpha: Float!

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = NiReaderUtils.readFlags(r)
        ambientColor = r.readT(3 << 2)
        diffuseColor = r.readT(3 << 2)
        specularColor = r.readT(3 << 2)
        emissiveColor = r.readT(3 << 2)
        glossiness = r.readLESingle()
        alpha = r.readLESingle()
    }
}

public class NiMaterialColorController: NiPoint3InterpController { }

public class NiDynamicEffect: NiAVObject {
    public var numAffectedNodeListPointers: UInt32!
    public var affectedNodeListPointers: [UInt32]!

    override init(_ r: BinaryReader) {
        super.init(r)
        numAffectedNodeListPointers = r.readLEUInt32(); let count = Int(numAffectedNodeListPointers)
        affectedNodeListPointers = r.readTArray(count << 2, count: count)
    }
}

public class NiTextureEffect: NiDynamicEffect {
    public var modelProjectionMatrix: simd_float4x4!
    public var modelProjectionTransform: float3!
    public var textureFiltering: TexFilterMode!
    public var textureClamping: TexClampMode!
    public var textureType: EffectType!
    public var coordinateGenerationType: CoordGenType!
    public var sourceTexture: Ref<NiSourceTexture>!
    public var clippingPlane: UInt8!
    public var unknownVector: float3!
    public var unknownFloat: Float!
    public var ps2L: Int16!
    public var ps2K: Int16!
    public var unknownShort: UInt16!

    override init(_ r: BinaryReader) {
        super.init(r)
        modelProjectionMatrix = NiReaderUtils.read3x3RotationMatrix(r)
        modelProjectionTransform = r.readLEFloat3()
        textureFiltering = TexFilterMode(rawValue: r.readLEUInt32())!
        textureClamping = TexClampMode(rawValue: r.readLEUInt32())!
        textureType = EffectType(rawValue: r.readLEUInt32())!
        coordinateGenerationType = CoordGenType(rawValue: r.readLEUInt32())!
        sourceTexture = Ref<NiSourceTexture>(r)
        clippingPlane = r.readByte()
        unknownVector = r.readLEFloat3()
        unknownFloat = r.readLESingle()
        ps2L = r.readLEInt16()
        ps2K = r.readLEInt16()
        unknownShort = r.readLEUInt16()
    }
}
