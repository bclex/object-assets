using OA.Ultima.Resources;
using System;
using UnityEngine;

namespace OA.Ultima.Formats
{
    // Refers to an object after the current one in the hierarchy.
    public struct Ref<T>
    {
        public int Value;
        public bool IsNull => Value < 0;
    }

    public class SiFile
    {
        public SiFile(string filePath)
        {
            switch (filePath.Substring(0, 3))
            {
                case "sta": StaBlocks(filePath, int.Parse(filePath.Substring(3))); break;
                default: throw new ArgumentOutOfRangeException("filePath", filePath);
            }
        }

        public readonly string Name;

        private void StaBlocks(string filePath, int itemId)
        {
            var itemData = TileData.ItemData[itemId];
            var Name = itemData.Name;
            Blocks = new[] {
                new SiSourceTexture { FilePath = filePath },
            };
        }

        public SiObject[] Blocks;
    }

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
        public Matrix4x4 Rotation;
        public float Scale;
    }

    // Nodes
    public class SiNode : SiAVObject
    {
        public Ref<SiAVObject>[] Children;
    }

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