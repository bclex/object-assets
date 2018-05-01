﻿using OA.Core;
using System;
using UnityEngine;

namespace OA.Tes.Formats
{
    // Refers to an object before the current one in the hierarchy.
    public struct Ptr<T>
    {
        public int Value;
        public bool IsNull
        {
            get { return Value < 0; }
        }

        public void Deserialize(UnityBinaryReader reader)
        {
            Value = reader.ReadLEInt32();
        }
    }

    // Refers to an object after the current one in the hierarchy.
    public struct Ref<T>
    {
        public int Value;
        public bool IsNull
        {
            get { return Value < 0; }
        }

        public void Deserialize(UnityBinaryReader r)
        {
            Value = r.ReadLEInt32();
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
            else throw new NotImplementedException("Tried to read an unsupported type.");
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
            Name = name;
        }

        public string Name;
        public NiHeader Header;
        public NiObject[] Blocks;
        public NiFooter Footer;

        public void Deserialize(UnityBinaryReader r)
        {
            Header = new NiHeader();
            Header.Deserialize(r);
            Blocks = new NiObject[Header.NumBlocks];
            for (var i = 0; i < Header.NumBlocks; i++)
                Blocks[i] = NiReaderUtils.ReadNiObject(r);
            Footer = new NiFooter();
            Footer.Deserialize(r);
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
        public byte[] Str; // 40 bytes (including \n)
        public uint Version;
        public uint NumBlocks;

        public void Deserialize(UnityBinaryReader r)
        {
            Str = r.ReadBytes(40);
            Version = r.ReadLEUInt32();
            NumBlocks = r.ReadLEUInt32();
        }
    }

    public class NiFooter
    {
        public uint NumRoots;
        public int[] Roots;

        public void Deserialize(UnityBinaryReader r)
        {
            NumRoots = r.ReadLEUInt32();
            Roots = new int[NumRoots];
            for (var i = 0; i < NumRoots; i++)
                Roots[i] = r.ReadLEInt32();
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
        public string Name;
        public Ref<NiExtraData> ExtraData;
        public Ref<NiTimeController> Controller;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Name = r.ReadLELength32PrefixedASCIIString();
            ExtraData = NiReaderUtils.ReadRef<NiExtraData>(r);
            Controller = NiReaderUtils.ReadRef<NiTimeController>(r);
        }
    }

    public abstract class NiAVObject : NiObjectNET
    {
        public enum NiFlags
        {
            Hidden = 0x1
        }

        public ushort Flags;
        public Vector3 Translation;
        public Matrix4x4 Rotation;
        public float Scale;
        public Vector3 Velocity;
        //public uint numProperties;
        public Ref<NiProperty>[] Properties;
        public bool HasBoundingBox;
        public BoundingBox BoundingBox;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Flags = NiReaderUtils.ReadFlags(r);
            Translation = r.ReadLEVector3();
            Rotation = NiReaderUtils.Read3x3RotationMatrix(r);
            Scale = r.ReadLESingle();
            Velocity = r.ReadLEVector3();
            Properties = NiReaderUtils.ReadLengthPrefixedRefs32<NiProperty>(r);
            HasBoundingBox = r.ReadLEBool32();
            if (HasBoundingBox)
            {
                BoundingBox = new BoundingBox();
                BoundingBox.Deserialize(r);
            }
        }
    }

