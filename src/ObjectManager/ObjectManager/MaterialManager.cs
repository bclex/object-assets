using OA.Configuration;
using OA.Materials;
using UnityEngine;
using UnityEngine.Rendering;

namespace OA
{
    public enum MatTestMode { Always, Less, LEqual, Equal, GEqual, Greater, NotEqual, Never }

    public enum MaterialType
    {
        Default, Standard, BumpedDiffuse, Unlit
    }

    public struct MaterialTextures
    {
        public string mainFilePath;
        public string darkFilePath;
        public string detailFilePath;
        public string glossFilePath;
        public string glowFilePath;
        public string bumpFilePath;
    }

    public struct MaterialProps
    {
        public MaterialTextures textures;
        public bool alphaBlended;
        public BlendMode srcBlendMode;
        public BlendMode dstBlendMode;
        public bool alphaTest;
        public float alphaCutoff;
        public bool zWrite;
    }

    /// <summary>
    /// Manages loading and instantiation of Morrowind materials.
    /// </summary>
    public class MaterialManager
    {
        BaseMaterial _material;

        public TextureManager TextureManager { get; }

        public MaterialManager(TextureManager textureManager)
        {
            var game = BaseSettings.Game;
            TextureManager = textureManager;
            switch (game.MaterialType)
            {
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