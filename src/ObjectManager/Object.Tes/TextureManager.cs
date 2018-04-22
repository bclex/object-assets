﻿using OA.Core;
using OA.Tes.FilePacks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace OA.Tes
{
    public class TextureManager
    {
        readonly BsaMultiFile r;
        readonly Dictionary<string, Task<Texture2DInfo>> textureFilePreloadTasks = new Dictionary<string, Task<Texture2DInfo>>();
        readonly Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();

        public TextureManager(BsaMultiFile r)
        {
            this.r = r;
        }

        public Texture2D LoadTexture(string texturePath, bool flipVertically = false)
        {
            if (!cachedTextures.TryGetValue(texturePath, out Texture2D texture))
            {
                // Load & cache the texture.
                var textureInfo = LoadTextureInfo(texturePath);
                texture = textureInfo != null ? textureInfo.ToTexture2D() : new Texture2D(1, 1);
                if (flipVertically) { TextureUtils.FlipTexture2DVertically(texture); }
                cachedTextures[texturePath] = texture;
            }
            return texture;
        }

        public void PreloadTextureFileAsync(string texturePath)
        {
            // If the texture has already been created we don't have to load the file again.
            if (cachedTextures.ContainsKey(texturePath)) return;
            // Start loading the texture file asynchronously if we haven't already started.
            if (!textureFilePreloadTasks.TryGetValue(texturePath, out Task<Texture2DInfo> textureFileLoadingTask))
            {
                textureFileLoadingTask = r.LoadTextureAsync(texturePath);
                textureFilePreloadTasks[texturePath] = textureFileLoadingTask;
            }
        }

        private Texture2DInfo LoadTextureInfo(string texturePath)
        {
            Debug.Assert(!cachedTextures.ContainsKey(texturePath));
            PreloadTextureFileAsync(texturePath);
            var textureInfo = textureFilePreloadTasks[texturePath].Result;
            textureFilePreloadTasks.Remove(texturePath);
            return textureInfo;
        }
    }
}