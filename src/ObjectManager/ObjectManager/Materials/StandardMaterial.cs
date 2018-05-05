using OA.Configuration;
using UnityEngine;
using ur = UnityEngine.Rendering;

namespace OA.Materials
{
    /// <summary>
    /// A material that uses the new Standard Shader.
    /// </summary>
    public class StandardMaterial : BaseMaterial
    {
        Material _standardMaterial;
        Material _standardCutoutMaterial;

        public StandardMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            _standardMaterial = new Material(Shader.Find("Standard"));
            _standardCutoutMaterial = Resources.Load<Material>("Materials/StandardCutout");
        }

        public override Material BuildMaterialFromProperties(MaterialProps mp)
        {
            var game = BaseSettings.Game;
            // check if the material is already cached
            if (!_existingMaterials.TryGetValue(mp, out Material material))
            {
                // otherwise create a new material and cache it
                if (mp.AlphaBlended)
                    material = BuildMaterialBlended(mp.SrcBlendMode, mp.DstBlendMode);
                else if (mp.AlphaTest) material = BuildMaterialTested(mp.AlphaCutoff);
                else material = BuildMaterial();
                if (mp.Textures.MainFilePath != null)
                {
                    material.mainTexture = _textureManager.LoadTexture(mp.Textures.MainFilePath);
                    if (game.GenerateNormalMap)
                    {
                        material.EnableKeyword("_NORMALMAP");
                        material.SetTexture("_BumpMap", GenerateNormalMap((Texture2D)material.mainTexture, game.NormalGeneratorIntensity));
                    }
                }
                else material.DisableKeyword("_NORMALMAP");
                if (mp.Textures.BumpFilePath != null)
                {
                    material.EnableKeyword("_NORMALMAP");
                    material.SetTexture("_NORMALMAP", _textureManager.LoadTexture(mp.Textures.BumpFilePath));
                }
                _existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            var material = new Material(Shader.Find("Standard"));
            material.CopyPropertiesFromMaterial(_standardMaterial);
            return material;
        }

        public override Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            var material = BuildMaterialTested();
            //material.SetInt("_SrcBlend", (int)sourceBlendMode);
            //material.SetInt("_DstBlend", (int)destinationBlendMode);
            return material;
        }

        public override Material BuildMaterialTested(float cutoff = 0.5f)
        {
            var material = new Material(Shader.Find("Standard"));
            material.CopyPropertiesFromMaterial(_standardCutoutMaterial);
            material.SetFloat("_Cutout", cutoff);
            return material;
        }
    }
}