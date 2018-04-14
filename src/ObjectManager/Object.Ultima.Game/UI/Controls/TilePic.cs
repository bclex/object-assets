using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    /// <summary>
    /// A gump that shows a static item.
    /// </summary>
    class StaticPic : AControl
    {
        Texture2D _texture;
        int Hue;
        int _tileID;

        StaticPic(AControl parent)
            : base(parent)
        {
        }

        public StaticPic(AControl parent, string[] arguements)
            : this(parent)
        {
            var hue = 0;
            var x = int.Parse(arguements[1]);
            var y = int.Parse(arguements[2]);
            var tileID = int.Parse(arguements[3]);
            if (arguements.Length > 4)
                hue = int.Parse(arguements[4]); // has a HUE="XXX" arguement!
            BuildGumpling(x, y, tileID, hue);
        }

        public StaticPic(AControl parent, int x, int y, int itemID, int hue)
            : this(parent)
        {
            BuildGumpling(x, y, itemID, hue);
        }

        void BuildGumpling(int x, int y, int tileID, int hue)
        {
            Position = new Vector2Int(x, y);
            Hue = hue;
            _tileID = tileID;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_texture == null)
            {
                var provider = Service.Get<IResourceProvider>();
                _texture = provider.GetItemTexture(_tileID);
                Size = new Vector2Int(_texture.width, _texture.height);
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            spriteBatch.Draw2D(_texture, new Vector3(position.x, position.y, 0), Utility.GetHueVector(0, false, false, true));
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
