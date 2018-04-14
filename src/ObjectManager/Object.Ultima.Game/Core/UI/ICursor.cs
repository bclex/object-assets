using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.Core.UI
{
    interface ICursor
    {
        void Update();
        void Draw(SpriteBatchUI spriteBatch, Vector2Int mousePosition);
    }
}
