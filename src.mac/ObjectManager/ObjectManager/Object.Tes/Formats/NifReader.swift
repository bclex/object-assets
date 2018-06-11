//
//  NifReader.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public struct Ptr<T> {
    public var value: Int32
    public var isNull: Bool { return Value < 0 }

    init(_ r: BinaryReader) {
        value = reader.readLEInt32()
    }
}

// Refers to an object after the current one in the hierarchy.
public struct Ref<T> {
    public value: Int32
    public var isNull: Bool { return Value < 0 }

    init(_ r: BinaryReader) {
        value = reader.readLEInt32()
    }
}

public class NiReaderUtils
{
    public static func readPtr<T>(_ r: BinaryReader) -> Ptr<T> {
        return Ptr<T>(r)
    }

    public static func readRef<T>(_ r: BinaryReader) -> Ref<T> {
        return Ref<T>(r)
    }

    public static func readLengthPrefixedRefs32<T>(_ r: BinaryReader) -> [Ref<T>] {
        var refs = [Ref<T>](); refs.reserveCapacity(r.readLEUInt32())
        for i in 0..<refs.capacity {
            refs[i] = readRef<T>(r)
        }
        return refs
    }

    public static func readFlags(_ r: BinaryReader) -> UInt16 {
        return r.readLEUInt16()
    }

    public static func Read<T>(_ r: BinaryReader) -> T {
        if T.self == Float.self { return T(r.readLESingle()) }
        else if T.self == UInt8.self { return T(r.readByte()) }
        else if T.self == String.self { return T(r.readLELength32PrefixedASCIIString()) }
        else if T.self == Vector3.self { return T(r.readLEVector3()) }
        else if T.self == Quaternion.self { return T(r.readLEQuaternionWFirst()) }
        else if T.self == Color4.self { let color = Color4(r); return T(color) }
        else fatalError("Tried to read an unsupported type.")
    }

    public static func readNiObject(_ r: BinaryReader) -> NiObject? {
        var nodeType = r.readLELength32PrefixedASCIIString()
        if (nodeType == "NiNode") { return NiNode(r) }
        else if (nodeType == "NiTriShape") { return NiTriShape(r) }
        else if (nodeType == "NiTexturingProperty") { return NiTexturingProperty(r) }
        else if (nodeType == "NiSourceTexture") { return NiSourceTexture(r) }
        else if (nodeType == "NiMaterialProperty") { return NiMaterialProperty(r) }
        else if (nodeType == "NiMaterialColorController") { return NiMaterialColorController(r) }
        else if (nodeType == "NiTriShapeData") { return NiTriShapeData(r) }
        else if (nodeType == "RootCollisionNode") { return RootCollisionNode(r) }
        else if (nodeType == "NiStringExtraData") { return NiStringExtraData(r) }
        else if (nodeType == "NiSkinInstance") { return NiSkinInstance(r) }
        else if (nodeType == "NiSkinData") { return NiSkinData(r) }
        else if (nodeType == "NiAlphaProperty") { return NiAlphaProperty(r) }
        else if (nodeType == "NiZBufferProperty") { return NiZBufferProperty(r) }
        else if (nodeType == "NiVertexColorProperty") { return NiVertexColorProperty(r) }
        else if (nodeType == "NiBSAnimationNode") { return NiBSAnimationNode(r) }
        else if (nodeType == "NiBSParticleNode") { return NiBSParticleNode(r) }
        else if (nodeType == "NiParticles") { return NiParticles(r) }
        else if (nodeType == "NiParticlesData") { return NiParticlesData(r) }
        else if (nodeType == "NiRotatingParticles") { return NiRotatingParticles(r) }
        else if (nodeType == "NiRotatingParticlesData") { return NiRotatingParticlesData(r) }
        else if (nodeType == "NiAutoNormalParticles") { return NiAutoNormalParticles(r) }
        else if (nodeType == "NiAutoNormalParticlesData") { return NiAutoNormalParticlesData(r) }
        else if (nodeType == "NiUVController") { return NiUVController(r) }
        else if (nodeType == "NiUVData") { return NiUVData(r) }
        else if (nodeType == "NiTextureEffect") { return NiTextureEffect(r) }
        else if (nodeType == "NiTextKeyExtraData") { return NiTextKeyExtraData(r) }
        else if (nodeType == "NiVertWeightsExtraData") { return NiVertWeightsExtraData(r) }
        else if (nodeType == "NiParticleSystemController") { return NiParticleSystemController(r) }
        else if (nodeType == "NiBSPArrayController") { return NiBSPArrayController(r) }
        else if (nodeType == "NiGravity") { return NiGravity(r) }
        else if (nodeType == "NiParticleBomb") { return NiParticleBomb(r) }
        else if (nodeType == "NiParticleColorModifier") { return NiParticleColorModifier(r) }
        else if (nodeType == "NiParticleGrowFade") { return NiParticleGrowFade(r) }
        else if (nodeType == "NiParticleMeshModifier") { return NiParticleMeshModifier(r) }
        else if (nodeType == "NiParticleRotation") { return NiParticleRotation(r) }
        else if (nodeType == "NiKeyframeController") { return NiKeyframeController(r) }
        else if (nodeType == "NiKeyframeData") { return NiKeyframeData(r) }
        else if (nodeType == "NiColorData") { return NiColorData(r) }
        else if (nodeType == "NiGeomMorpherController") { return NiGeomMorpherController(r) }
        else if (nodeType == "NiMorphData") { return NiMorphData(r) }
        else if (nodeType == "AvoidNode") { return AvoidNode(r) }
        else if (nodeType == "NiVisController") { return NiVisController(r) }
        else if (nodeType == "NiVisData") { return NiVisData(r) }
        else if (nodeType == "NiAlphaController") { return NiAlphaController(r) }
        else if (nodeType == "NiFloatData") { return NiFloatData(r) }
        else if (nodeType == "NiPosData") { return NiPosData(r) }
        else if (nodeType == "NiBillboardNode") { return NiBillboardNode(r) }
        else if (nodeType == "NiShadeProperty") { return NiShadeProperty(r) }
        else { debugPrint("Tried to read an unsupported NiObject type (\(nodeType))."); return nil }
    }

