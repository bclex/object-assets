using OA.Core;
using System;
using UnityEngine;

namespace OA.Tes.Formats
{
    // Refers to an object before the current one in the hierarchy.
    public struct Ptr<T>
    {
        public int value;
        public bool isNull
        {
            get { return value < 0; }
        }

        public void Deserialize(UnityBinaryReader reader)
        {
            value = reader.ReadLEInt32();
        }
    }

    // Refers to an object after the current one in the hierarchy.
    public struct Ref<T>
    {
        public int value;
        public bool isNull
        {
            get { return value < 0; }
        }

        public void Deserialize(UnityBinaryReader r)
        {
            value = r.ReadLEInt32();
        }
    }

    public class NiReaderUtils
    {
        public static Ptr<T> ReadPtr<T>(UnityBinaryReader r)
        {
            var ptr = new Ptr<T>();
            ptr.Deserialize(r);
            return ptr;
        }

        public static Ref<T> ReadRef<T>(UnityBinaryReader r)
        {
            var readRef = new Ref<T>();
            readRef.Deserialize(r);
            return readRef;
        }

        public static Ref<T>[] ReadLengthPrefixedRefs32<T>(UnityBinaryReader r)
        {
            var refs = new Ref<T>[r.ReadLEUInt32()];
            for (var i = 0; i < refs.Length; i++)
                refs[i] = ReadRef<T>(r);
            return refs;
        }

        public static ushort ReadFlags(UnityBinaryReader r)
        {
            return r.ReadLEUInt16();
        }

        public static T Read<T>(UnityBinaryReader r)
        {
            if (typeof(T) == typeof(float)) { return (T)((object)r.ReadLESingle()); }
            else if (typeof(T) == typeof(byte)) { return (T)((object)r.ReadByte()); }
            else if (typeof(T) == typeof(string)) { return (T)((object)r.ReadLELength32PrefixedASCIIString()); }
            else if (typeof(T) == typeof(Vector3)) { return (T)((object)r.ReadLEVector3()); }
            else if (typeof(T) == typeof(Quaternion)) { return (T)((object)r.ReadLEQuaternionWFirst()); }
            else if (typeof(T) == typeof(Color4)) { var color = new Color4(); color.Deserialize(r); return (T)((object)color); }
            else
                throw new NotImplementedException("Tried to read an unsupported type.");
        }

