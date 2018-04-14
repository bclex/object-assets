using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class GumpPicTiled : AControl
    {
        Texture2D _bgGump;
        int _gumpID;

        GumpPicTiled(AControl parent)
            : base(parent)
        {
        }

        public GumpPicTiled(AControl parent, string[] arguements)
            : this(parent)
        {
            var x = int.Parse(arguements[1]);
            var y = int.Parse(arguements[2]);
            var width = int.Parse(arguements[3]);
            var height = int.Parse(arguements[4]);
            var gumpID = int.Parse(arguements[5]);
            BuildGumpling(x, y, width, height, gumpID);
        }

        public GumpPicTiled(AControl parent, int x, int y, int width, int height, int gumpID)
            : this(parent)
        {
            BuildGumpling(x, y, width, height, gumpID);
        }

        void BuildGumpling(int x, int y, int width, int height, int gumpID)
        {
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);
            _gumpID = gumpID;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_bgGump == null)
            {
                var provider = Service.Get<IResourceProvider>();
                _bgGump = provider.GetUITexture(_gumpID);
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            spriteBatch.Draw2DTiled(_bgGump, new RectInt(position.x, position.y, Width, Height), Vector3.zero);
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