    public static func read3x3RotationMatrix(_ r: BinaryReader) -> Matrix4x4 {
        return r.readLERowMajorMatrix3x3()
    }
}

public class NiFile {
    public let name: String
    public var header: NiHeader
    public var blocks: [NiObject]
    public var footer: NiFooter

    init(name: String) {
        self.name = name
    }
    convenience init(_ r: BinaryReader, name: String) {
        self.init(name: name)
        header = NiHeader(r)
        blocks = [NiObject](); blocks.reserveCapacity(Header.NumBlocks)
        for i in 0..<Header.NumBlocks {
            Blocks[i] = NiReaderUtils.readNiObject(r)
        }
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
        public let translation: Vector3
        public let rotation: Matrix4x4
        public let radius: Vector3

        init(_ r: BinaryReader) {
            unknownInt = r.readLEUInt32()
            translation = r.readLEVector3()
            rotation = NiReaderUtils.read3x3RotationMatrix(r)
            radius = r.readLEVector3()
        }
    }

    public struct Color3 {
        public let r: Float
        public let g: Float
        public let b: Float

        init(_ r: BinaryReader) {
            self.r = r.readLESingle()
            g = r.readLESingle()
            b = r.readLESingle()
        }
    }

    public struct Color4 {
        public let r: Float
        public let g: Float
        public let b: Float
        public let a: Float

        init(_ r: BinaryReader) {
            this.r = r.readLESingle()
            g = r.readLESingle()
            b = r.readLESingle()
            a = r.readLESingle()
        }
    }

    public struct TexDesc {
        public let source: Ref<NiSourceTexture>
        public let clampMode: TexClampMode
        public let filterMode: TexFilterMode
        public let uvset: UInt32
        public let ps2L: Int16
        public let ps2K: Int16
        public let unknown1: UInt16

        init(_ r: BinaryReader) {
            source = NiReaderUtils.readRef<NiSourceTexture>(r)
            clampMode = TexClampMode(rawValue: r.readLEUInt32())
            filterMode = TexFilterMode(rawValue: r.readLEUInt32())
            uvSet = r.readLEUInt32()
            ps2L = r.readLEInt16()
            ps2K = r.readLEInt16()
            unknown1 = r.readLEUInt16()
        }
    }

    public struct TexCoord {
        public let u: Float
        public let v: Float

        init(_ r: BinaryReader) {
            u = r.readLESingle()
            v = r.readLESingle()
        }
    }

    public struct Triangle {
        public let v1: UInt16
        public let v2: UInt16
        public let v3: UInt16

        init(_ r: BinaryReader) {
            v1 = r.readLEUInt16()
            v2 = r.readLEUInt16()
            v3 = r.readLEUInt16()
        }
    }

    public struct MatchGroup {
        public let numVertices: UInt16
        public let vertexIndices: [UInt16]

        init(_ r: BinaryReader) {
            numVertices = r.readLEUInt16()
            vertexIndices = [UInt16](); vertexIndices.reserveCapacity(numVertices)
            for i in 0..<vertexIndices.capacity {
                vertexIndices[i] = r.readLEUInt16();
            }
        }
    }

    public struct TBC {
        public let t: Float
        public let b: Float
        public let c: Float

        init(_ r: BinaryReader) {
            t = r.readLESingle()
            b = r.readLESingle()
            c = r.readLESingle()
        }
    }

    public class Key<T> {
        public let time: Float
        public let value: T
        public let forward: T
        public let backward: T
        public let tbc: TBC

        init(_ r: BinaryReader, keyType: KeyType) {
            time = r.readLESingle()
            value = NiReaderUtils.read<T>(r)
            if keyType == .QUADRATIC_KEY {
                forward = NiReaderUtils.read<T>(r)
                backward = NiReaderUtils.read<T>(r)
            }
            else if keyType == .TBC_KEY {
                tbc = TBC(r)
            }
        }
    }

    public class KeyGroup<T> {
        public let numKeys: UInt32
        public let interpolation: KeyType
        public let keys: [Key<T>]

