using UnityEngine;
using ur = UnityEngine.Rendering;

namespace OA.Materials
{
    /// <summary>
    /// A material that uses the default shader created for TESUnity.
    /// </summary>
    public class DefaultMaterial : BaseMaterial
    {
        public DefaultMaterial(TextureManager textureManager) : base(textureManager) { }

        public override Material BuildMaterialFromProperties(MaterialProps mp)
        {
            // check if the material is already cached
            if (!_existingMaterials.TryGetValue(mp, out Material material))
            {
                // otherwise create a new material and cache it
                if (mp.AlphaBlended) material = BuildMaterialBlended(mp.SrcBlendMode, mp.DstBlendMode);
                else if (mp.AlphaTest) material = BuildMaterialTested(mp.AlphaCutoff);
                else material = BuildMaterial();
                if (mp.Textures.MainFilePath != null && material.HasProperty("_MainTex")) material.SetTexture("_MainTex", _textureManager.LoadTexture(mp.Textures.MainFilePath));
                if (mp.Textures.DetailFilePath != null && material.HasProperty("_DetailTex")) material.SetTexture("_DetailTex", _textureManager.LoadTexture(mp.Textures.DetailFilePath));
                if (mp.Textures.DarkFilePath != null && material.HasProperty("_DarkTex")) material.SetTexture("_DarkTex", _textureManager.LoadTexture(mp.Textures.DarkFilePath));
                if (mp.Textures.GlossFilePath != null && material.HasProperty("_GlossTex")) material.SetTexture("_GlossTex", _textureManager.LoadTexture(mp.Textures.GlossFilePath));
                if (mp.Textures.GlowFilePath != null && material.HasProperty("_Glowtex")) material.SetTexture("_Glowtex", _textureManager.LoadTexture(mp.Textures.GlowFilePath));
                if (mp.Textures.BumpFilePath != null && material.HasProperty("_BumpTex")) material.SetTexture("_BumpTex", _textureManager.LoadTexture(mp.Textures.BumpFilePath));
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