using OA.Configuration;
using OA.Materials;
using UnityEngine;
using UnityEngine.Rendering;

namespace OA
{
    public enum MatTestMode { Always, Less, LEqual, Equal, GEqual, Greater, NotEqual, Never }

    public enum MaterialType
    {
        None, Default, Standard, BumpedDiffuse, Unlit
    }

    public struct MaterialTextures
    {
        public string MainFilePath;
        public string DarkFilePath;
        public string DetailFilePath;
        public string GlossFilePath;
        public string GlowFilePath;
        public string BumpFilePath;
    }

    public struct MaterialProps
    {
        public MaterialTextures Textures;
        public bool AlphaBlended;
        public BlendMode SrcBlendMode;
        public BlendMode DstBlendMode;
        public bool AlphaTest;
        public float AlphaCutoff;
        public bool ZWrite;
    }

    /// <summary>
    /// Manages loading and instantiation of materials.
    /// </summary>
    public class MaterialManager
    {
        BaseMaterial _material;

        public TextureManager TextureManager { get; }

        public MaterialManager(TextureManager textureManager)
        {
            TextureManager = textureManager;
            var game = BaseSettings.Game;
            switch (game.MaterialType)
            {
                case MaterialType.None: _material = null; break;
                case MaterialType.Default: _material = new DefaultMaterial(textureManager); break;
                case MaterialType.Standard: _material = new StandardMaterial(textureManager); break;
                case MaterialType.Unlit: _material = new UnliteMaterial(textureManager); break;
                default: _material = new BumpedDiffuseMaterial(textureManager); break;
            }
        }

        public Material BuildMaterialFromProperties(MaterialProps mp)
        {
            return _material.BuildMaterialFromProperties(mp);
        }

        private Material BuildMaterial()
        {
            return _material.BuildMaterial();
        }

        private Material BuildMaterialBlended(BlendMode sourceBlendMode, BlendMode destinationBlendMode)
        {
            return _material.BuildMaterialBlended(sourceBlendMode, destinationBlendMode);
        }

        private Material BuildMaterialTested(float cutoff = 0.5f)
        {
            return _material.BuildMaterialTested(cutoff);
        }
    }
}