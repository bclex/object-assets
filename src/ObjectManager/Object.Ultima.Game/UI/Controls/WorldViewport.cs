using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.World;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    /// <summary>
    /// A control that shows the current isometric view around the player.
    /// </summary>
    class WorldViewport : AControl
    {
        WorldModel _model;

        public Vector2Int MousePosition;

        Vector2 _inputMultiplier = Vector2.one;

        public WorldViewport(AControl parent, int x, int y, int width, int height)
            : base(parent)
        {
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);
            HandlesMouseInput = true;
            Service.Add<WorldViewport>(this);
        }

        protected override void OnInitialize()
        {
            _model = Service.Get<WorldModel>();
        }

        public override void Dispose()
        {
            Service.Remove<WorldViewport>();
            base.Dispose();
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            var worldTexture = (_model.GetView() as WorldView).Isometric.Texture;
            if (worldTexture == null)
                return;
            _inputMultiplier = new Vector2((float)worldTexture.Width / Width, (float)worldTexture.Height / Height);
            spriteBatch.Draw2D(worldTexture, new RectInt(position.x, position.y, Width, Height), Vector3.zero);
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override void OnMouseOver(int x, int y)
        {
            MousePosition = new Vector2Int((int)(x * _inputMultiplier.x), (int)(y * _inputMultiplier.y));
        }
    }
}
