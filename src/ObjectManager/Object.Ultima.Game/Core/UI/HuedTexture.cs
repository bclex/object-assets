using OA.Core;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.Resources
{
    public class HuedTexture
    {
        readonly Texture2DInfo _texture;
        readonly RectInt _sourceRect = default(RectInt);

        Vector2Int _offset;
        public Vector2Int Offset
        {
            set { _offset = value; }
            get { return _offset; }
        }

        int _hue;
        public int Hue
        {
            set { _hue = value; }
            get { return _hue; }
        }

        public HuedTexture(Texture2DInfo texture, Vector2Int offset, RectInt source, int hue)
        {
            _texture = texture;
            _offset = offset;
            _sourceRect = source;
            _hue = hue;
        }

        public void Draw(SpriteBatchUI sb, RectInt position)
        {
            var v = new Vector3(position.x - _offset.x, position.y - _offset.y, 0);
            sb.Draw2D(_texture, v, _sourceRect, Utility.GetHueVector(_hue));
        }
    }
}
