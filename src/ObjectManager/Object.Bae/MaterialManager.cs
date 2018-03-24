using OA.Bae.Materials;
using UnityEngine;
using ur = UnityEngine.Rendering;

namespace OA.Bae
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
        public ur.BlendMode srcBlendMode;
        public ur.BlendMode dstBlendMode;
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
        TextureManager _textureManager;

        public TextureManager TextureManager
        {
            get { return _textureManager; }
        }

        public MaterialManager(TextureManager textureManager)
        {
            _textureManager = textureManager;
            switch (BaeSettings.materialType)
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

        private Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            return _material.BuildMaterialBlended(sourceBlendMode, destinationBlendMode);
        }

        private Material BuildMaterialTested(float cutoff = 0.5f)
        {
            return _material.BuildMaterialTested(cutoff);
        }
    }
}