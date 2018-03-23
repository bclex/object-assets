using UnityEngine;
using ur = UnityEngine.Rendering;

namespace OA.Bae.Materials
{
    /// <summary>
    /// A material that uses the default shader created for TESUnity.
    /// </summary>
    public class MWDefaultMaterial : MWBaseMaterial
    {
        public MWDefaultMaterial(TextureManager textureManager) : base(textureManager) { }

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
                if (mp.textures.mainFilePath != null && material.HasProperty("_MainTex")) material.SetTexture("_MainTex", _textureManager.LoadTexture(mp.textures.mainFilePath));
                if (mp.textures.detailFilePath != null && material.HasProperty("_DetailTex")) material.SetTexture("_DetailTex", _textureManager.LoadTexture(mp.textures.detailFilePath));
                if (mp.textures.darkFilePath != null && material.HasProperty("_DarkTex")) material.SetTexture("_DarkTex", _textureManager.LoadTexture(mp.textures.darkFilePath));
                if (mp.textures.glossFilePath != null && material.HasProperty("_GlossTex")) material.SetTexture("_GlossTex", _textureManager.LoadTexture(mp.textures.glossFilePath));
                if (mp.textures.glowFilePath != null && material.HasProperty("_Glowtex")) material.SetTexture("_Glowtex", _textureManager.LoadTexture(mp.textures.glowFilePath));
                if (mp.textures.bumpFilePath != null && material.HasProperty("_BumpTex")) material.SetTexture("_BumpTex", _textureManager.LoadTexture(mp.textures.bumpFilePath));
                if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", 0f);
                if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", 0f);
                _existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            return new Material(Shader.Find("TES Unity/Standard"));
        }

        public override Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            var material = new Material(Shader.Find("TES Unity/Alpha Blended"));
            material.SetInt("_SrcBlend", (int)sourceBlendMode);
            material.SetInt("_DstBlend", (int)destinationBlendMode);
            return material;
        }

        public override Material BuildMaterialTested(float cutoff = 0.5f)
        {
            var material = new Material(Shader.Find("TES Unity/Alpha Tested"));
            material.SetFloat("_Cutoff", cutoff);
            return material;
        }
    }
}