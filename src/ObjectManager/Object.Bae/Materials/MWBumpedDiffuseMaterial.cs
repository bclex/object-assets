using UnityEngine;
using ur = UnityEngine.Rendering;

namespace OA.Bae.Materials
{
    /// <summary>
    /// A material that uses the legacy Bumped Diffuse Shader.
    /// </summary>
    public class MWBumpedDiffuseMaterial : MWBaseMaterial
    {
        public MWBumpedDiffuseMaterial(TextureManager textureManager) : base(textureManager) { }

        public override Material BuildMaterialFromProperties(MWMaterialProps mp)
        {
            Material material;
            //check if the material is already cached
            if (!_existingMaterials.TryGetValue(mp, out material))
            {
                //otherwise create a new material and cache it
                if (mp.alphaBlended) material = BuildMaterialBlended(mp.srcBlendMode, mp.dstBlendMode);
                else if (mp.alphaTest) material = BuildMaterialTested(mp.alphaCutoff);
                else material = BuildMaterial();
                if (mp.textures.mainFilePath != null)
                {
                    material.mainTexture = _textureManager.LoadTexture(mp.textures.mainFilePath);
                    if (BaeSettings.generateNormalMap) material.SetTexture("_BumpMap", GenerateNormalMap((Texture2D)material.mainTexture, BaeSettings.normalGeneratorIntensity));
                }
                if (mp.textures.bumpFilePath != null) material.SetTexture("_BumpMap", _textureManager.LoadTexture(mp.textures.bumpFilePath));
                _existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            return new Material(Shader.Find("Legacy Shaders/Bumped Diffuse"));
        }

        public override Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            var material = new Material(Shader.Find("Legacy Shaders/Transparent/Cutout/Bumped Diffuse"));
            material.SetInt("_SrcBlend", (int)sourceBlendMode);
            material.SetInt("_DstBlend", (int)destinationBlendMode);
            return material;
        }

        public override Material BuildMaterialTested(float cutoff = 0.5f)
        {
            var material = new Material(Shader.Find("Legacy Shaders/Transparent/Cutout/Bumped Diffuse"));
            material.SetFloat("_AlphaCutoff", cutoff);
            return material;
        }
    }
}