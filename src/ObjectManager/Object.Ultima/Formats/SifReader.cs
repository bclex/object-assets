using OA.Ultima.Resources;
using System;
using UnityEngine;

namespace OA.Ultima.Formats
{
    // Refers to an object after the current one in the hierarchy.
    public struct Ref<T>
    {
        public Ref(int value)
        {
            Value = value;
        }
        public int Value;
        public bool IsNull => Value <= 0;
    }

    public class SiFile
    {
        public SiFile(string filePath)
        {
            switch (filePath.Substring(0, 3))
            {
                case "sta": StaBlocks(filePath, short.Parse(filePath.Substring(3))); break;
                default: throw new ArgumentOutOfRangeException("filePath", filePath);
            }
        }

        public readonly string Name;

        private void StaBlocks(string filePath, short itemId)
        {
            var itemData = TileData.ItemData[itemId];
            var Name = itemData.Name;
            Blocks = new SiObject[]
            {
                new SiPrimitive
                {
                    Name = Name,
                    Properties = new[] { new Ref<SiProperty>(1) },
                    Height = Math.Max(itemData.CalcHeight / ConvertUtils.MeterInUnits, 0.01F),
                },
                new SiTexturingProperty { TextureCount = 1, BaseTexture = new TexDesc { Source = new Ref<SiSourceTexture>(2) } },
                new SiSourceTexture { FilePath = filePath },
            };
        }

        public SiObject[] Blocks;
    }

    #region Misc

    public class TexDesc
    {
        public Ref<SiSourceTexture> Source;
    }

    #endregion

    /// <summary>
    /// These are the main units of data that STA files are arranged in.
    /// </summary>
    public abstract class SiObject
    {
    }

    /// <summary>
    /// An object that can be controlled by a controller.
    /// </summary>
    public abstract class SiObjectNET : SiObject
    {
        public string Name;
        public Ref<SiExtraData> ExtraData;
    }

    public abstract class SiAVObject : SiObjectNET
    {
        public enum SiFlags
        {
            Hidden = 0x1
        }

        public ushort Flags;
        public Vector3 Translation;
        public Matrix4x4 Rotation = Matrix4x4.identity;
        public float Scale = 1;
        public Ref<SiProperty>[] Properties;
    }

    // Nodes
    public class SiNode : SiAVObject
    {
        public Ref<SiAVObject>[] Children;
    }

    public class RootCollisionNode : SiNode { }

    public class AvoidNode : SiNode { }

    // Geometry
    public abstract class SiGeometry : SiAVObject
    {
        public Ref<SiGeometryData> Data;
    }

    public abstract class SiGeometryData : SiObject
    {
        //public ushort NumVertices;
        //public bool HasVertices;
        //public Vector3[] Vertices;
        //public bool HasNormals;
        //public Vector3[] Normals;
        //public Vector3 Center;
        //public float Radius;
        //public bool HasVertexColors;
        //public Color4[] VertexColors;
        //public ushort NumUVSets;
        //public bool HasUV;
        //public TexCoord[,] UVSets;
    }

    public class SiPrimitive : SiGeometry
    {
        public float Height;
    }

    public class SiTriShape : SiGeometry
    {
    }

    public class SiTriShapeData : SiGeometryData
    {
    }

    // Properties
    public abstract class SiProperty : SiObjectNET
    {
    }

    public class SiTexturingProperty : SiProperty
    {
        public uint TextureCount;
        public TexDesc BaseTexture;
        public TexDesc DarkTexture;
        public TexDesc DetailTexture;
        public TexDesc GlossTexture;
        public TexDesc GlowTexture;
        public TexDesc BumpMapTexture;
    }

    public class SiAlphaProperty : SiProperty
    {
        public ushort Flags;
        public byte Threshold;
    }

    // Data
    public class SiExtraData : SiObject
    {
        public Ref<SiExtraData> NextExtraData;
    }

    public class SiStringExtraData : SiExtraData
    {
        public string Str;
    }

    // Miscellaneous
    public abstract class SiTexture : SiObjectNET
    {
    }

    public class SiSourceTexture : SiTexture
    {
        public string FilePath;
    }
}