using OA.Ultima.Formats;
using UnityEngine;

namespace OA.Ultima.FilePacks
{
    public class UltimaAssetPack : AssetFile, IAssetPack
    {
        TextureManager _textureManager;
        MaterialManager _materialManager;
        StaManager _staManager;

        public UltimaAssetPack()
        {
            _textureManager = new TextureManager(this);
            _materialManager = new MaterialManager(_textureManager);
            _staManager = new StaManager(this, _materialManager);
        }

        public Texture2D LoadTexture(string texturePath, int method = 0)
        {
            return _textureManager.LoadTexture(texturePath, method);
        }

        public void PreloadTextureAsync(string texturePath)
        {
            _textureManager.PreloadTextureFileAsync(texturePath);
        }

        public GameObject CreateObject(string filePath)
        {
            return _staManager.InstantiateSta(filePath);
        }

        public void PreloadObjectAsync(string filePath)
        {
            _staManager.PreloadStaFileAsync(filePath);
        }
    }
}
