using OA.Core;
using OA.Core.UI;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    abstract class AGumpPic : AControl
    {
        protected Texture2DInfo _texture;
        int _lastFrameGumpID = -1;

        internal int GumpID { get; set; }

        internal int Hue { get; set; }

        internal bool IsPaperdoll { get; set; }

        public AGumpPic(AControl parent)
            : base(parent)
        {
            MakeThisADragger();
        }

        protected void BuildGumpling(int x, int y, int gumpID, int hue)
        {
            Position = new Vector2Int(x, y);
            GumpID = gumpID;
            Hue = hue;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_texture == null || GumpID != _lastFrameGumpID)
            {
                _lastFrameGumpID = GumpID;
                var provider = Service.Get<IResourceProvider>();
                _texture = provider.GetUITexture(GumpID);
                Size = new Vector2Int(_texture.width, _texture.height);
            }
            base.Update(totalMS, frameMS);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            var provider = Service.Get<IResourceProvider>();
            return provider.IsPointInUITexture(GumpID, x, y);
        }
    }
}