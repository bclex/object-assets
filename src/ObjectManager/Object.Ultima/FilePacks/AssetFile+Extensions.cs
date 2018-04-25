using OA.Core;
using System;
using System.Threading.Tasks;

namespace OA.Ultima.FilePacks
{
    partial class AssetFile
    {
        public Task<Texture2DInfo> LoadTextureInfoAsync(string texturePath)
        {
            Texture2DInfo texture = null;
            switch (texturePath.Substring(0, 3))
            {
                case "lnd": texture = _artmulResource.GetLandTexture(int.Parse(texturePath.Substring(3))); break;
                case "art": texture = _artmulResource.GetStaticTexture(int.Parse(texturePath.Substring(3))); break;
                case "gmp": texture = _gumpMulResource.GetGumpTexture(int.Parse(texturePath.Substring(3))); break;
                case "tex": texture = _texmapResource.GetTexmapTexture(int.Parse(texturePath.Substring(3))); break;
                default: throw new ArgumentOutOfRangeException("texturePath", texturePath);
            }
            return Task.FromResult(texture);
        }
        //case "ani": texture = _animationResource.GetAnimation(int.Parse(texturePath.Substring(3))); break;
        //case "fnt": texture = _fontsResource.GetAsciiFont(int.Parse(texturePath.Substring(3))); break;

        public Task<object> LoadObjectInfoAsync(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}