        init(_ r: BinaryReader) {
            numKeys = r.readLEUInt32()
            if numKeys != 0 {
                interpolation = KeyType(rawValue: r.readLEUInt32())
            }
            keys = [Key<T>](); keys.reserveCapacity(numKeys)
            for i in 0..<keys.capacity {
                keys[i] = new Key<T>(r, interpolation)
            }
        }
    }

    public class QuatKey<T> {
        public let time: Float
        public let value: T
        public let tbc: TBC

        init(_ r: BinaryReader, keyType: KeyType) {
            time = r.readLESingle()
            if keyType != .XYZ_ROTATION_KEY) {
                value = NiReaderUtils.read<T>(r)
            }
            if keyType == .TBC_KEY {
                tbc = new TBC(r)
            }
        }
    }

    public struct SkinData {
        public let skinTransform: SkinTransform
        public let boundingSphereOffset: Vector3
        public let boundingSphereRadius: Float
        public let numVertices: UInt16
        public let vertexWeights: [SkinWeight]

        init(_ r: BinaryReader) {
            skinTransform = SkinTransform(r)
            boundingSphereOffset = r.readLEVector3()
            boundingSphereRadius = r.readLESingle()
            numVertices = r.readLEUInt16()
            vertexWeights = [SkinWeight](); vertexWeights.reserveCapacity(numVertices)
            for i in 0..<vertexWeights.capacity {
                vertexWeights[i] = SkinWeight(r)
            }
        }
    }

    public struct SkinWeight {
        public let index: UInt16
        public let weight: Float

        init(_ r: BinaryReader) {
            index = r.readLEUInt16()
            weight = r.readLESingle()
        }
    }

    public struct SkinTransform {
        public let rotation: Matrix4x4
        public let translation: Vector3
        public let scale: Float

        init(_ r: BinaryReader) {
            rotation = NiReaderUtils.read3x3RotationMatrix(r)
            translation = r.readLEVector3()
            scale = r.readLESingle()
        }
    }

    public struct Particle {
        public let velocity: Vector3
        public let unknownVector: Vector3
        public let lifetime: Float
        public let lifespan: Float
        public let timestamp: Float
        public let unknownShort: UInt16
        public let vertexId: UInt16

        init(_ r: BinaryReader) {
            velocity = r.readLEVector3()
            unknownVector = r.readLEVector3()
            lifetime = r.readLESingle()
            lifespan = r.readLESingle()
            timestamp = r.readLESingle()
            unknownShort = r.readLEUInt16()
            vertexId = r.readLEUInt16()
        }
    }

    public struct Morph {
        public uint numKeys;
        public KeyType interpolation;
        public Key<float>[] keys;
        public Vector3[] vectors;

        init(_ r: BinaryReader, numVertices: UInt32) {
            numKeys = r.readLEUInt32()
            interpolation = KeyType(rawValue: r.readLEUInt32())
            keys = [Key<float>](); keys.reserveCapacity(numKeys)
            for i in 0..<keys.capacity {
                keys[i] = Key<float>(r, interpolation)
            }
            vectors = [Vector3](); vectors.reserveCapacity(numVertices)
            for i in 0..<vectors.capacity {
                vectors[i] = r.readLEVector3()
            }
        }
    }

    // MARK: A

    public struct NiHeader {
        public let Str: [UInt8]; // 40 bytes (including \n)
        public let Version: UInt32
        public let NumBlocks: UInt32

        init(_ r: BinaryReader) {
            str = r.readBytes(40)
            version = r.readLEUInt32()
            numBlocks = r.readLEUInt32()
        }
    }

    public struct NiFooter {
        public let numRoots: UInt32
        public let roots: [Int32]

        init(_ r: BinaryReader) {
            numRoots = r.readLEUInt32()
            roots = [Int](); roots.reserveCapacity(numRoots)
            for i in 0..<numRoots {
                Roots[i] = r.readLEInt32()
            }
        }
    }

    public struct NiObject {
        init(_ r: BinaryReader) {
        }
    }

    public struct NiObjectNET: NiObject {
        public let name: String
        public let extraData: Ref<NiExtraData>
        public let controller: Ref<NiTimeController>

        init(_ r: BinaryReader) {
            super.init(r)
            name = r.readLELength32PrefixedASCIIString()
            extraData = NiReaderUtils.ReadRef<NiExtraData>(r)
            controller = NiReaderUtils.ReadRef<NiTimeController>(r)
        }
    }

    public struct NiAVObject: NiObjectNET {
        public enum NiFlags {
            case Hidden = 0x1
        }

        public let flags: UInt16
        public let translation: Vector3
        public let rotation: Matrix4x4
        public let scale: Float
        public let velocity: Vector3
        //public let numProperties: uint 
        public let properties: [Ref<NiProperty>]
        public let hasBoundingBox: Bool
        public let boundingBox: BoundingBox

        init(_ r: BinaryReader) {
            super.init(r)
            flags = NiReaderUtils.readFlags(r)
            translation = r.readLEVector3()
            rotation = NiReaderUtils.read3x3RotationMatrix(r)
            scale = r.readLESingle()
            velocity = r.readLEVector3()
            properties = NiReaderUtils.readLengthPrefixedRefs32<NiProperty>(r)
            hasBoundingBox = r.readLEBool32()
            if hasBoundingBox {
                boundingBox = BoundingBox(r)
            }
        }
    }

    // Nodes
    public struct NiNode: NiAVObject {
        //public let numChildren: UInt32
        public children: [Ref<NiAVObject>]
        //public let numEffects: UInt32
        public effects: [Ref<NiDynamicEffect>]

        init(_ r: BinaryReader) { {
            super.init(r)
            children = NiReaderUtils.readLengthPrefixedRefs32<NiAVObject>(r)
            effects = NiReaderUtils.readLengthPrefixedRefs32<NiDynamicEffect>(r)
        }
    }

    public struct RootCollisionNode: NiNode { }

    public struct NiBSAnimationNode: NiNode { }

    public struct NiBSParticleNode: NiNode { }

    public struct NiBillboardNode: NiNode { }

    public struct AvoidNode: NiNode { }

    // Geometry
    public struct NiGeometry: NiAVObject {
        public var data: Ref<NiGeometryData>
        public var skinInstance: Ref<NiSkinInstance>

        init(_ r: BinaryReader) {
            super.init(r)
            data = NiReaderUtils.readRef<NiGeometryData>(r)
            skinInstance = NiReaderUtils.readRef<NiSkinInstance>(r)
        }
    }

    public struct NiGeometryData: NiObject {
        public let numVertices: UInt16
        public let hasVertices: Bool
        public let vertices: [Vector3]
        public let hasNormals: Bool
        public let normals: [Vector3]
        public let center: Vector3
        public let radius: Float
        public let hasVertexColors: Bool
        public let vertexColors: [Color4]
        public let numUVSets: UInt16
        public let hasUV: Bool
        public let uvSets: [[TexCoord]]

        init(_ r: BinaryReader) {
            super.init(r)
            numVertices = r.readLEUInt16()
            hasVertices = r.readLEBool32()
            if hasVertices {
                vertices = [Vector3](); vertices.reserveCapacity(numVertices)
                for i in 0..<vertices.capacity {
                    vertices[i] = r.readLEVector3()
                }
            }
            hasNormals = r.readLEBool32()
            if hasNormals {
                normals = [Vector3])(); normals.reserveCapacity(numVertices)
                for i in 0..<normals.capacity {
                    normals[i] = r.readLEVector3()
                }
            }
            center = r.readLEVector3()
            radius = r.readLESingle()
            hasVertexColors = r.readLEBool32()
            if hasVertexColors {
                certexColors = [Color4](); certexColors.reserveCapacity(numVertices)
                for i in 0..<vertexColors.capacity {
                    vertexColors[i] = Color4(r)
                }
            }
            numUVSets = r.readLEUInt16()
            hasUV = r.readLEBool32()
            if hasUV {
                uvSets = [[TextCord]](); uvSets.reserveCapacity(NumUVSets, NumVertices)
                for i in 0..numUVSets {
                    for j in 0..<numVertices {
                        UVSets[i, j] = TexCoord(r)
                    }
                }
            }
        }
    }

    public struct NiTriBasedGeom: NiGeometry {
        init(_ r: BinaryReader) {
            super.init(r)
        }
    }

    public struct NiTriBasedGeomData: NiGeometryData {
        public let numTriangles: UInt16

        init(_ r: BinaryReader) {
            super.init(r)
            numTriangles = r.readLEUInt16()
        }
    }

    public struct NiTriShape: NiTriBasedGeom {
        init(_ r: BinaryReader) {
            super.init(r)
        }
    }

    public struct NiTriShapeData: NiTriBasedGeomData {
        public let numTrianglePoints: UInt32
        public let triangles: [Triangle]
        public let numMatchGroups: UInt16
        public let matchGroups: [MatchGroup]

        init(_ r: BinaryReader) {
            super.init(r)
            numTrianglePoints = r.readLEUInt32()
            triangles = [Triangle](); triangles.reserveCapacity(numTriangles)
            for i in 0..<triangles.capacity {
                triangles[i] = Triangle(r)
            }
            numMatchGroups = r.readLEUInt16()
            matchGroups = [MatchGroup](); matchGroups.reserveCapacity(numMatchGroups)
            for i in 0..<matchGroups.capacity {
                matchGroups[i] = MatchGroup(r)
            }
        }
    }

    // Properties
    public struct NiProperty: NiObjectNET {
        init(_ r: BinaryReader) {
            super.init(r)
        }
    }

    public struct NiTexturingProperty: NiProperty {
        public let flags: UInt16
        public let applyMode: ApplyMode
        public let textureCount: UInt32
        //public let hasBaseTexture: Bool
        public let baseTexture: TexDesc
        //public let hasDarkTexture: Bool
        public let darkTexture: TexDesc
        //public let hasDetailTexture: Bool
        public let detailTexture: TexDesc
        //public let hasGlossTexture: Bool
        public let glossTexture: TexDesc
        //public let hasGlowTexture: Bool
        public let glowTexture: TexDesc
        //public let hasBumpMapTexture: Bool
        public let bumpMapTexture: TexDesc
        //public let hasDecal0Texture: Bool
        public let decal0Texture: TexDesc

        init(_ r: BinaryReader) {
            super.init(r)
            flags = NiReaderUtils.readFlags(r)
            applyMode = ApplyMode(rawValue: r.readLEUInt32())
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

    public struct NiAlphaProperty: NiProperty {
        public let flags: UInt16
        public let threshold: UInt8

        init(_ r: BinaryReader) {
            super.init(r)
            flags = r.readLEUInt16()
            threshold = r.readByte()
        }
    }

    public struct NiZBufferProperty: NiProperty {
        public let flags: UInt16

        init(_ r: BinaryReader) {
            super.init(r)
            flags = r.readLEUInt16()
        }
    }

    public struct NiVertexColorProperty: NiProperty {
        public let flags: UInt16
        public let vertexMode: VertMode
        public let lightingMode: LightMode

        init(_ r: BinaryReader) {
            super.init(r)
            flags = NiReaderUtils.readFlags(r)
            vertexMode = VertMode(rawValue: r.readLEUInt32())
            lightingMode = LightMode(rawValue: r.readLEUInt32())
        }
    }

    public struct NiShadeProperty: NiProperty {
        public let flags: UInt16

        init(_ r: BinaryReader) {
            super.init(r)
            flags = NiReaderUtils.readFlags(r)
        }
    }

    // Data
    public struct NiUVData: NiObject {
        public let uvGroups: [KeyGroup<Float>]

        init(_ r: BinaryReader) {
            super.init(r)
            uvGroups = [KeyGroup<Float>](); uvGroups.reserveCapacity(4)
            for i in 0..<uvGroups.capacity {
                uvGroups[i] = KeyGroup<Float>(r);
            }
        }
    }

    public struct NiKeyframeData: NiObject {
        public let numRotationKeys: UInt32
        public let rotationType: KeyType
        public let quaternionKeys: [QuatKey<Quaternion>]
        public let unknownFloat: Float
        public let xyzRotations: [KeyGroup<float>]
        public let translations: KeyGroup<Vector3>
        public let scales: KeyGroup<float>

        init(_ r: BinaryReader) {
            super.init(r)
            numRotationKeys = r.readLEUInt32()
            if numRotationKeys != 0 {
                rotationType = KeyType(rawValue: r.readLEUInt32())
                if rotationType != .XYZ_ROTATION_KEY {
                    quaternionKeys = [QuatKey<Quaternion>](); quaternionKeys.reserveCapacity(numRotationKeys)
                    for i in 0..<quaternionKeys.capacity {
                        quaternionKeys[i] = QuatKey<Quaternion>(r, rotationType)
                    }
                }
                else {
                    unknownFloat = r.readLESingle()
                    xyzRotations = [KeyGroup<Float>](); xyzRotations.reserveCapacity(3)
                    for i in 0..<xyzRotations.capacity {
                        xyzRotations[i] = KeyGroup<Float>(r)
                    }
                }
            }
            translations = KeyGroup<Vector3>(r)
            scales = KeyGroup<Float>(r)
        }
    }

    public class NiColorData: NiObject {
        public let data: KeyGroup<Color4>

        init(_ r: BinaryReader) {
            super.init(r)
            data = KeyGroup<Color4>(r)
        }
    }

    public struct NiMorphData: NiObject {
        public let numMorphs: UInt32
        public let numVertices: UInt32
        public let relativeTargets: UInt8
        public let morphs: [Morph]

        init(_ r: BinaryReader) {
            super.init(r)
            numMorphs = r.readLEUInt32()
            numVertices = r.readLEUInt32()
            relativeTargets = r.readByte()
            morphs = [Morph](); morphs.reserveCapacity(numMorphs)
            for i in 0..<morphs.capacity {
                morphs[i] = Morph(r, numVertices)
            }
        }
    }

    public struct NiVisData: NiObject {
        public let numKeys: UInt32
        public let keys: [Key<UInt8>]

        init(_ r: BinaryReader) {
            super.init(r)
            numKeys = r.readLEUInt32()
            keys = [Key<UInt8>](); keys.reserveCapacity(numKeys)
            for i in 0..<Keys.capacity {
                keys[i] = Key<UInt8>(r, .LINEAR_KEY)
            }
        }
    }

    public struct NiFloatData: NiObject {
        public let data: KeyGroup<Float>

        init(_ r: BinaryReader) {
            super.init(r)
            data = KeyGroup<Float>(r)
        }
    }

    public struct NiPosData: NiObject {
        public let data: KeyGroup<Vector3>

        init(_ r: BinaryReader) {
            super.init(r)
            data = KeyGroup<Vector3>(r)
        }
    }

    public struct NiExtraData: NiObject {
        public let nextExtraData: Ref<NiExtraData>

        init(_ r: BinaryReader) {
            super.init(r)
            nextExtraData = NiReaderUtils.readRef<NiExtraData>(r)
        }
    }

    public struct NiStringExtraData: NiExtraData {
        public let bytesRemaining: UIt32
        public let str: String

        init(_ r: BinaryReader) {
            super.init(r)
            bytesRemaining = r.readLEUInt32()
            str = r.readLELength32PrefixedASCIIString()
        }
    }

    public struct NiTextKeyExtraData: NiExtraData {
        public let unknownInt1: UInt32
        public let numTextKeys: UInt32
        public let textKeys: [Key<String>]

        init(_ r: BinaryReader) {
            super.init(r)
            unknownInt1 = r.readLEUInt32()
            numTextKeys = r.readLEUInt32()
            textKeys = [Key<String>](); textKeys.reserveCapacity(numTextKeys)
            for i in 0..<textKeys.capacity {
                textKeys[i] = Key<String>(r, .LINEAR_KEY)
            }
        }
    }

    public struct NiVertWeightsExtraData: NiExtraData {
        public let numBytes: UInt32
        public let numVertices: UInt16
        public let weights: [Float]

        init(_ r: BinaryReader) {
            super.init(r)
            numBytes = r.readLEUInt32()
            numVertices = r.readLEUInt16()
            weights = [Float](); weights.reserveCapacity(numVertices)
            for i in 0..<weights.capacity {
                weights[i] = r.readLESingle()
            }
        }
    }

    // Particles
    public struct NiParticles: NiGeometry { }

    public struct NiParticlesData: NiGeometryData {
        public let numParticles: UInt16
        public let particleRadius: Float
        public let numActive: UInt16
        public let hasSizes: Bool
        public let sizes: [Float]

        init(_ r: BinaryReader) {
            super.init(r)
            numParticles = r.readLEUInt16()
            particleRadius = r.readLESingle()
            numActive = r.readLEUInt16()
            hasSizes = r.readLEBool32()
            if hasSizes {
                sizes = [Float](); sizes.reserveCapacity(numVertices)
                for i in 0..<sizes.capacity {
                    sizes[i] = r.readLESingle()
                }
            }
        }
    }

    public struct NiRotatingParticles: NiParticles { }

    public struct NiRotatingParticlesData: NiParticlesData {
        public let hasRotations: Bool
        public let rotations: [Quaternion]

        init(_ r: BinaryReader) {
            super.init(r)
            hasRotations = r.readLEBool32()
            if hasRotations {
                rotations = [Quaternion](); rotations.reserveCapacity(numVertices)
                for i in 0..<rotations.capacity {
                    rotations[i] = r.readLEQuaternionWFirst()
                }
            }
        }
    }

    public struct NiAutoNormalParticles: NiParticles { }

    public struct NiAutoNormalParticlesData: NiParticlesData { }

    public struct NiParticleSystemController: NiTimeController {
        public let speed: Float
        public let speedRandom: Float
        public let verticalDirection: Float
        public let verticalAngle: Float
        public let horizontalDirection: Float
        public let horizontalAngle: Float
        public let unknownNormal: Vector3
        public let unknownColor: Color4
        public let size: Float
        public let emitStartTime: Float
        public let emitStopTime: Float
        public let unknownByte: UInt8
        public let emitRate: Float
        public let lifetime: Float
        public let lifetimeRandom: Float
        public let emitFlags: UInt16
        public let startRandom: Vector3
        public let emitter: Ptr<NiObject>
        public let unknownShort2: UInt16
        public let unknownFloat13: Float
        public let unknownInt1: UInt32
        public let unknownInt2: UInt32
        public let unknownShort3: UInt16
        public let numParticles: UInt16
        public let numValid: UInt16
        public let particles: [Particle]
        public let unknownLink: Ref<NiObject>
        public let particleExtra: Ref<NiParticleModifier>
        public let unknownLink2: Ref<NiObject>
        public let trailer: UInt8

        init(_ r: BinaryReader) {
            super.init(r)
            speed = r.readLESingle()
            speedRandom = r.readLESingle()
            verticalDirection = r.readLESingle()
            verticalAngle = r.readLESingle()
            horizontalDirection = r.readLESingle()
            horizontalAngle = r.readLESingle()
            unknownNormal = r.readLEVector3()
            unknownColor = Color4(r)
            size = r.readLESingle()
            emitStartTime = r.readLESingle()
            emitStopTime = r.readLESingle()
            unknownByte = r.readByte()
            emitRate = r.readLESingle()
            lifetime = r.readLESingle()
            lifetimeRandom = r.readLESingle()
            emitFlags = r.readLEUInt16()
            startRandom = r.readLEVector3()
            emitter = NiReaderUtils.readPtr<NiObject>(r)
            unknownShort2 = r.readLEUInt16()
            unknownFloat13 = r.readLESingle()
            unknownInt1 = r.readLEUInt32()
            unknownInt2 = r.readLEUInt32()
            unknownShort3 = r.readLEUInt16()
            numParticles = r.readLEUInt16()
            numValid = r.readLEUInt16()
            particles = [Particle](); particles.reserveCapacity(numParticles)
            for i in 0..i<particles.capacity {
                particles[i] = Particle(r)
            }
            UnknownLink = NiReaderUtils.readRef<NiObject>(r)
            ParticleExtra = NiReaderUtils.readRef<NiParticleModifier>(r)
            UnknownLink2 = NiReaderUtils.readRef<NiObject>(r)
            Trailer = r.readByte()
        }
    }

    public struct NiBSPArrayController: NiParticleSystemController { }

    // Particle Modifiers
    public struct NiParticleModifier: NiObject {
        public let nextModifier: Ref<NiParticleModifier>
        public let controller: Ptr<NiParticleSystemController>

        init(_ r: BinaryReader) {
            super.init(r)
            nextModifier = NiReaderUtils.readRef<NiParticleModifier>(r)
            controller = NiReaderUtils.readPtr<NiParticleSystemController>(r)
        }
    }

    public struct NiGravity: NiParticleModifier {
        public let UnknownFloat1: Float
        public let Force: Float
        public let Type: FieldType
        public let Position: Vector3
        public let Direction: Vector3

        init(_ r: BinaryReader) {
            super.init(r)
            unknownFloat1 = r.readLESingle()
            force = r.readLESingle()
            type = FieldType(rawValue: r.readLEUInt32())
            position = r.readLEVector3()
            direction = r.readLEVector3()
        }
    }

    public struct NiParticleBomb: NiParticleModifier {
        public let decay: Float
        public let duration: Float
        public let deltaV: Float
        public let start: Float
        public let decayType: DecayType
        public let position: Vector3
        public let direction: Vector3

        init(_ r: BinaryReader) {
            super.init(r)
            decay = r.readLESingle()
            duration = r.readLESingle()
            deltaV = r.readLESingle()
            start = r.readLESingle()
            decayType = DecayType(rawValue: r.readLEUInt32())
            position = r.readLEVector3()
            direction = r.readLEVector3()
        }
    }

    public struct NiParticleColorModifier: NiParticleModifier {
        public let colorData: Ref<NiColorData>

        init(_ r: BinaryReader) {
            super.init(r)
            ColorData = NiReaderUtils.readRef<NiColorData>(r)
        }
    }

    public struct NiParticleGrowFade: NiParticleModifier {
        public let grow: Float
        public let fade: Float

        init(_ r: BinaryReader) {
            super.init(r)
            grow = r.readLESingle()
            fade = r.readLESingle()
        }
    }

    public struct NiParticleMeshModifier: NiParticleModifier {
        public let numParticleMeshes: UInt32
        public let particleMeshes: Ref<NiAVObject>[]

        init(_ r: BinaryReader) {
            super.init(r)
            numParticleMeshes = r.readLEUInt32()
            particleMeshes = [Ref<NiAVObject>](); particleMeshes.reserveCapacity(numParticleMeshes)
            for i in 0..<particleMeshes.capacity {
                particleMeshes[i] = NiReaderUtils.readRef<NiAVObject>(r)
            }
        }
    }

    public struct NiParticleRotation: NiParticleModifier {
        public let randomInitialAxis: UInt8
        public let initialAxis: Vector3
        public let rotationSpeed: Float

        init(_ r: BinaryReader) {
            super.init(r)
            randomInitialAxis = r.readByte()
            initialAxis = r.readLEVector3()
            rotationSpeed = r.readLESingle()
        }
    }

    // Controllers
    public struct NiTimeController: NiObject {
        public let nextController: Ref<NiTimeController>
        public let flags: UInt16
        public let frequency: Float
        public let phase: Float
        public let startTime: Float
        public let stopTime: Float
        public let target: Ptr<NiObjectNET>

        init(_ r: BinaryReader) {
            super.init(r)
            nextController = NiReaderUtils.readRef<NiTimeController>(r)
            flags = r.readLEUInt16()
            frequency = r.readLESingle()
            phase = r.readLESingle()
            startTime = r.readLESingle()
            stopTime = r.readLESingle()
            target = NiReaderUtils.readPtr<NiObjectNET>(r)
        }
    }

    public struct NiUVController: NiTimeController {
        public let unknownShort: UInt16
        public let data: Ref<NiUVData>

        init(_ r: BinaryReader) {
            super.init(r)
            unknownShort = r.readLEUInt16()
            data = NiReaderUtils.readRef<NiUVData>(r)
        }
    }

    public struct NiInterpController: NiTimeController { }

    public struct NiSingleInterpController: NiInterpController { }

    public struct NiKeyframeController: NiSingleInterpController {
        public let data: Ref<NiKeyframeData>

        init(_ r: BinaryReader) {
            super.init(r)
            data = NiReaderUtils.readRef<NiKeyframeData>(r)
        }
    }

    public struct NiGeomMorpherController: NiInterpController {
        public let data: Ref<NiMorphData>
        public let alwaysUpdate: UInt8 

        init(_ r: BinaryReader) {
            super.init(r)
            data = NiReaderUtils.readRef<NiMorphData>(r)
            alwaysUpdate = r.readByte()
        }
    }

    public struct NiBoolInterpController: NiSingleInterpController { }

    public struct NiVisController: NiBoolInterpController {
        public let data: Ref<NiVisData> 

        init(_ r: BinaryReader) {
            super.init(r)
            data = NiReaderUtils.readRef<NiVisData>(r)
        }
    }

    public struct NiFloatInterpController: NiSingleInterpController { }

    public struct NiAlphaController: NiFloatInterpController {
        public let data: Ref<NiFloatData> 

        init(_ r: BinaryReader) {
            super.init(r)
            data = NiReaderUtils.readRef<NiFloatData>(r)
        }
    }

    // Skin Stuff
    public struct NiSkinInstance: NiObject {
        public let data: Ref<NiSkinData> 
        public let skeletonRoot: Ptr<NiNode>
        public let numBones: UInt32
        public let bones: [Ptr<NiNode>]

        init(_ r: BinaryReader) {
            super.init(r)
            data = NiReaderUtils.readRef<NiSkinData>(r)
            skeletonRoot = NiReaderUtils.readPtr<NiNode>(r)
            numBones = r.readLEUInt32()
            bones = [Ptr<NiNode>](); bones.reserveCapacity(numBones)
            for i in 0..<bones.capacity {
                bones[i] = NiReaderUtils.readPtr<NiNode>(r)
            }
        }
    }

    public struct NiSkinData: NiObject {
        public let skinTransform: SkinTransform
        public let numBones: UInt32
        public let skinPartition: Ref<NiSkinPartition>
        public let boneList: [SkinData]

        init(_ r: BinaryReader) {
            super.init(r)
            skinTransform = SkinTransform(r)
            numBones = r.readLEUInt32()
            skinPartition = NiReaderUtils.ReadRef<NiSkinPartition>(r)
            boneList = [SkinData](); boneList.reserveCapacity(numBones)
            for i in 0..<boneList.capacity {
                boneList[i] = SkinData(r)
            }
        }
    }

    public struct NiSkinPartition: NiObject { }

    // Miscellaneous
    public struct NiTexture: NiObjectNET {
        init(_ r: BinaryReader) {
            super.init(r)
        }
    }

    public struct NiSourceTexture: NiTexture {
        public let useExternal: UInt8
        public let fileName: String
        public let pixelLayout: PixelLayout
        public let useMipMaps: MipMapFormat
        public let alphaFormat: AlphaFormat
        public let isStatic: UInt8

        init(_ r: BinaryReader) {
            super.init(r)
            useExternal = r.readByte()
            fileName = r.readLELength32PrefixedASCIIString()
            pixelLayout = PixelLayout(rawValue: r.readLEUInt32())
            useMipMaps = MipMapFormat(rawValue: r.readLEUInt32())
            alphaFormat = AlphaFormat(rawValue: r.readLEUInt32())
            isStatic = r.readByte()
        }
    }

    public struct NiPoint3InterpController: NiSingleInterpController {
        public let data: Ref<NiPosData> 

        init(_ r: BinaryReader) {
            super.init(r)
            data = NiReaderUtils.readRef<NiPosData>(r)
        }
    }

    public struct NiMaterialProperty: NiProperty {
        public let flags: UInt16
        public let ambientColor: Color3
        public let diffuseColor: Color3
        public let specularColor: Color3
        public let emissiveColor: Color3
        public let glossiness: Float
        public let alpha: Float

        init(_ r: BinaryReader) {
            super.init(r)
            flags = NiReaderUtils.readFlags(r)
            ambientColor = Color3(r)
            diffuseColor = Color3(r)
            specularColor = Color3(r)
            emissiveColor = Color3(r)
            glossiness = r.readLESingle()
            alpha = r.readLESingle()
        }
    }

    public struct NiMaterialColorController: NiPoint3InterpController { }

    public struct NiDynamicEffect: NiAVObject {
        let numAffectedNodeListPointers: UInt32
        let affectedNodeListPointers: [UInt32]

        init(_ r: BinaryReader) {
            super.init(r)
            numAffectedNodeListPointers = r.readLEUInt32()
            affectedNodeListPointers = [Uint32](); affectedNodeListPointers.reserveCapacity(numAffectedNodeListPointers)
            for i in 0..<affectedNodeListPointers.capacity {
                affectedNodeListPointers[i] = r.readLEUInt32()
            }
        }
    }

    public struct NiTextureEffect: NiDynamicEffect {
        public let modelProjectionMatrix: Matrix4x4
        public let modelProjectionTransform: Vector3
        public let textureFiltering: TexFilterMode
        public let textureClamping: TexClampMode
        public let textureType: EffectType
        public let coordinateGenerationType: CoordGenType
        public let sourceTexture: Ref<NiSourceTexture>
        public let clippingPlane: UInt8
        public let unknownVector: Vector3
        public let unknownFloat: Float
        public let ps2L: Int16
        public let ps2K: Int16
        public let unknownShort: UInt16

        init(_ r: BinaryReader) {
            super.init(r)
            modelProjectionMatrix = NiReaderUtils.read3x3RotationMatrix(r)
            modelProjectionTransform = r.readLEVector3()
            textureFiltering = TexFilterMode(rawValue: r.readLEUInt32())
            textureClamping = TexClampMode(rawValue: r.readLEUInt32())
            textureType = EffectType(rawValue: r.readLEUInt32())
            coordinateGenerationType = CoordGenType(rawValue: r.readLEUInt32())
            sourceTexture = NiReaderUtils.readRef<NiSourceTexture>(r)
            clippingPlane = r.readByte()
            unknownVector = r.readLEVector3()
            unknownFloat = r.readLESingle()
            ps2L = r.readLEInt16()
            ps2K = r.readLEInt16()
            unknownShort = r.readLEUInt16()
        }
    }
}