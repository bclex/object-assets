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
            _allocatedVertices[1] = new VertexPositionNormalTextureHue(new Vector3(position.x + texture.width, position.y, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0));
            _allocatedVertices[2] = new VertexPositionNormalTextureHue(new Vector3(position.x, position.y + texture.height, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0));
            _allocatedVertices[3] = new VertexPositionNormalTextureHue(new Vector3(position.x + texture.width, position.y + texture.height, 0), new Vector3(0, 0, 1), new Vector3(1, 1, 0));
            _allocatedVertices[0].Hue = _allocatedVertices[1].Hue = _allocatedVertices[2].Hue = _allocatedVertices[3].Hue = hue;
            return DrawSprite(texture, _allocatedVertices);
        }

        public bool Draw2D(Texture2DInfo texture, Vector3 position, RectInt sourceRect, Vector3 hue)
        {
            float minX = sourceRect.x / (float)texture.width;
            float maxX = (sourceRect.x + sourceRect.width) / (float)texture.width;
            float minY = sourceRect.y / (float)texture.height;
            float maxY = (sourceRect.y + sourceRect.height) / (float)texture.height;
            _allocatedVertices[0] = new VertexPositionNormalTextureHue(new Vector3(position.x, position.y, 0), new Vector3(0, 0, 1), new Vector3(minX, minY, 0));
            _allocatedVertices[1] = new VertexPositionNormalTextureHue(new Vector3(position.x + sourceRect.width, position.y, 0), new Vector3(0, 0, 1), new Vector3(maxX, minY, 0));
            _allocatedVertices[2] = new VertexPositionNormalTextureHue(new Vector3(position.x, position.y + sourceRect.height, 0), new Vector3(0, 0, 1), new Vector3(minX, maxY, 0));
            _allocatedVertices[3] = new VertexPositionNormalTextureHue(new Vector3(position.x + sourceRect.width, position.y + sourceRect.height, 0), new Vector3(0, 0, 1), new Vector3(maxX, maxY, 0));
            _allocatedVertices[0].Hue = _allocatedVertices[1].Hue = _allocatedVertices[2].Hue = _allocatedVertices[3].Hue = hue;
            return DrawSprite(texture, _allocatedVertices);
        }

        public bool Draw2D(Texture2DInfo texture, RectInt destRect, RectInt sourceRect, Vector3 hue)
        {
            float minX = sourceRect.x / (float)texture.width, maxX = (sourceRect.x + sourceRect.width) / (float)texture.width;
            float minY = sourceRect.y / (float)texture.height, maxY = (sourceRect.y + sourceRect.height) / (float)texture.height;
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
                if (h < texture.height) rect = new RectInt(0, 0, texture.width, h);
                else rect = new RectInt(0, 0, texture.width, texture.height);
                while (w > 0)
                {
                    if (w < texture.width)
                        rect.width = w;
                    Draw2D(texture, new Vector3(x, y, 0), rect, hue);
                    w -= texture.width;
                    x += texture.width;
                }
                h -= texture.height;
                y += texture.height;
            }
            return true;
        }
    }
}
