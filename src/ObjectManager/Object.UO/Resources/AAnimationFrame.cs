using UnityEngine;

namespace OA.Ultima.Resources
{
    public abstract class AAnimationFrame
    {
        public Vector2Int Center { get; protected set; } 

        public Texture2D Texture { get; protected set; }

        public abstract bool IsPointInTexture(int x, int y);
    }
}
