using OA.Core;
using OA.Ultima.Core.UI;
using OA.Ultima.Resources;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima
{
    class ResourceProvider : IResourceProvider
    {
        AnimationResource _anim;
        ArtMulResource _art;
        ClilocResource _cliloc;
        EffectDataResource _effects;
        FontsResource _fonts;
        GumpMulResource _gumps;
        TexmapResource _texmaps;
        readonly Dictionary<Type, object> _resources = new Dictionary<Type, object>();

        public ResourceProvider(object game)
        {
            _anim = new AnimationResource(game);
            _art = new ArtMulResource(game);
            _cliloc = new ClilocResource("enu");
            _effects = new EffectDataResource();
            _fonts = new FontsResource(game);
            _gumps = new GumpMulResource(game);
            _texmaps = new TexmapResource(game);
        }

        public AAnimationFrame[] GetAnimation(int body, ref int hue, int action, int direction)
        {
            return _anim.GetAnimation(body, ref hue, action, direction);
        }

        public Texture2DInfo GetUITexture(int textureID, bool replaceMask080808 = false)
        {
            return _gumps.GetGumpTexture(textureID, replaceMask080808);
        }

        public bool IsPointInUITexture(int textureID, int x, int y)
        {
            return _gumps.IsPointInGumpTexture(textureID, x, y);
        }

        public Texture2DInfo GetItemTexture(int itemIndex)
        {
            return _art.GetStaticTexture(itemIndex);
        }

        public bool IsPointInItemTexture(int textureID, int x, int y, int extraRange = 0)
        {
            return _art.IsPointInItemTexture(textureID, x, y, extraRange);
        }

        public Texture2DInfo GetLandTexture(int landIndex)
        {
            return _art.GetLandTexture(landIndex);
        }

        public void GetItemDimensions(int itemIndex, out int width, out int height)
        {
            _art.GetStaticDimensions(itemIndex, out width, out height);
        }

        public Texture2DInfo GetTexmapTexture(int textureIndex)
        {
            return _texmaps.GetTexmapTexture(textureIndex);
        }

        /// <summary>
        /// Returns a Ultima Online Hue index that approximates the passed color.
        /// </summary>
        public ushort GetWebSafeHue(Color32 color)
        {
            return (ushort)HueData.GetWebSafeHue(color);
        }

        public IFont GetUnicodeFont(int fontIndex)
        {
            return _fonts.GetUniFont(fontIndex);
        }

        public IFont GetAsciiFont(int fontIndex)
        {
            return _fonts.GetAsciiFont(fontIndex);
        }

        public string GetString(int clilocIndex)
        {
            return _cliloc.GetString(clilocIndex);
        }

        public void RegisterResource<T>(IResource<T> resource)
        {
            var type = typeof(T);
            if (_resources.ContainsKey(type))
            {
                Utils.Error($"Attempted to register resource provider of type {type} twice.");
                _resources.Remove(type);
            }
            _resources.Add(type, resource);
        }

        public T GetResource<T>(int resourceIndex)
        {
            var type = typeof(T);
            if (_resources.ContainsKey(type))
            {
                var resource = (IResource<T>)_resources[type];
                return (T)resource.GetResource(resourceIndex);
            }
            else
            {
                Utils.Error($"Attempted to get resource provider of type {type}, but no provider with this type is registered.");
                return default(T);
            }
        }
    }
}
