using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    public class ResizePic : AControl
    {
        readonly Texture2DInfo[] _gumps;
        int GumpID;

        ResizePic(AControl parent)
            : base(parent)
        {
            _gumps = new Texture2DInfo[9];
            MakeThisADragger();
        }

        public ResizePic(AControl parent, string[] arguements)
            : this(parent)
        {
            var x = int.Parse(arguements[1]);
            var y = int.Parse(arguements[2]);
            var gumpID = int.Parse(arguements[3]);
            var width = int.Parse(arguements[4]);
            var height = int.Parse(arguements[5]);
            BuildGumpling(x, y, gumpID, width, height);
        }

        public ResizePic(AControl parent, int x, int y, int gumpID, int width, int height)
            : this(parent)
        {
            BuildGumpling(x, y, gumpID, width, height);
        }

        public ResizePic(AControl parent, AControl createBackgroundAroundThisControl)
            : this(parent)
        {
            BuildGumpling(createBackgroundAroundThisControl.X - 4,
                createBackgroundAroundThisControl.Y - 4,
                9350,
                createBackgroundAroundThisControl.Width + 8,
                createBackgroundAroundThisControl.Height + 8);
            Page = createBackgroundAroundThisControl.Page;
        }

        void BuildGumpling(int x, int y, int gumpID, int width, int height)
        {
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);
            GumpID = gumpID;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_gumps[0] == null)
            {
                var provider = Service.Get<IResourceProvider>();
                for (var i = 0; i < 9; i++)
                    _gumps[i] = provider.GetUITexture(GumpID + i);
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            var centerWidth = Width - _gumps[0].width - _gumps[2].width;
            var centerHeight = Height - _gumps[0].height - _gumps[6].height;
            var line2Y = position.y + _gumps[0].height;
            var line3Y = position.y + Height - _gumps[6].height;
            // top row
            spriteBatch.Draw2D(_gumps[0], new Vector3(position.x, position.y, 0), Vector3.zero);
            spriteBatch.Draw2DTiled(_gumps[1], new RectInt(position.x + _gumps[0].width, position.y, centerWidth, _gumps[0].height), Vector3.zero);
            spriteBatch.Draw2D(_gumps[2], new Vector3(position.x + Width - _gumps[2].width, position.y, 0), Vector3.zero);
            // middle
            spriteBatch.Draw2DTiled(_gumps[3], new RectInt(position.x, line2Y, _gumps[3].width, centerHeight), Vector3.zero);
            spriteBatch.Draw2DTiled(_gumps[4], new RectInt(position.x + _gumps[3].width, line2Y, centerWidth, centerHeight), Vector3.zero);
            spriteBatch.Draw2DTiled(_gumps[5], new RectInt(position.x + Width - _gumps[5].width, line2Y, _gumps[5].width, centerHeight), Vector3.zero);
            // bottom
            spriteBatch.Draw2D(_gumps[6], new Vector3(position.x, line3Y, 0), Vector3.zero);
            spriteBatch.Draw2DTiled(_gumps[7], new RectInt(position.x + _gumps[6].width, line3Y, centerWidth, _gumps[6].height), Vector3.zero);
            spriteBatch.Draw2D(_gumps[8], new Vector3(position.x + Width - _gumps[8].width, line3Y, 0), Vector3.zero);

            base.Draw(spriteBatch, position, frameMS);
        }
    }
}