        public static NiObject ReadNiObject(UnityBinaryReader r)
        {
            var nodeTypeBytes = r.ReadLELength32PrefixedBytes();
            if (StringUtils.Equals(nodeTypeBytes, "NiNode")) { var node = new NiNode(); node.Deserialize(r); return node; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiTriShape")) { var triShape = new NiTriShape(); triShape.Deserialize(r); return triShape; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiTexturingProperty")) { var prop = new NiTexturingProperty(); prop.Deserialize(r); return prop; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiSourceTexture")) { var srcTexture = new NiSourceTexture(); srcTexture.Deserialize(r); return srcTexture; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiMaterialProperty")) { var prop = new NiMaterialProperty(); prop.Deserialize(r); return prop; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiMaterialColorController")) { var controller = new NiMaterialColorController(); controller.Deserialize(r); return controller; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiTriShapeData")) { var data = new NiTriShapeData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "RootCollisionNode")) { var node = new RootCollisionNode(); node.Deserialize(r); return node; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiStringExtraData")) { var data = new NiStringExtraData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiSkinInstance")) { var instance = new NiSkinInstance(); instance.Deserialize(r); return instance; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiSkinData")) { var data = new NiSkinData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiAlphaProperty")) { var prop = new NiAlphaProperty(); prop.Deserialize(r); return prop; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiZBufferProperty")) { var prop = new NiZBufferProperty(); prop.Deserialize(r); return prop; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiVertexColorProperty")) { var prop = new NiVertexColorProperty(); prop.Deserialize(r); return prop; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiBSAnimationNode")) { var node = new NiBSAnimationNode(); node.Deserialize(r); return node; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiBSParticleNode")) { var node = new NiBSParticleNode(); node.Deserialize(r); return node; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiParticles")) { var node = new NiParticles(); node.Deserialize(r); return node; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiParticlesData")) { var data = new NiParticlesData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiRotatingParticles")) { var node = new NiRotatingParticles(); node.Deserialize(r); return node; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiRotatingParticlesData")) { var data = new NiRotatingParticlesData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiAutoNormalParticles")) { var node = new NiAutoNormalParticles(); node.Deserialize(r); return node; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiAutoNormalParticlesData")) { var data = new NiAutoNormalParticlesData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiUVController")) { var controller = new NiUVController(); controller.Deserialize(r); return controller; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiUVData")) { var data = new NiUVData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiTextureEffect")) { var effect = new NiTextureEffect(); effect.Deserialize(r); return effect; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiTextKeyExtraData")) { var data = new NiTextKeyExtraData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiVertWeightsExtraData")) { var data = new NiVertWeightsExtraData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiParticleSystemController")) { var controller = new NiParticleSystemController(); controller.Deserialize(r); return controller; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiBSPArrayController")) { var controller = new NiBSPArrayController(); controller.Deserialize(r); return controller; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiGravity")) { var obj = new NiGravity(); obj.Deserialize(r); return obj; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiParticleBomb")) { var modifier = new NiParticleBomb(); modifier.Deserialize(r); return modifier; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiParticleColorModifier")) { var modifier = new NiParticleColorModifier(); modifier.Deserialize(r); return modifier; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiParticleGrowFade")) { var modifier = new NiParticleGrowFade(); modifier.Deserialize(r); return modifier; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiParticleMeshModifier")) { var modifier = new NiParticleMeshModifier(); modifier.Deserialize(r); return modifier; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiParticleRotation")) { var modifier = new NiParticleRotation(); modifier.Deserialize(r); return modifier; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiKeyframeController")) { var controller = new NiKeyframeController(); controller.Deserialize(r); return controller; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiKeyframeData")) { var data = new NiKeyframeData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiColorData")) { var data = new NiColorData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiGeomMorpherController")) { var controller = new NiGeomMorpherController(); controller.Deserialize(r); return controller; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiMorphData")) { var data = new NiMorphData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "AvoidNode")) { var node = new AvoidNode(); node.Deserialize(r); return node; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiVisController")) { var controller = new NiVisController(); controller.Deserialize(r); return controller; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiVisData")) { var data = new NiVisData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiAlphaController")) { var controller = new NiAlphaController(); controller.Deserialize(r); return controller; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiFloatData")) { var data = new NiFloatData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiPosData")) { var data = new NiPosData(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiBillboardNode")) { var data = new NiBillboardNode(); data.Deserialize(r); return data; }
            else if (StringUtils.Equals(nodeTypeBytes, "NiShadeProperty")) { var property = new NiShadeProperty(); property.Deserialize(r); return property; }
            else { Utils.Log("Tried to read an unsupported NiObject type (" + System.Text.Encoding.ASCII.GetString(nodeTypeBytes) + ")."); return null; }
        }

        public static Matrix4x4 Read3x3RotationMatrix(UnityBinaryReader r)
        {
            return r.ReadLERowMajorMatrix3x3();
        }
    }

    public class NiFile
    {
        public NiFile(string name)
        {
            this.name = name;
        }

        public string name;
        public NiHeader header;
        public NiObject[] blocks;
        public NiFooter footer;

        public void Deserialize(UnityBinaryReader r)
        {
            header = new NiHeader();
            header.Deserialize(r);
            blocks = new NiObject[header.numBlocks];
            for (var i = 0; i < header.numBlocks; i++)
                blocks[i] = NiReaderUtils.ReadNiObject(r);
            footer = new NiFooter();
            footer.Deserialize(r);
        }
    }

    #region Enums

    // texture enums
    public enum ApplyMode : uint
    {
        APPLY_REPLACE = 0,
        APPLY_DECAL = 1,
        APPLY_MODULATE = 2,
        APPLY_HILIGHT = 3,
        APPLY_HILIGHT2 = 4
    }

    public enum TexClampMode : uint
    {
        CLAMP_S_CLAMP_T = 0,
        CLAMP_S_WRAP_T = 1,
        WRAP_S_CLAMP_T = 2,
        WRAP_S_WRAP_T = 3
    }

    public enum TexFilterMode : uint
    {
        FILTER_NEAREST = 0,
        FILTER_BILERP = 1,
        FILTER_TRILERP = 2,
        FILTER_NEAREST_MIPNEAREST = 3,
        FILTER_NEAREST_MIPLERP = 4,
        FILTER_BILERP_MIPNEAREST = 5
    }

    public enum PixelLayout : uint
    {
        PIX_LAY_PALETTISED = 0,
        PIX_LAY_HIGH_COLOR_16 = 1,
        PIX_LAY_TRUE_COLOR_32 = 2,
        PIX_LAY_COMPRESSED = 3,
        PIX_LAY_BUMPMAP = 4,
        PIX_LAY_PALETTISED_4 = 5,
        PIX_LAY_DEFAULT = 6
    }

    public enum MipMapFormat : uint
    {
        MIP_FMT_NO = 0,
        MIP_FMT_YES = 1,
        MIP_FMT_DEFAULT = 2
    }

    public enum AlphaFormat : uint
    {
        ALPHA_NONE = 0,
        ALPHA_BINARY = 1,
        ALPHA_SMOOTH = 2,
        ALPHA_DEFAULT = 3
    }

    // miscellaneous
    public enum VertMode : uint
    {
        VERT_MODE_SRC_IGNORE = 0,
        VERT_MODE_SRC_EMISSIVE = 1,
        VERT_MODE_SRC_AMB_DIF = 2
    }

    public enum LightMode : uint
    {
        LIGHT_MODE_EMISSIVE = 0,
        LIGHT_MODE_EMI_AMB_DIF = 1
    }

    public enum KeyType : uint
    {
        LINEAR_KEY = 1,
        QUADRATIC_KEY = 2,
        TBC_KEY = 3,
        XYZ_ROTATION_KEY = 4,
        CONST_KEY = 5
    }

    public enum EffectType : uint
    {
        EFFECT_PROJECTED_LIGHT = 0,
        EFFECT_PROJECTED_SHADOW = 1,
        EFFECT_ENVIRONMENT_MAP = 2,
        EFFECT_FOG_MAP = 3
    }

    public enum CoordGenType : uint
    {
        CG_WORLD_PARALLEL = 0,
        CG_WORLD_PERSPECTIVE = 1,
        CG_SPHERE_MAP = 2,
        CG_SPECULAR_CUBE_MAP = 3,
        CG_DIFFUSE_CUBE_MAP = 4
    }

    public enum FieldType : uint
    {
        FIELD_WIND = 0,
        FIELD_POINT = 1
    }

    public enum DecayType : uint
    {
        DECAY_NONE = 0,
        DECAY_LINEAR = 1,
        DECAY_EXPONENTIAL = 2
    }

    #endregion // Enums

    #region Misc Classes

    public class BoundingBox
    {
        public uint unknownInt;
        public Vector3 translation;
        public Matrix4x4 rotation;
        public Vector3 radius;

        public void Deserialize(UnityBinaryReader r)
        {
            unknownInt = r.ReadLEUInt32();
            translation = r.ReadLEVector3();
            rotation = NiReaderUtils.Read3x3RotationMatrix(r);
            radius = r.ReadLEVector3();
        }
    }

    public class Color3
    {
        public float r;
        public float g;
        public float b;

        public void Deserialize(UnityBinaryReader r)
        {
            this.r = r.ReadLESingle();
            g = r.ReadLESingle();
            b = r.ReadLESingle();
        }
    }

    public class Color4
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public void Deserialize(UnityBinaryReader r)
        {
            this.r = r.ReadLESingle();
            g = r.ReadLESingle();
            b = r.ReadLESingle();
            a = r.ReadLESingle();
        }
    }

    public class TexDesc
    {
        public Ref<NiSourceTexture> source;
        public TexClampMode clampMode;
        public TexFilterMode filterMode;
        public uint UVSet;
        public short PS2L;
        public short PS2K;
        public ushort unknown1;

        public void Deserialize(UnityBinaryReader r)
        {
            source = NiReaderUtils.ReadRef<NiSourceTexture>(r);
            clampMode = (TexClampMode)r.ReadLEUInt32();
            filterMode = (TexFilterMode)r.ReadLEUInt32();
            UVSet = r.ReadLEUInt32();
            PS2L = r.ReadLEInt16();
            PS2K = r.ReadLEInt16();
            unknown1 = r.ReadLEUInt16();
        }
    }

    public class TexCoord
    {
        public float u;
        public float v;

        public void Deserialize(UnityBinaryReader r)
        {
            u = r.ReadLESingle();
            v = r.ReadLESingle();
        }
    }

    public class Triangle
    {
        public ushort v1;
        public ushort v2;
        public ushort v3;

        public void Deserialize(UnityBinaryReader r)
        {
            v1 = r.ReadLEUInt16();
            v2 = r.ReadLEUInt16();
            v3 = r.ReadLEUInt16();
        }
    }

    public class MatchGroup
    {
        public ushort numVertices;
        public ushort[] vertexIndices;

        public void Deserialize(UnityBinaryReader r)
        {
            numVertices = r.ReadLEUInt16();
            vertexIndices = new ushort[numVertices];
            for (var i = 0; i < vertexIndices.Length; i++)
                vertexIndices[i] = r.ReadLEUInt16();
        }
    }

    public class TBC
    {
        public float t;
        public float b;
        public float c;

        public void Deserialize(UnityBinaryReader r)
        {
            t = r.ReadLESingle();
            b = r.ReadLESingle();
            c = r.ReadLESingle();
        }
    }

    public class Key<T>
    {
        public float time;
        public T value;
        public T forward;
        public T backward;
        public TBC TBC;

        public void Deserialize(UnityBinaryReader r, KeyType keyType)
        {
            time = r.ReadLESingle();
            value = NiReaderUtils.Read<T>(r);
            if (keyType == KeyType.QUADRATIC_KEY)
            {
                forward = NiReaderUtils.Read<T>(r);
                backward = NiReaderUtils.Read<T>(r);
            }
            else if (keyType == KeyType.TBC_KEY)
            {
                TBC = new TBC();
                TBC.Deserialize(r);
            }
        }
    }
    public class KeyGroup<T>
    {
        public uint numKeys;
        public KeyType interpolation;
        public Key<T>[] keys;

        public void Deserialize(UnityBinaryReader r)
        {
            numKeys = r.ReadLEUInt32();
            if (numKeys != 0)
                interpolation = (KeyType)r.ReadLEUInt32();
            keys = new Key<T>[numKeys];
            for (var i = 0; i < keys.Length; i++)
            {
                keys[i] = new Key<T>();
                keys[i].Deserialize(r, interpolation);
            }
        }
    }

    public class QuatKey<T>
    {
        public float time;
        public T value;
        public TBC TBC;

        public void Deserialize(UnityBinaryReader r, KeyType keyType)
        {
            time = r.ReadLESingle();
            if (keyType != KeyType.XYZ_ROTATION_KEY)
                value = NiReaderUtils.Read<T>(r);
            if (keyType == KeyType.TBC_KEY)
            {
                TBC = new TBC();
                TBC.Deserialize(r);
            }
        }
    }

    public class SkinData
    {
        public SkinTransform skinTransform;
        public Vector3 boundingSphereOffset;
        public float boundingSphereRadius;
        public ushort numVertices;
        public SkinWeight[] vertexWeights;

        public void Deserialize(UnityBinaryReader r)
        {
            skinTransform = new SkinTransform();
            skinTransform.Deserialize(r);
            boundingSphereOffset = r.ReadLEVector3();
            boundingSphereRadius = r.ReadLESingle();
            numVertices = r.ReadLEUInt16();
            vertexWeights = new SkinWeight[numVertices];
            for (var i = 0; i < vertexWeights.Length; i++)
            {
                vertexWeights[i] = new SkinWeight();
                vertexWeights[i].Deserialize(r);
            }
        }
    }

    public class SkinWeight
    {
        public ushort index;
        public float weight;

        public void Deserialize(UnityBinaryReader r)
        {
            index = r.ReadLEUInt16();
            weight = r.ReadLESingle();
        }
    }

    public class SkinTransform
    {
        public Matrix4x4 rotation;
        public Vector3 translation;
        public float scale;

        public void Deserialize(UnityBinaryReader r)
        {
            rotation = NiReaderUtils.Read3x3RotationMatrix(r);
            translation = r.ReadLEVector3();
            scale = r.ReadLESingle();
        }
    }

    public class Particle
    {
        public Vector3 velocity;
        public Vector3 unknownVector;
        public float lifetime;
        public float lifespan;
        public float timestamp;
        public ushort unknownShort;
        public ushort vertexID;

        public void Deserialize(UnityBinaryReader r)
        {
            velocity = r.ReadLEVector3();
            unknownVector = r.ReadLEVector3();
            lifetime = r.ReadLESingle();
            lifespan = r.ReadLESingle();
            timestamp = r.ReadLESingle();
            unknownShort = r.ReadLEUInt16();
            vertexID = r.ReadLEUInt16();
        }
    }

    public class Morph
    {
        public uint numKeys;
        public KeyType interpolation;
        public Key<float>[] keys;
        public Vector3[] vectors;

        public void Deserialize(UnityBinaryReader r, uint numVertices)
        {
            numKeys = r.ReadLEUInt32();
            interpolation = (KeyType)r.ReadLEUInt32();
            keys = new Key<float>[numKeys];
            for (var i = 0; i < keys.Length; i++)
            {
                keys[i] = new Key<float>();
                keys[i].Deserialize(r, interpolation);
            }
            vectors = new Vector3[numVertices];
            for (var i = 0; i < vectors.Length; i++)
                vectors[i] = r.ReadLEVector3();
        }
    }

    #endregion

    public class NiHeader
    {
        public byte[] str; // 40 bytes (including \n)
        public uint version;
        public uint numBlocks;

        public void Deserialize(UnityBinaryReader r)
        {
            str = r.ReadBytes(40);
            version = r.ReadLEUInt32();
            numBlocks = r.ReadLEUInt32();
        }
    }

    public class NiFooter
    {
        public uint numRoots;
        public int[] roots;

        public void Deserialize(UnityBinaryReader r)
        {
            numRoots = r.ReadLEUInt32();
            roots = new int[numRoots];
            for (var i = 0; i < numRoots; i++)
                roots[i] = r.ReadLEInt32();
        }
    }

    /// <summary>
    /// These are the main units of data that NIF files are arranged in.
    /// </summary>
    public abstract class NiObject
    {
        public virtual void Deserialize(UnityBinaryReader r) { }
    }

    /// <summary>
    /// An object that can be controlled by a controller.
    /// </summary>
    public abstract class NiObjectNET : NiObject
    {
        public string name;
        public Ref<NiExtraData> extraData;
        public Ref<NiTimeController> controller;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            name = r.ReadLELength32PrefixedASCIIString();
            extraData = NiReaderUtils.ReadRef<NiExtraData>(r);
            controller = NiReaderUtils.ReadRef<NiTimeController>(r);
        }
    }

    public abstract class NiAVObject : NiObjectNET
    {
        public enum Flags
        {
            Hidden = 0x1
        }

        public ushort flags;
        public Vector3 translation;
        public Matrix4x4 rotation;
        public float scale;
        public Vector3 velocity;
        //public uint numProperties;
        public Ref<NiProperty>[] properties;
        public bool hasBoundingBox;
        public BoundingBox boundingBox;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            flags = NiReaderUtils.ReadFlags(r);
            translation = r.ReadLEVector3();
            rotation = NiReaderUtils.Read3x3RotationMatrix(r);
            scale = r.ReadLESingle();
            velocity = r.ReadLEVector3();
            properties = NiReaderUtils.ReadLengthPrefixedRefs32<NiProperty>(r);
            hasBoundingBox = r.ReadLEBool32();
            if (hasBoundingBox)
            {
                boundingBox = new BoundingBox();
                boundingBox.Deserialize(r);
            }
        }
    }

    // Nodes
    public class NiNode : NiAVObject
    {
        //public uint numChildren;
        public Ref<NiAVObject>[] children;
        //public uint numEffects;
        public Ref<NiDynamicEffect>[] effects;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);

            children = NiReaderUtils.ReadLengthPrefixedRefs32<NiAVObject>(r);
            effects = NiReaderUtils.ReadLengthPrefixedRefs32<NiDynamicEffect>(r);
        }
    }

    public class RootCollisionNode : NiNode { }

    public class NiBSAnimationNode : NiNode { }

    public class NiBSParticleNode : NiNode { }

    public class NiBillboardNode : NiNode { }

    public class AvoidNode : NiNode { }

    // Geometry
    public abstract class NiGeometry : NiAVObject
    {
        public Ref<NiGeometryData> data;
        public Ref<NiSkinInstance> skinInstance;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            data = NiReaderUtils.ReadRef<NiGeometryData>(r);
            skinInstance = NiReaderUtils.ReadRef<NiSkinInstance>(r);
        }
    }

    public abstract class NiGeometryData : NiObject
    {
        public ushort numVertices;
        public bool hasVertices;
        public Vector3[] vertices;
        public bool hasNormals;
        public Vector3[] normals;
        public Vector3 center;
        public float radius;
        public bool hasVertexColors;
        public Color4[] vertexColors;
        public ushort numUVSets;
        public bool hasUV;
        public TexCoord[,] UVSets;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            numVertices = r.ReadLEUInt16();
            hasVertices = r.ReadLEBool32();
            if (hasVertices)
            {
                vertices = new Vector3[numVertices];
                for (var i = 0; i < vertices.Length; i++)
                    vertices[i] = r.ReadLEVector3();
            }
            hasNormals = r.ReadLEBool32();
            if (hasNormals)
            {
                normals = new Vector3[numVertices];
                for (var i = 0; i < normals.Length; i++)
                    normals[i] = r.ReadLEVector3();
            }
            center = r.ReadLEVector3();
            radius = r.ReadLESingle();
            hasVertexColors = r.ReadLEBool32();
            if (hasVertexColors)
            {
                vertexColors = new Color4[numVertices];
                for (var i = 0; i < vertexColors.Length; i++)
                {
                    vertexColors[i] = new Color4();
                    vertexColors[i].Deserialize(r);
                }
            }
            numUVSets = r.ReadLEUInt16();
            hasUV = r.ReadLEBool32();
            if (hasUV)
            {
                UVSets = new TexCoord[numUVSets, numVertices];
                for (var i = 0; i < numUVSets; i++)
                    for (var j = 0; j < numVertices; j++)
                    {
                        UVSets[i, j] = new TexCoord();
                        UVSets[i, j].Deserialize(r);
                    }
            }
        }
    }

    public abstract class NiTriBasedGeom : NiGeometry
    {
        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
        }
    }

    public abstract class NiTriBasedGeomData : NiGeometryData
    {
        public ushort numTriangles;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            numTriangles = r.ReadLEUInt16();
        }
    }

    public class NiTriShape : NiTriBasedGeom
    {
        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
        }
    }

    public class NiTriShapeData : NiTriBasedGeomData
    {
        public uint numTrianglePoints;
        public Triangle[] triangles;
        public ushort numMatchGroups;
        public MatchGroup[] matchGroups;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            numTrianglePoints = r.ReadLEUInt32();
            triangles = new Triangle[numTriangles];
            for (var i = 0; i < triangles.Length; i++)
            {
                triangles[i] = new Triangle();
                triangles[i].Deserialize(r);
            }
            numMatchGroups = r.ReadLEUInt16();
            matchGroups = new MatchGroup[numMatchGroups];
            for (var i = 0; i < matchGroups.Length; i++)
            {
                matchGroups[i] = new MatchGroup();
                matchGroups[i].Deserialize(r);
            }
        }
    }

    // Properties
    public abstract class NiProperty : NiObjectNET
    {
        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
        }
    }

    public class NiTexturingProperty : NiProperty
    {
        public ushort flags;
        public ApplyMode applyMode;
        public uint textureCount;
        public bool hasBaseTexture;
        public TexDesc baseTexture;
        public bool hasDarkTexture;
        public TexDesc darkTexture;
        public bool hasDetailTexture;
        public TexDesc detailTexture;
        public bool hasGlossTexture;
        public TexDesc glossTexture;
        public bool hasGlowTexture;
        public TexDesc glowTexture;
        public bool hasBumpMapTexture;
        public TexDesc bumpMapTexture;
        public bool hasDecal0Texture;
        public TexDesc decal0Texture;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            flags = NiReaderUtils.ReadFlags(r);
            applyMode = (ApplyMode)r.ReadLEUInt32();
            textureCount = r.ReadLEUInt32();
            hasBaseTexture = r.ReadLEBool32();
            if (hasBaseTexture)
            {
                baseTexture = new TexDesc();
                baseTexture.Deserialize(r);
            }
            hasDarkTexture = r.ReadLEBool32();
            if (hasDarkTexture)
            {
                darkTexture = new TexDesc();
                darkTexture.Deserialize(r);
            }
            hasDetailTexture = r.ReadLEBool32();
            if (hasDetailTexture)
            {
                detailTexture = new TexDesc();
                detailTexture.Deserialize(r);
            }
            hasGlossTexture = r.ReadLEBool32();
            if (hasGlossTexture)
            {
                glossTexture = new TexDesc();
                glossTexture.Deserialize(r);
            }
            hasGlowTexture = r.ReadLEBool32();
            if (hasGlowTexture)
            {
                glowTexture = new TexDesc();
                glowTexture.Deserialize(r);
            }
            hasBumpMapTexture = r.ReadLEBool32();
            if (hasBumpMapTexture)
            {
                bumpMapTexture = new TexDesc();
                bumpMapTexture.Deserialize(r);
            }
            hasDecal0Texture = r.ReadLEBool32();
            if (hasDecal0Texture)
            {
                decal0Texture = new TexDesc();
                decal0Texture.Deserialize(r);
            }
        }
    }
    public class NiAlphaProperty : NiProperty
    {
        public ushort flags;
        public byte threshold;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            flags = r.ReadLEUInt16();
            threshold = r.ReadByte();
        }
    }
    public class NiZBufferProperty : NiProperty
    {
        public ushort flags;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            flags = r.ReadLEUInt16();
        }
    }

    public class NiVertexColorProperty : NiProperty
    {
        public ushort flags;
        public VertMode vertexMode;
        public LightMode lightingMode;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            flags = NiReaderUtils.ReadFlags(r);
            vertexMode = (VertMode)r.ReadLEUInt32();
            lightingMode = (LightMode)r.ReadLEUInt32();
        }
    }

    public class NiShadeProperty : NiProperty
    {
        public ushort flags;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            flags = NiReaderUtils.ReadFlags(r);
        }
    }

    // Data
    public class NiUVData : NiObject
    {
        public KeyGroup<float>[] UVGroups;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            UVGroups = new KeyGroup<float>[4];
            for (var i = 0; i < UVGroups.Length; i++)
            {
                UVGroups[i] = new KeyGroup<float>();
                UVGroups[i].Deserialize(r);
            }
        }
    }

    public class NiKeyframeData : NiObject
    {
        public uint numRotationKeys;
        public KeyType rotationType;
        public QuatKey<Quaternion>[] quaternionKeys;
        public float unknownFloat;
        public KeyGroup<float>[] XYZRotations;
        public KeyGroup<Vector3> translations;
        public KeyGroup<float> scales;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            numRotationKeys = r.ReadLEUInt32();
            if (numRotationKeys != 0)
            {
                rotationType = (KeyType)r.ReadLEUInt32();
                if (rotationType != KeyType.XYZ_ROTATION_KEY)
                {
                    quaternionKeys = new QuatKey<Quaternion>[numRotationKeys];
                    for (var i = 0; i < quaternionKeys.Length; i++)
                    {
                        quaternionKeys[i] = new QuatKey<Quaternion>();
                        quaternionKeys[i].Deserialize(r, rotationType);
                    }
                }
                else
                {
                    unknownFloat = r.ReadLESingle();
                    XYZRotations = new KeyGroup<float>[3];
                    for (var i = 0; i < XYZRotations.Length; i++)
                    {
                        XYZRotations[i] = new KeyGroup<float>();
                        XYZRotations[i].Deserialize(r);
                    }
                }
            }
            translations = new KeyGroup<Vector3>();
            translations.Deserialize(r);
            scales = new KeyGroup<float>();
            scales.Deserialize(r);
        }
    }

    public class NiColorData : NiObject
    {
        public KeyGroup<Color4> data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            data = new KeyGroup<Color4>();
            data.Deserialize(r);
        }
    }

    public class NiMorphData : NiObject
    {
        public uint numMorphs;
        public uint numVertices;
        public byte relativeTargets;
        public Morph[] morphs;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            numMorphs = r.ReadLEUInt32();
            numVertices = r.ReadLEUInt32();
            relativeTargets = r.ReadByte();
            morphs = new Morph[numMorphs];
            for (var i = 0; i < morphs.Length; i++)
            {
                morphs[i] = new Morph();
                morphs[i].Deserialize(r, numVertices);
            }
        }
    }
    public class NiVisData : NiObject
    {
        public uint numKeys;
        public Key<byte>[] keys;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            numKeys = r.ReadLEUInt32();
            keys = new Key<byte>[numKeys];
            for (var i = 0; i < keys.Length; i++)
            {
                keys[i] = new Key<byte>();
                keys[i].Deserialize(r, KeyType.LINEAR_KEY);
            }
        }
    }

    public class NiFloatData : NiObject
    {
        public KeyGroup<float> data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            data = new KeyGroup<float>();
            data.Deserialize(r);
        }
    }

    public class NiPosData : NiObject
    {
        public KeyGroup<Vector3> data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            data = new KeyGroup<Vector3>();
            data.Deserialize(r);
        }
    }

    public class NiExtraData : NiObject
    {
        public Ref<NiExtraData> nextExtraData;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            nextExtraData = NiReaderUtils.ReadRef<NiExtraData>(r);
        }
    }

    public class NiStringExtraData : NiExtraData
    {
        public uint bytesRemaining;
        public string str;

        public override void Deserialize(UnityBinaryReader reader)
        {
            base.Deserialize(reader);
            bytesRemaining = reader.ReadLEUInt32();
            str = reader.ReadLELength32PrefixedASCIIString();
        }
    }

    public class NiTextKeyExtraData : NiExtraData
    {
        public uint unknownInt1;
        public uint numTextKeys;
        public Key<string>[] textKeys;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            unknownInt1 = r.ReadLEUInt32();
            numTextKeys = r.ReadLEUInt32();
            textKeys = new Key<string>[numTextKeys];
            for (var i = 0; i < textKeys.Length; i++)
            {
                textKeys[i] = new Key<string>();
                textKeys[i].Deserialize(r, KeyType.LINEAR_KEY);
            }
        }
    }

    public class NiVertWeightsExtraData : NiExtraData
    {
        public uint numBytes;
        public ushort numVertices;
        public float[] weights;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            numBytes = r.ReadLEUInt32();
            numVertices = r.ReadLEUInt16();
            weights = new float[numVertices];
            for (var i = 0; i < weights.Length; i++)
                weights[i] = r.ReadLESingle();
        }
    }

    // Particles
    public class NiParticles : NiGeometry { }

    public class NiParticlesData : NiGeometryData
    {
        public ushort numParticles;
        public float particleRadius;
        public ushort numActive;
        public bool hasSizes;
        public float[] sizes;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            numParticles = r.ReadLEUInt16();
            particleRadius = r.ReadLESingle();
            numActive = r.ReadLEUInt16();
            hasSizes = r.ReadLEBool32();
            if (hasSizes)
            {
                sizes = new float[numVertices];
                for (var i = 0; i < sizes.Length; i++)
                    sizes[i] = r.ReadLESingle();
            }
        }
    }

    public class NiRotatingParticles : NiParticles { }

    public class NiRotatingParticlesData : NiParticlesData
    {
        public bool hasRotations;
        public Quaternion[] rotations;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            hasRotations = r.ReadLEBool32();
            if (hasRotations)
            {
                rotations = new Quaternion[numVertices];
                for (var i = 0; i < rotations.Length; i++)
                    rotations[i] = r.ReadLEQuaternionWFirst();
            }
        }
    }

    public class NiAutoNormalParticles : NiParticles { }

    public class NiAutoNormalParticlesData : NiParticlesData { }

    public class NiParticleSystemController : NiTimeController
    {
        public float speed;
        public float speedRandom;
        public float verticalDirection;
        public float verticalAngle;
        public float horizontalDirection;
        public float horizontalAngle;
        public Vector3 unknownNormal;
        public Color4 unknownColor;
        public float size;
        public float emitStartTime;
        public float emitStopTime;
        public byte unknownByte;
        public float emitRate;
        public float lifetime;
        public float lifetimeRandom;
        public ushort emitFlags;
        public Vector3 startRandom;
        public Ptr<NiObject> emitter;
        public ushort unknownShort2;
        public float unknownFloat13;
        public uint unknownInt1;
        public uint unknownInt2;
        public ushort unknownShort3;
        public ushort numParticles;
        public ushort numValid;
        public Particle[] particles;
        public Ref<NiObject> unknownLink;
        public Ref<NiParticleModifier> particleExtra;
        public Ref<NiObject> unknownLink2;
        public byte trailer;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            speed = r.ReadLESingle();
            speedRandom = r.ReadLESingle();
            verticalDirection = r.ReadLESingle();
            verticalAngle = r.ReadLESingle();
            horizontalDirection = r.ReadLESingle();
            horizontalAngle = r.ReadLESingle();
            unknownNormal = r.ReadLEVector3();
            unknownColor = new Color4();
            unknownColor.Deserialize(r);
            size = r.ReadLESingle();
            emitStartTime = r.ReadLESingle();
            emitStopTime = r.ReadLESingle();
            unknownByte = r.ReadByte();
            emitRate = r.ReadLESingle();
            lifetime = r.ReadLESingle();
            lifetimeRandom = r.ReadLESingle();
            emitFlags = r.ReadLEUInt16();
            startRandom = r.ReadLEVector3();
            emitter = NiReaderUtils.ReadPtr<NiObject>(r);
            unknownShort2 = r.ReadLEUInt16();
            unknownFloat13 = r.ReadLESingle();
            unknownInt1 = r.ReadLEUInt32();
            unknownInt2 = r.ReadLEUInt32();
            unknownShort3 = r.ReadLEUInt16();
            numParticles = r.ReadLEUInt16();
            numValid = r.ReadLEUInt16();
            particles = new Particle[numParticles];
            for (var i = 0; i < particles.Length; i++)
            {
                particles[i] = new Particle();
                particles[i].Deserialize(r);
            }
            unknownLink = NiReaderUtils.ReadRef<NiObject>(r);
            particleExtra = NiReaderUtils.ReadRef<NiParticleModifier>(r);
            unknownLink2 = NiReaderUtils.ReadRef<NiObject>(r);
            trailer = r.ReadByte();
        }
    }

    public class NiBSPArrayController : NiParticleSystemController { }

    // Particle Modifiers
    public abstract class NiParticleModifier : NiObject
    {
        public Ref<NiParticleModifier> nextModifier;
        public Ptr<NiParticleSystemController> controller;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            nextModifier = NiReaderUtils.ReadRef<NiParticleModifier>(r);
            controller = NiReaderUtils.ReadPtr<NiParticleSystemController>(r);
        }
    }

    public class NiGravity : NiParticleModifier
    {
        public float unknownFloat1;
        public float force;
        public FieldType type;
        public Vector3 position;
        public Vector3 direction;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            unknownFloat1 = r.ReadLESingle();
            force = r.ReadLESingle();
            type = (FieldType)r.ReadLEUInt32();
            position = r.ReadLEVector3();
            direction = r.ReadLEVector3();
        }
    }

    public class NiParticleBomb : NiParticleModifier
    {
        public float decay;
        public float duration;
        public float deltaV;
        public float start;
        public DecayType decayType;
        public Vector3 position;
        public Vector3 direction;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            decay = r.ReadLESingle();
            duration = r.ReadLESingle();
            deltaV = r.ReadLESingle();
            start = r.ReadLESingle();
            decayType = (DecayType)r.ReadLEUInt32();
            position = r.ReadLEVector3();
            direction = r.ReadLEVector3();
        }
    }

    public class NiParticleColorModifier : NiParticleModifier
    {
        public Ref<NiColorData> colorData;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            colorData = NiReaderUtils.ReadRef<NiColorData>(r);
        }
    }

    public class NiParticleGrowFade : NiParticleModifier
    {
        public float grow;
        public float fade;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            grow = r.ReadLESingle();
            fade = r.ReadLESingle();
        }
    }

    public class NiParticleMeshModifier : NiParticleModifier
    {
        public uint numParticleMeshes;
        public Ref<NiAVObject>[] particleMeshes;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            numParticleMeshes = r.ReadLEUInt32();
            particleMeshes = new Ref<NiAVObject>[numParticleMeshes];
            for (var i = 0; i < particleMeshes.Length; i++)
                particleMeshes[i] = NiReaderUtils.ReadRef<NiAVObject>(r);
        }
    }

    public class NiParticleRotation : NiParticleModifier
    {
        public byte randomInitialAxis;
        public Vector3 initialAxis;
        public float rotationSpeed;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            randomInitialAxis = r.ReadByte();
            initialAxis = r.ReadLEVector3();
            rotationSpeed = r.ReadLESingle();
        }
    }

    // Controllers
    public abstract class NiTimeController : NiObject
    {
        public Ref<NiTimeController> nextController;
        public ushort flags;
        public float frequency;
        public float phase;
        public float startTime;
        public float stopTime;
        public Ptr<NiObjectNET> target;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            nextController = NiReaderUtils.ReadRef<NiTimeController>(r);
            flags = r.ReadLEUInt16();
            frequency = r.ReadLESingle();
            phase = r.ReadLESingle();
            startTime = r.ReadLESingle();
            stopTime = r.ReadLESingle();
            target = NiReaderUtils.ReadPtr<NiObjectNET>(r);
        }
    }

    public class NiUVController : NiTimeController
    {
        public ushort unknownShort;
        public Ref<NiUVData> data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            unknownShort = r.ReadLEUInt16();
            data = NiReaderUtils.ReadRef<NiUVData>(r);
        }
    }

    public abstract class NiInterpController : NiTimeController { }

    public abstract class NiSingleInterpController : NiInterpController { }

    public class NiKeyframeController : NiSingleInterpController
    {
        public Ref<NiKeyframeData> data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            data = NiReaderUtils.ReadRef<NiKeyframeData>(r);
        }
    }

    public class NiGeomMorpherController : NiInterpController
    {
        public Ref<NiMorphData> data;
        public byte alwaysUpdate;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            data = NiReaderUtils.ReadRef<NiMorphData>(r);
            alwaysUpdate = r.ReadByte();
        }
    }

    public abstract class NiBoolInterpController : NiSingleInterpController { }

    public class NiVisController : NiBoolInterpController
    {
        public Ref<NiVisData> data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            data = NiReaderUtils.ReadRef<NiVisData>(r);
        }
    }

    public abstract class NiFloatInterpController : NiSingleInterpController { }

    public class NiAlphaController : NiFloatInterpController
    {
        public Ref<NiFloatData> data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            data = NiReaderUtils.ReadRef<NiFloatData>(r);
        }
    }

    // Skin Stuff
    public class NiSkinInstance : NiObject
    {
        public Ref<NiSkinData> data;
        public Ptr<NiNode> skeletonRoot;
        public uint numBones;
        public Ptr<NiNode>[] bones;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            data = NiReaderUtils.ReadRef<NiSkinData>(r);
            skeletonRoot = NiReaderUtils.ReadPtr<NiNode>(r);
            numBones = r.ReadLEUInt32();
            bones = new Ptr<NiNode>[numBones];
            for (var i = 0; i < bones.Length; i++)
                bones[i] = NiReaderUtils.ReadPtr<NiNode>(r);
        }
    }

    public class NiSkinData : NiObject
    {
        public SkinTransform skinTransform;
        public uint numBones;
        public Ref<NiSkinPartition> skinPartition;
        public SkinData[] boneList;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            skinTransform = new SkinTransform();
            skinTransform.Deserialize(r);
            numBones = r.ReadLEUInt32();
            skinPartition = NiReaderUtils.ReadRef<NiSkinPartition>(r);
            boneList = new SkinData[numBones];
            for (var i = 0; i < boneList.Length; i++)
            {
                boneList[i] = new SkinData();
                boneList[i].Deserialize(r);
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
        public byte useExternal;
        public string fileName;
        public PixelLayout pixelLayout;
        public MipMapFormat useMipMaps;
        public AlphaFormat alphaFormat;
        public byte isStatic;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            useExternal = r.ReadByte();
            fileName = r.ReadLELength32PrefixedASCIIString();
            pixelLayout = (PixelLayout)r.ReadLEUInt32();
            useMipMaps = (MipMapFormat)r.ReadLEUInt32();
            alphaFormat = (AlphaFormat)r.ReadLEUInt32();
            isStatic = r.ReadByte();
        }
    }

    public abstract class NiPoint3InterpController : NiSingleInterpController
    {
        public Ref<NiPosData> data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            data = NiReaderUtils.ReadRef<NiPosData>(r);
        }
    }

    public class NiMaterialProperty : NiProperty
    {
        public ushort flags;
        public Color3 ambientColor;
        public Color3 diffuseColor;
        public Color3 specularColor;
        public Color3 emissiveColor;
        public float glossiness;
        public float alpha;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            flags = NiReaderUtils.ReadFlags(r);
            ambientColor = new Color3();
            ambientColor.Deserialize(r);
            diffuseColor = new Color3();
            diffuseColor.Deserialize(r);
            specularColor = new Color3();
            specularColor.Deserialize(r);
            emissiveColor = new Color3();
            emissiveColor.Deserialize(r);
            glossiness = r.ReadLESingle();
            alpha = r.ReadLESingle();
        }
    }

    public class NiMaterialColorController : NiPoint3InterpController { }

    public abstract class NiDynamicEffect : NiAVObject
    {
        uint numAffectedNodeListPointers;
        uint[] affectedNodeListPointers;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            numAffectedNodeListPointers = r.ReadLEUInt32();
            affectedNodeListPointers = new uint[numAffectedNodeListPointers];
            for (var i = 0; i < affectedNodeListPointers.Length; i++)
                affectedNodeListPointers[i] = r.ReadLEUInt32();
        }
    }

    public class NiTextureEffect : NiDynamicEffect
    {
        public Matrix4x4 modelProjectionMatrix;
        public Vector3 modelProjectionTransform;
        public TexFilterMode textureFiltering;
        public TexClampMode textureClamping;
        public EffectType textureType;
        public CoordGenType coordinateGenerationType;
        public Ref<NiSourceTexture> sourceTexture;
        public byte clippingPlane;
        public Vector3 unknownVector;
        public float unknownFloat;
        public short PS2L;
        public short PS2K;
        public ushort unknownShort;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            modelProjectionMatrix = NiReaderUtils.Read3x3RotationMatrix(r);
            modelProjectionTransform = r.ReadLEVector3();
            textureFiltering = (TexFilterMode)r.ReadLEUInt32();
            textureClamping = (TexClampMode)r.ReadLEUInt32();
            textureType = (EffectType)r.ReadLEUInt32();
            coordinateGenerationType = (CoordGenType)r.ReadLEUInt32();
            sourceTexture = NiReaderUtils.ReadRef<NiSourceTexture>(r);
            clippingPlane = r.ReadByte();
            unknownVector = r.ReadLEVector3();
            unknownFloat = r.ReadLESingle();
            PS2L = r.ReadLEInt16();
            PS2K = r.ReadLEInt16();
            unknownShort = r.ReadLEUInt16();
        }
    }
}