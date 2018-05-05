using OA.Core;
using UnityEngine;

namespace OA.Ultima.Core.Graphics
{
    public class SpriteBatchUI : SpriteBatch3D
    {
        public SpriteBatchUI(object game)
            : base(game) { }

        VertexPositionNormalTextureHue[] _allocatedVertices = new VertexPositionNormalTextureHue[4];

        public bool Draw2D(Texture2DInfo texture, Vector3 position, Vector3 hue)
        {
            _allocatedVertices[0] = new VertexPositionNormalTextureHue(new Vector3(position.x, position.y, 0), new Vector3(0, 0, 1), new Vector3(0, 0, 0));
            _allocatedVertices[1] = new VertexPositionNormalTextureHue(new Vector3(position.x + texture.Width, position.y, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0));
            _allocatedVertices[2] = new VertexPositionNormalTextureHue(new Vector3(position.x, position.y + texture.Height, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0));
            _allocatedVertices[3] = new VertexPositionNormalTextureHue(new Vector3(position.x + texture.Width, position.y + texture.Height, 0), new Vector3(0, 0, 1), new Vector3(1, 1, 0));
            _allocatedVertices[0].Hue = _allocatedVertices[1].Hue = _allocatedVertices[2].Hue = _allocatedVertices[3].Hue = hue;
            return DrawSprite(texture, _allocatedVertices);
        }

        public bool Draw2D(Texture2DInfo texture, Vector3 position, RectInt sourceRect, Vector3 hue)
        {
            float minX = sourceRect.x / (float)texture.Width;
            float maxX = (sourceRect.x + sourceRect.width) / (float)texture.Width;
            float minY = sourceRect.y / (float)texture.Height;
            float maxY = (sourceRect.y + sourceRect.height) / (float)texture.Height;
            _allocatedVertices[0] = new VertexPositionNormalTextureHue(new Vector3(position.x, position.y, 0), new Vector3(0, 0, 1), new Vector3(minX, minY, 0));
            _allocatedVertices[1] = new VertexPositionNormalTextureHue(new Vector3(position.x + sourceRect.width, position.y, 0), new Vector3(0, 0, 1), new Vector3(maxX, minY, 0));
            _allocatedVertices[2] = new VertexPositionNormalTextureHue(new Vector3(position.x, position.y + sourceRect.height, 0), new Vector3(0, 0, 1), new Vector3(minX, maxY, 0));
            _allocatedVertices[3] = new VertexPositionNormalTextureHue(new Vector3(position.x + sourceRect.width, position.y + sourceRect.height, 0), new Vector3(0, 0, 1), new Vector3(maxX, maxY, 0));
            _allocatedVertices[0].Hue = _allocatedVertices[1].Hue = _allocatedVertices[2].Hue = _allocatedVertices[3].Hue = hue;
            return DrawSprite(texture, _allocatedVertices);
        }

        public bool Draw2D(Texture2DInfo texture, RectInt destRect, RectInt sourceRect, Vector3 hue)
        {
            float minX = sourceRect.x / (float)texture.Width, maxX = (sourceRect.x + sourceRect.width) / (float)texture.Width;
            float minY = sourceRect.y / (float)texture.Height, maxY = (sourceRect.y + sourceRect.height) / (float)texture.Height;
            _allocatedVertices[0] = new VertexPositionNormalTextureHue(new Vector3(destRect.x, destRect.y, 0), new Vector3(0, 0, 1), new Vector3(minX, minY, 0));
            _allocatedVertices[1] = new VertexPositionNormalTextureHue(new Vector3(destRect.x + destRect.width, destRect.y, 0), new Vector3(0, 0, 1), new Vector3(maxX, minY, 0));
            _allocatedVertices[2] = new VertexPositionNormalTextureHue(new Vector3(destRect.x, destRect.y + destRect.height, 0), new Vector3(0, 0, 1), new Vector3(minX, maxY, 0));
            _allocatedVertices[3] = new VertexPositionNormalTextureHue(new Vector3(destRect.x + destRect.width, destRect.y + destRect.height, 0), new Vector3(0, 0, 1), new Vector3(maxX, maxY, 0));
            _allocatedVertices[0].Hue = _allocatedVertices[1].Hue = _allocatedVertices[2].Hue = _allocatedVertices[3].Hue = hue;
            return DrawSprite(texture, _allocatedVertices);
        }

        public bool Draw2D(Texture2DInfo texture, RectInt destRect, Vector3 hue)
        {
            _allocatedVertices[0] = new VertexPositionNormalTextureHue(new Vector3(destRect.x, destRect.y, 0), new Vector3(0, 0, 1), new Vector3(0, 0, 0));
            _allocatedVertices[1] = new VertexPositionNormalTextureHue(new Vector3(destRect.x + destRect.width, destRect.y, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0));
            _allocatedVertices[2] = new VertexPositionNormalTextureHue(new Vector3(destRect.x, destRect.y + destRect.height, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0));
            _allocatedVertices[3] = new VertexPositionNormalTextureHue(new Vector3(destRect.x + destRect.width, destRect.y + destRect.height, 0), new Vector3(0, 0, 1), new Vector3(1, 1, 0));
            _allocatedVertices[0].Hue = _allocatedVertices[1].Hue = _allocatedVertices[2].Hue = _allocatedVertices[3].Hue = hue;
            return DrawSprite(texture, _allocatedVertices);
        }

        public bool Draw2DTiled(Texture2DInfo texture, RectInt destRect, Vector3 hue)
        {
            var y = destRect.y;
            var h = destRect.height;
            RectInt rect;
            while (h > 0)
            {
                int x = destRect.x;
                int w = destRect.width;
                if (h < texture.Height) rect = new RectInt(0, 0, texture.Width, h);
                else rect = new RectInt(0, 0, texture.Width, texture.Height);
                while (w > 0)
                {
                    if (w < texture.Width)
                        rect.width = w;
                    Draw2D(texture, new Vector3(x, y, 0), rect, hue);
                    w -= texture.Width;
                    x += texture.Width;
                }
                h -= texture.Height;
                y += texture.Height;
            }
            return true;
        }
    }
}
