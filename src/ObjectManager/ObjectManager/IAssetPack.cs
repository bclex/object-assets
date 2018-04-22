using System;
using UnityEngine;

namespace OA
{
    public interface IAssetPack : IDisposable
    {
        Texture2D LoadTexture(string texturePath, bool flipVertically = false);
        GameObject CreateObject(string path);
    }
}