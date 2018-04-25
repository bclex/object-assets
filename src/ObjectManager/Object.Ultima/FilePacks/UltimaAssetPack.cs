using OA.Core;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace OA.Ultima.FilePacks
{
    public class UltimaAssetPack : AssetFile, IAssetPack
    {
        TextureManager _textureManager;
        MaterialManager _materialManager;

        public UltimaAssetPack()
        {
            _textureManager = new TextureManager(this);
            _materialManager = new MaterialManager(_textureManager);
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
            return null;
        }

        public void PreloadObjectAsync(string filePath)
        {
        }
    }
}