    // Nodes
    public class NiNode : NiAVObject
    {
        //public uint numChildren;
        public Ref<NiAVObject>[] Children;
        //public uint numEffects;
        public Ref<NiDynamicEffect>[] Effects;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Children = NiReaderUtils.ReadLengthPrefixedRefs32<NiAVObject>(r);
            Effects = NiReaderUtils.ReadLengthPrefixedRefs32<NiDynamicEffect>(r);
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
        public Ref<NiGeometryData> Data;
        public Ref<NiSkinInstance> SkinInstance;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Data = NiReaderUtils.ReadRef<NiGeometryData>(r);
            SkinInstance = NiReaderUtils.ReadRef<NiSkinInstance>(r);
        }
    }

    public abstract class NiGeometryData : NiObject
    {
        public ushort NumVertices;
        public bool HasVertices;
        public Vector3[] Vertices;
        public bool HasNormals;
        public Vector3[] Normals;
        public Vector3 Center;
        public float Radius;
        public bool HasVertexColors;
        public Color4[] VertexColors;
        public ushort NumUVSets;
        public bool HasUV;
        public TexCoord[,] UVSets;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NumVertices = r.ReadLEUInt16();
            HasVertices = r.ReadLEBool32();
            if (HasVertices)
            {
                Vertices = new Vector3[NumVertices];
                for (var i = 0; i < Vertices.Length; i++)
                    Vertices[i] = r.ReadLEVector3();
            }
            HasNormals = r.ReadLEBool32();
            if (HasNormals)
            {
                Normals = new Vector3[NumVertices];
                for (var i = 0; i < Normals.Length; i++)
                    Normals[i] = r.ReadLEVector3();
            }
            Center = r.ReadLEVector3();
            Radius = r.ReadLESingle();
            HasVertexColors = r.ReadLEBool32();
            if (HasVertexColors)
            {
                VertexColors = new Color4[NumVertices];
                for (var i = 0; i < VertexColors.Length; i++)
                {
                    VertexColors[i] = new Color4();
                    VertexColors[i].Deserialize(r);
                }
            }
            NumUVSets = r.ReadLEUInt16();
            HasUV = r.ReadLEBool32();
            if (HasUV)
            {
                UVSets = new TexCoord[NumUVSets, NumVertices];
                for (var i = 0; i < NumUVSets; i++)
                    for (var j = 0; j < NumVertices; j++)
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
        public ushort NumTriangles;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NumTriangles = r.ReadLEUInt16();
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
        public uint NumTrianglePoints;
        public Triangle[] Triangles;
        public ushort NumMatchGroups;
        public MatchGroup[] MatchGroups;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NumTrianglePoints = r.ReadLEUInt32();
            Triangles = new Triangle[NumTriangles];
            for (var i = 0; i < Triangles.Length; i++)
            {
                Triangles[i] = new Triangle();
                Triangles[i].Deserialize(r);
            }
            NumMatchGroups = r.ReadLEUInt16();
            MatchGroups = new MatchGroup[NumMatchGroups];
            for (var i = 0; i < MatchGroups.Length; i++)
            {
                MatchGroups[i] = new MatchGroup();
                MatchGroups[i].Deserialize(r);
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
        public ushort Flags;
        public ApplyMode ApplyMode;
        public uint TextureCount;
        public bool HasBaseTexture;
        public TexDesc BaseTexture;
        public bool HasDarkTexture;
        public TexDesc DarkTexture;
        public bool HasDetailTexture;
        public TexDesc DetailTexture;
        public bool HasGlossTexture;
        public TexDesc GlossTexture;
        public bool HasGlowTexture;
        public TexDesc GlowTexture;
        public bool HasBumpMapTexture;
        public TexDesc BumpMapTexture;
        public bool HasDecal0Texture;
        public TexDesc Decal0Texture;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Flags = NiReaderUtils.ReadFlags(r);
            ApplyMode = (ApplyMode)r.ReadLEUInt32();
            TextureCount = r.ReadLEUInt32();
            HasBaseTexture = r.ReadLEBool32();
            if (HasBaseTexture)
            {
                BaseTexture = new TexDesc();
                BaseTexture.Deserialize(r);
            }
            HasDarkTexture = r.ReadLEBool32();
            if (HasDarkTexture)
            {
                DarkTexture = new TexDesc();
                DarkTexture.Deserialize(r);
            }
            HasDetailTexture = r.ReadLEBool32();
            if (HasDetailTexture)
            {
                DetailTexture = new TexDesc();
                DetailTexture.Deserialize(r);
            }
            HasGlossTexture = r.ReadLEBool32();
            if (HasGlossTexture)
            {
                GlossTexture = new TexDesc();
                GlossTexture.Deserialize(r);
            }
            HasGlowTexture = r.ReadLEBool32();
            if (HasGlowTexture)
            {
                GlowTexture = new TexDesc();
                GlowTexture.Deserialize(r);
            }
            HasBumpMapTexture = r.ReadLEBool32();
            if (HasBumpMapTexture)
            {
                BumpMapTexture = new TexDesc();
                BumpMapTexture.Deserialize(r);
            }
            HasDecal0Texture = r.ReadLEBool32();
            if (HasDecal0Texture)
            {
                Decal0Texture = new TexDesc();
                Decal0Texture.Deserialize(r);
            }
        }
    }
    public class NiAlphaProperty : NiProperty
    {
        public ushort Flags;
        public byte Threshold;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Flags = r.ReadLEUInt16();
            Threshold = r.ReadByte();
        }
    }
    public class NiZBufferProperty : NiProperty
    {
        public ushort Flags;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Flags = r.ReadLEUInt16();
        }
    }

    public class NiVertexColorProperty : NiProperty
    {
        public ushort Flags;
        public VertMode VertexMode;
        public LightMode LightingMode;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Flags = NiReaderUtils.ReadFlags(r);
            VertexMode = (VertMode)r.ReadLEUInt32();
            LightingMode = (LightMode)r.ReadLEUInt32();
        }
    }

    public class NiShadeProperty : NiProperty
    {
        public ushort Flags;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Flags = NiReaderUtils.ReadFlags(r);
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
        public uint NumRotationKeys;
        public KeyType RotationType;
        public QuatKey<Quaternion>[] QuaternionKeys;
        public float UnknownFloat;
        public KeyGroup<float>[] XYZRotations;
        public KeyGroup<Vector3> Translations;
        public KeyGroup<float> Scales;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NumRotationKeys = r.ReadLEUInt32();
            if (NumRotationKeys != 0)
            {
                RotationType = (KeyType)r.ReadLEUInt32();
                if (RotationType != KeyType.XYZ_ROTATION_KEY)
                {
                    QuaternionKeys = new QuatKey<Quaternion>[NumRotationKeys];
                    for (var i = 0; i < QuaternionKeys.Length; i++)
                    {
                        QuaternionKeys[i] = new QuatKey<Quaternion>();
                        QuaternionKeys[i].Deserialize(r, RotationType);
                    }
                }
                else
                {
                    UnknownFloat = r.ReadLESingle();
                    XYZRotations = new KeyGroup<float>[3];
                    for (var i = 0; i < XYZRotations.Length; i++)
                    {
                        XYZRotations[i] = new KeyGroup<float>();
                        XYZRotations[i].Deserialize(r);
                    }
                }
            }
            Translations = new KeyGroup<Vector3>();
            Translations.Deserialize(r);
            Scales = new KeyGroup<float>();
            Scales.Deserialize(r);
        }
    }

    public class NiColorData : NiObject
    {
        public KeyGroup<Color4> Data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Data = new KeyGroup<Color4>();
            Data.Deserialize(r);
        }
    }

    public class NiMorphData : NiObject
    {
        public uint NumMorphs;
        public uint NumVertices;
        public byte RelativeTargets;
        public Morph[] Morphs;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NumMorphs = r.ReadLEUInt32();
            NumVertices = r.ReadLEUInt32();
            RelativeTargets = r.ReadByte();
            Morphs = new Morph[NumMorphs];
            for (var i = 0; i < Morphs.Length; i++)
            {
                Morphs[i] = new Morph();
                Morphs[i].Deserialize(r, NumVertices);
            }
        }
    }

    public class NiVisData : NiObject
    {
        public uint NumKeys;
        public Key<byte>[] Keys;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NumKeys = r.ReadLEUInt32();
            Keys = new Key<byte>[NumKeys];
            for (var i = 0; i < Keys.Length; i++)
            {
                Keys[i] = new Key<byte>();
                Keys[i].Deserialize(r, KeyType.LINEAR_KEY);
            }
        }
    }

    public class NiFloatData : NiObject
    {
        public KeyGroup<float> Data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Data = new KeyGroup<float>();
            Data.Deserialize(r);
        }
    }

    public class NiPosData : NiObject
    {
        public KeyGroup<Vector3> Data;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            Data = new KeyGroup<Vector3>();
            Data.Deserialize(r);
        }
    }

    public class NiExtraData : NiObject
    {
        public Ref<NiExtraData> NextExtraData;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NextExtraData = NiReaderUtils.ReadRef<NiExtraData>(r);
        }
    }

    public class NiStringExtraData : NiExtraData
    {
        public uint BytesRemaining;
        public string Str;

        public override void Deserialize(UnityBinaryReader reader)
        {
            base.Deserialize(reader);
            BytesRemaining = reader.ReadLEUInt32();
            Str = reader.ReadLELength32PrefixedASCIIString();
        }
    }

    public class NiTextKeyExtraData : NiExtraData
    {
        public uint UnknownInt1;
        public uint NumTextKeys;
        public Key<string>[] TextKeys;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            UnknownInt1 = r.ReadLEUInt32();
            NumTextKeys = r.ReadLEUInt32();
            TextKeys = new Key<string>[NumTextKeys];
            for (var i = 0; i < TextKeys.Length; i++)
            {
                TextKeys[i] = new Key<string>();
                TextKeys[i].Deserialize(r, KeyType.LINEAR_KEY);
            }
        }
    }

    public class NiVertWeightsExtraData : NiExtraData
    {
        public uint NumBytes;
        public ushort NumVertices;
        public float[] Weights;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NumBytes = r.ReadLEUInt32();
            NumVertices = r.ReadLEUInt16();
            Weights = new float[NumVertices];
            for (var i = 0; i < Weights.Length; i++)
                Weights[i] = r.ReadLESingle();
        }
    }

    // Particles
    public class NiParticles : NiGeometry { }

    public class NiParticlesData : NiGeometryData
    {
        public ushort NumParticles;
        public float ParticleRadius;
        public ushort NumActive;
        public bool HasSizes;
        public float[] Sizes;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            NumParticles = r.ReadLEUInt16();
            ParticleRadius = r.ReadLESingle();
            NumActive = r.ReadLEUInt16();
            HasSizes = r.ReadLEBool32();
            if (HasSizes)
            {
                Sizes = new float[NumVertices];
                for (var i = 0; i < Sizes.Length; i++)
                    Sizes[i] = r.ReadLESingle();
            }
        }
    }

    public class NiRotatingParticles : NiParticles { }

    public class NiRotatingParticlesData : NiParticlesData
    {
        public bool HasRotations;
        public Quaternion[] Rotations;

        public override void Deserialize(UnityBinaryReader r)
        {
            base.Deserialize(r);
            HasRotations = r.ReadLEBool32();
            if (HasRotations)
            {
                Rotations = new Quaternion[NumVertices];
                for (var i = 0; i < Rotations.Length; i++)
                    Rotations[i] = r.ReadLEQuaternionWFirst();
            }
        }
    }

    public class NiAutoNormalParticles : NiParticles { }

    public class NiAutoNormalParticlesData : NiParticlesData { }

    public class NiParticleSystemController : NiTimeController
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