using UnityEngine;
using ur = UnityEngine.Rendering;

namespace OA.Materials
{
    /// <summary>
    /// A material that uses the Unlit Shader.
    /// </summary>
    public class UnliteMaterial : BaseMaterial
    {
        public UnliteMaterial(TextureManager textureManager) : base(textureManager) { }

        public override Material BuildMaterialFromProperties(MaterialProps mp)
        {
            // check if the material is already cached
            if (!_existingMaterials.TryGetValue(mp, out Material material))
            {
                // otherwise create a new material and cache it
                if (mp.AlphaBlended) material = BuildMaterialBlended(mp.SrcBlendMode, mp.DstBlendMode);
                else if (mp.AlphaTest) material = BuildMaterialTested(mp.AlphaCutoff);
                else material = BuildMaterial();
                if (mp.Textures.MainFilePath != null) material.mainTexture = _textureManager.LoadTexture(mp.Textures.MainFilePath);
                _existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            return new Material(Shader.Find("Unlit/Texture"));
        }

        public override Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            var material = BuildMaterialTested();
            material.SetInt("_SrcBlend", (int)sourceBlendMode);
            material.SetInt("_DstBlend", (int)destinationBlendMode);
            return material;
        }

        public override Material BuildMaterialTested(float cutoff = 0.5f)
        {
            var material = new Material(Shader.Find("Unlit/Transparent Cutout"));
            material.SetFloat("_AlphaCutoff", cutoff);
            return material;
        }
    }
}