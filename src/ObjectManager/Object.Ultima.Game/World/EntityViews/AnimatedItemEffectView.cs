using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using OA.Ultima.World.Entities.Effects;
using OA.Ultima.World.Input;
using OA.Ultima.World.Maps;
using UnityEngine;

namespace OA.Ultima.World.EntityViews
{
    public class AnimatedItemEffectView : AEntityView
    {
        AnimatedItemEffect Effect => (AnimatedItemEffect)Entity;

        EffectData _animData;
        readonly bool _animated;
        int _displayItemID = -1;

        public AnimatedItemEffectView(AnimatedItemEffect effect)
            : base(effect)
        {
            _animated = true;
            _animData = Provider.GetResource<EffectData>(Effect.ItemID);
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            CheckDefer(map, drawPosition);
            return DrawInternal(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }

        public override bool DrawInternal(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            var displayItemdID = _animated ? Effect.ItemID + ((Effect.FramesActive / _animData.FrameInterval) % _animData.FrameCount) : Effect.ItemID;
            if (displayItemdID != _displayItemID)
            {
                _displayItemID = displayItemdID;
                DrawTexture = Provider.GetItemTexture(_displayItemID);
                DrawArea = new RectInt(DrawTexture.Width / 2 - 22, DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER + (Entity.Z * 4), DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickNothing;
                DrawFlip = false;
            }
            // Update hue vector.
            HueVector = Utility.GetHueVector(Entity.Hue);
            return base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }
    }
}
