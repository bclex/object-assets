using OA.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace OA
{
    public class TextureManager
    {
        readonly IAssetPack _asset;
        readonly Dictionary<string, Task<Texture2DInfo>> _textureFilePreloadTasks = new Dictionary<string, Task<Texture2DInfo>>();
        readonly Dictionary<string, Texture2D> _cachedTextures = new Dictionary<string, Texture2D>();

        public TextureManager(IAssetPack asset)
        {
            _asset = asset;
        }

        public Texture2D LoadTexture(string texturePath, int method = 1)
        {
            if (!_cachedTextures.TryGetValue(texturePath, out Texture2D texture))
            {
                // Load & cache the texture.
                var textureInfo = LoadTextureInfo(texturePath);
                texture = textureInfo != null ? textureInfo.ToTexture2D() : new Texture2D(1, 1);
                //if (method == 1) TextureUtils.FlipTexture2DVertically(texture);
                _cachedTextures[texturePath] = texture;
            }
            return texture;
        }

        public void PreloadTextureFileAsync(string texturePath)
        {
            // If the texture has already been created we don't have to load the file again.
            if (_cachedTextures.ContainsKey(texturePath)) return;
            // Start loading the texture file asynchronously if we haven't already started.
            if (!_textureFilePreloadTasks.TryGetValue(texturePath, out Task<Texture2DInfo> textureFileLoadingTask))
            {
                textureFileLoadingTask = _asset.LoadTextureInfoAsync(texturePath);
                _textureFilePreloadTasks[texturePath] = textureFileLoadingTask;
            }
        }

        private Texture2DInfo LoadTextureInfo(string texturePath)
        {
            Debug.Assert(!_cachedTextures.ContainsKey(texturePath));
            PreloadTextureFileAsync(texturePath);
            var textureInfo = _textureFilePreloadTasks[texturePath].Result;
            _textureFilePreloadTasks.Remove(texturePath);
            return textureInfo;
        }
    }
}