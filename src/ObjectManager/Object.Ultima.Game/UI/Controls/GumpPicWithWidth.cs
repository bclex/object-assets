using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class GumpPicWithWidth : AGumpPic
    {
        float _percentWidthDrawn = 1.0f;

        /// <summary>
        /// The percent of this gump pic's width which is drawn. Clipped to 0.0f to 1.0f.
        /// </summary>
        public float PercentWidthDrawn
        {
            get { return _percentWidthDrawn; }
            set
            {
                if (value < 0f) value = 0f;
                else if (value > 1f) value = 1f;
                _percentWidthDrawn = value;
            }
        }

        public GumpPicWithWidth(AControl parent, int x, int y, int gumpID, int hue, float percentWidth)
            : base(parent)
        {
            BuildGumpling(x, y, gumpID, hue);
            PercentWidthDrawn = percentWidth;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            var hueVector = Utility.GetHueVector(Hue);
            var width = (int)(_percentWidthDrawn * Width);
            spriteBatch.Draw2D(_texture, new RectInt(position.x, position.y, width, Height), new RectInt(0, 0, width, Height), hueVector);
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
