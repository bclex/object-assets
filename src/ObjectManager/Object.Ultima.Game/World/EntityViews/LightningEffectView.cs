using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.World.Entities.Effects;
using OA.Ultima.World.Input;
using OA.Ultima.World.Maps;
using UnityEngine;

namespace OA.Ultima.World.EntityViews
{
    class LightningEffectView : AEntityView
    {
        LightningEffect Effect => (LightningEffect)Entity;

        int _displayItemID = -1;

        public LightningEffectView(LightningEffect effect)
            : base(effect) { }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            CheckDefer(map, drawPosition);
            return DrawInternal(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }

        public override bool DrawInternal(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            var displayItemdID = 0x4e20 + Effect.FramesActive;
            if (displayItemdID > 0x4e29)
            {
                return false;
            }
            if (displayItemdID != _displayItemID)
            {
                _displayItemID = displayItemdID;
                DrawTexture = Provider.GetUITexture(displayItemdID);
                var offset = _offsets[_displayItemID - 20000];
                DrawArea = new RectInt(offset.x, DrawTexture.Height - 33 + (Entity.Z * 4) + offset.y, DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickNothing;
                DrawFlip = false;
            }
            // Update hue vector.
            HueVector = Utility.GetHueVector(Entity.Hue);
            return base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }

        static readonly Vector2Int[] _offsets = {
                new Vector2Int(48, 0),
                new Vector2Int(68, 0),
                new Vector2Int(92, 0),
                new Vector2Int(72, 0),
                new Vector2Int(48, 0),
                new Vector2Int(56, 0),
                new Vector2Int(76, 0),
                new Vector2Int(76, 0),
                new Vector2Int(92, 0),
                new Vector2Int(80, 0)
            };
    }
}
