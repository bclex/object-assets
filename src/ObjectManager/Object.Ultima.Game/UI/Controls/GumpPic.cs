using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class GumpPic : AGumpPic
    {
        public GumpPic(AControl parent, string[] arguements)
            : base(parent)
        {
            var hue = 0;
            var x = int.Parse(arguements[1]);
            var y = int.Parse(arguements[2]);
            var gumpID = int.Parse(arguements[3]);
            if (arguements.Length > 4)
            {
                // has a HUE=XXX arguement (and potentially a CLASS=XXX argument).
                var hueArgument = arguements[4].Substring(arguements[4].IndexOf('=') + 1);
                hue = int.Parse(hueArgument);
            }
            BuildGumpling(x, y, gumpID, hue);
        }

        public GumpPic(AControl parent, int x, int y, int gumpID, int hue)
            : base(parent)
        {
            BuildGumpling(x, y, gumpID, hue);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            var hueVector = Utility.GetHueVector(Hue);
            spriteBatch.Draw2D(_texture, new Vector3(position.x, position.y, 0), hueVector);
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}