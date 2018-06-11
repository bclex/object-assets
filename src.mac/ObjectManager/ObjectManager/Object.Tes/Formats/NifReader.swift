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
                vertexIndices[i] = r.ReadLEUInt16();
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
            numVertices = r.ReadLEUInt16()
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

    public class NiAutoNormalParticles: NiParticles { }

    public class NiAutoNormalParticlesData: NiParticlesData { }

    public class NiParticleSystemController: NiTimeController
    {
        public float Speed;
        public float SpeedRandom;
        public float VerticalDirection;
        public float VerticalAngle;
        public float HorizontalDirection;
        public float HorizontalAngle;
        public Vector3 UnknownNormal;
        public Color4 UnknownColor;
        public float Size;
        public float EmitStartTime;
        public float EmitStopTime;
        public byte UnknownByte;
        public float EmitRate;
        public float Lifetime;
        public float LifetimeRandom;
        public ushort EmitFlags;
        public Vector3 StartRandom;
        public Ptr<NiObject> Emitter;
        public ushort UnknownShort2;
        public float UnknownFloat13;
        public uint UnknownInt1;
        public uint UnknownInt2;
        public ushort UnknownShort3;
        public ushort NumParticles;
        public ushort NumValid;
        public Particle[] Particles;
        public Ref<NiObject> UnknownLink;
        public Ref<NiParticleModifier> ParticleExtra;
        public Ref<NiObject> UnknownLink2;
        public byte Trailer;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Speed = r.ReadLESingle();
            SpeedRandom = r.ReadLESingle();
            VerticalDirection = r.ReadLESingle();
            VerticalAngle = r.ReadLESingle();
            HorizontalDirection = r.ReadLESingle();
            HorizontalAngle = r.ReadLESingle();
            UnknownNormal = r.ReadLEVector3();
            UnknownColor = new Color4();
            UnknownColor.Deserialize(r);
            Size = r.ReadLESingle();
            EmitStartTime = r.ReadLESingle();
            EmitStopTime = r.ReadLESingle();
            UnknownByte = r.ReadByte();
            EmitRate = r.ReadLESingle();
            Lifetime = r.ReadLESingle();
            LifetimeRandom = r.ReadLESingle();
            EmitFlags = r.ReadLEUInt16();
            StartRandom = r.ReadLEVector3();
            Emitter = NiReaderUtils.ReadPtr<NiObject>(r);
            UnknownShort2 = r.ReadLEUInt16();
            UnknownFloat13 = r.ReadLESingle();
            UnknownInt1 = r.ReadLEUInt32();
            UnknownInt2 = r.ReadLEUInt32();
            UnknownShort3 = r.ReadLEUInt16();
            NumParticles = r.ReadLEUInt16();
            NumValid = r.ReadLEUInt16();
            Particles = new Particle[NumParticles];
            for (var i = 0; i < Particles.Length; i++)
            {
                Particles[i] = new Particle();
                Particles[i].Deserialize(r);
            }
            UnknownLink = NiReaderUtils.ReadRef<NiObject>(r);
            ParticleExtra = NiReaderUtils.ReadRef<NiParticleModifier>(r);
            UnknownLink2 = NiReaderUtils.ReadRef<NiObject>(r);
            Trailer = r.ReadByte();
        }
    }

    public class NiBSPArrayController : NiParticleSystemController { }

    // Particle Modifiers
    public abstract class NiParticleModifier : NiObject
    {
        public Ref<NiParticleModifier> NextModifier;
        public Ptr<NiParticleSystemController> Controller;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NextModifier = NiReaderUtils.ReadRef<NiParticleModifier>(r);
            Controller = NiReaderUtils.ReadPtr<NiParticleSystemController>(r);
        }
    }

    public class NiGravity : NiParticleModifier
    {
        public float UnknownFloat1;
        public float Force;
        public FieldType Type;
        public Vector3 Position;
        public Vector3 Direction;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            UnknownFloat1 = r.ReadLESingle();
            Force = r.ReadLESingle();
            Type = (FieldType)r.ReadLEUInt32();
            Position = r.ReadLEVector3();
            Direction = r.ReadLEVector3();
        }
    }

    public class NiParticleBomb : NiParticleModifier
    {
        public float Decay;
        public float Duration;
        public float DeltaV;
        public float Start;
        public DecayType DecayType;
        public Vector3 Position;
        public Vector3 Direction;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Decay = r.ReadLESingle();
            Duration = r.ReadLESingle();
            DeltaV = r.ReadLESingle();
            Start = r.ReadLESingle();
            DecayType = (DecayType)r.ReadLEUInt32();
            Position = r.ReadLEVector3();
            Direction = r.ReadLEVector3();
        }
    }

    public class NiParticleColorModifier : NiParticleModifier
    {
        public Ref<NiColorData> ColorData;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            ColorData = NiReaderUtils.ReadRef<NiColorData>(r);
        }
    }

    public class NiParticleGrowFade : NiParticleModifier
    {
        public float Grow;
        public float Fade;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Grow = r.ReadLESingle();
            Fade = r.ReadLESingle();
        }
    }

    public class NiParticleMeshModifier : NiParticleModifier
    {
        public uint NumParticleMeshes;
        public Ref<NiAVObject>[] ParticleMeshes;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NumParticleMeshes = r.ReadLEUInt32();
            ParticleMeshes = new Ref<NiAVObject>[NumParticleMeshes];
            for (var i = 0; i < ParticleMeshes.Length; i++)
                ParticleMeshes[i] = NiReaderUtils.ReadRef<NiAVObject>(r);
        }
    }

    public class NiParticleRotation : NiParticleModifier
    {
        public byte RandomInitialAxis;
        public Vector3 InitialAxis;
        public float RotationSpeed;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            RandomInitialAxis = r.ReadByte();
            InitialAxis = r.ReadLEVector3();
            RotationSpeed = r.ReadLESingle();
        }
    }

    // Controllers
    public abstract class NiTimeController : NiObject
    {
        public Ref<NiTimeController> NextController;
        public ushort Flags;
        public float Frequency;
        public float Phase;
        public float StartTime;
        public float StopTime;
        public Ptr<NiObjectNET> Target;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NextController = NiReaderUtils.ReadRef<NiTimeController>(r);
            Flags = r.ReadLEUInt16();
            Frequency = r.ReadLESingle();
            Phase = r.ReadLESingle();
            StartTime = r.ReadLESingle();
            StopTime = r.ReadLESingle();
            Target = NiReaderUtils.ReadPtr<NiObjectNET>(r);
        }
    }

    public class NiUVController : NiTimeController
    {
        public ushort UnknownShort;
        public Ref<NiUVData> Data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            UnknownShort = r.ReadLEUInt16();
            Data = NiReaderUtils.ReadRef<NiUVData>(r);
        }
    }

    public abstract class NiInterpController : NiTimeController { }

    public abstract class NiSingleInterpController : NiInterpController { }

    public class NiKeyframeController : NiSingleInterpController
    {
        public Ref<NiKeyframeData> Data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Data = NiReaderUtils.ReadRef<NiKeyframeData>(r);
        }
    }

    public class NiGeomMorpherController : NiInterpController
    {
        public Ref<NiMorphData> Data;
        public byte AlwaysUpdate;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Data = NiReaderUtils.ReadRef<NiMorphData>(r);
            AlwaysUpdate = r.ReadByte();
        }
    }

    public abstract class NiBoolInterpController : NiSingleInterpController { }

    public class NiVisController : NiBoolInterpController
    {
        public Ref<NiVisData> Data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Data = NiReaderUtils.ReadRef<NiVisData>(r);
        }
    }

    public abstract class NiFloatInterpController : NiSingleInterpController { }

    public class NiAlphaController : NiFloatInterpController
    {
        public Ref<NiFloatData> Data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Data = NiReaderUtils.ReadRef<NiFloatData>(r);
        }
    }

    // Skin Stuff
    public class NiSkinInstance : NiObject
    {
        public Ref<NiSkinData> Data;
        public Ptr<NiNode> SkeletonRoot;
        public uint NumBones;
        public Ptr<NiNode>[] Bones;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Data = NiReaderUtils.ReadRef<NiSkinData>(r);
            SkeletonRoot = NiReaderUtils.ReadPtr<NiNode>(r);
            NumBones = r.ReadLEUInt32();
            Bones = new Ptr<NiNode>[NumBones];
            for (var i = 0; i < Bones.Length; i++)
                Bones[i] = NiReaderUtils.ReadPtr<NiNode>(r);
        }
    }

    public class NiSkinData : NiObject
    {
        public SkinTransform SkinTransform;
        public uint NumBones;
        public Ref<NiSkinPartition> SkinPartition;
        public SkinData[] BoneList;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            SkinTransform = new SkinTransform();
            SkinTransform.Deserialize(r);
            NumBones = r.ReadLEUInt32();
            SkinPartition = NiReaderUtils.ReadRef<NiSkinPartition>(r);
            BoneList = new SkinData[NumBones];
            for (var i = 0; i < BoneList.Length; i++)
            {
                BoneList[i] = new SkinData();
                BoneList[i].Deserialize(r);
            }
        }
    }

    public class NiSkinPartition : NiObject { }

    // Miscellaneous
    public abstract class NiTexture : NiObjectNET
    {
        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
        }
    }

    public class NiSourceTexture : NiTexture
    {
        public byte UseExternal;
        public string FileName;
        public PixelLayout PixelLayout;
        public MipMapFormat UseMipMaps;
        public AlphaFormat AlphaFormat;
        public byte IsStatic;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            UseExternal = r.ReadByte();
            FileName = r.ReadLELength32PrefixedASCIIString();
            PixelLayout = (PixelLayout)r.ReadLEUInt32();
            UseMipMaps = (MipMapFormat)r.ReadLEUInt32();
            AlphaFormat = (AlphaFormat)r.ReadLEUInt32();
            IsStatic = r.ReadByte();
        }
    }

    public abstract class NiPoint3InterpController : NiSingleInterpController
    {
        public Ref<NiPosData> Data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Data = NiReaderUtils.ReadRef<NiPosData>(r);
        }
    }

    public class NiMaterialProperty : NiProperty
    {
        public ushort Flags;
        public Color3 AmbientColor;
        public Color3 DiffuseColor;
        public Color3 SpecularColor;
        public Color3 EmissiveColor;
        public float Glossiness;
        public float Alpha;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Flags = NiReaderUtils.ReadFlags(r);
            AmbientColor = new Color3();
            AmbientColor.Deserialize(r);
            DiffuseColor = new Color3();
            DiffuseColor.Deserialize(r);
            SpecularColor = new Color3();
            SpecularColor.Deserialize(r);
            EmissiveColor = new Color3();
            EmissiveColor.Deserialize(r);
            Glossiness = r.ReadLESingle();
            Alpha = r.ReadLESingle();
        }
    }

    public class NiMaterialColorController : NiPoint3InterpController { }

    public abstract class NiDynamicEffect : NiAVObject
    {
        uint NumAffectedNodeListPointers;
        uint[] AffectedNodeListPointers;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NumAffectedNodeListPointers = r.ReadLEUInt32();
            AffectedNodeListPointers = new uint[NumAffectedNodeListPointers];
            for (var i = 0; i < AffectedNodeListPointers.Length; i++)
                AffectedNodeListPointers[i] = r.ReadLEUInt32();
        }
    }

    public class NiTextureEffect : NiDynamicEffect
    {
        public Matrix4x4 ModelProjectionMatrix;
        public Vector3 ModelProjectionTransform;
        public TexFilterMode TextureFiltering;
        public TexClampMode TextureClamping;
        public EffectType TextureType;
        public CoordGenType CoordinateGenerationType;
        public Ref<NiSourceTexture> SourceTexture;
        public byte ClippingPlane;
        public Vector3 UnknownVector;
        public float UnknownFloat;
        public short PS2L;
        public short PS2K;
        public ushort UnknownShort;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            ModelProjectionMatrix = NiReaderUtils.Read3x3RotationMatrix(r);
            ModelProjectionTransform = r.ReadLEVector3();
            TextureFiltering = (TexFilterMode)r.ReadLEUInt32();
            TextureClamping = (TexClampMode)r.ReadLEUInt32();
            TextureType = (EffectType)r.ReadLEUInt32();
            CoordinateGenerationType = (CoordGenType)r.ReadLEUInt32();
            SourceTexture = NiReaderUtils.ReadRef<NiSourceTexture>(r);
            ClippingPlane = r.ReadByte();
            UnknownVector = r.ReadLEVector3();
            UnknownFloat = r.ReadLESingle();
            PS2L = r.ReadLEInt16();
            PS2K = r.ReadLEInt16();
            UnknownShort = r.ReadLEUInt16();
        }
    }
}