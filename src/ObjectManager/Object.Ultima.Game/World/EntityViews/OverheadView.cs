using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Input;
using OA.Ultima.World.Maps;
using UnityEngine;

namespace OA.Ultima.World.EntityViews
{
    class OverheadView : AEntityView
    {
        new Overhead Entity => (Overhead)base.Entity;
        RenderedText _text;

        public OverheadView(Overhead entity)
            : base(entity)
        {
            _text = new RenderedText(Entity.Text, collapseContent: true);
            DrawTexture = _text.Texture;
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            HueVector = Utility.GetHueVector(Entity.Hue, false, false, true);
            return base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }
    }
}
