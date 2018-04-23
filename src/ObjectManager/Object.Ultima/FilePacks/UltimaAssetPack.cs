using System;
using System.IO;
using System.Threading.Tasks;
using OA.Core;
using UnityEngine;

namespace OA.Ultima.FilePacks
{
    public class UltimaAssetPack : AssetFile, IAssetPack
    {
        TextureManager _textureManager;
        MaterialManager _materialManager;
        string _webPath;

        public UltimaAssetPack(string searchPath, string webPath)
            :base(null)
        {
            _webPath = webPath;
            _textureManager = new TextureManager(this);
            _materialManager = new MaterialManager(_textureManager);
        }

        public Texture2D LoadTexture(string texturePath, bool flipVertically = false)
        {
            return _textureManager.LoadTexture(texturePath, flipVertically);
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

        public Task<Texture2DInfo> LoadTextureInfoAsync(string texturePath)
        {
            throw new NotImplementedException();
        }

        public Task<object> LoadObjectInfoAsync(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
