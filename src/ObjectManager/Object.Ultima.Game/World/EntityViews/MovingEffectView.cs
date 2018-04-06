using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.IO;
using OA.Ultima.Resources;
using OA.Ultima.World.Entities.Effects;
using OA.Ultima.World.Input;
using OA.Ultima.World.Maps;
using UnityEngine;

namespace OA.Ultima.World.EntityViews
{
    public class MovingEffectView : AEntityView
    {
        MovingEffect Effect => (MovingEffect)Entity;

        EffectData _animData;
        readonly bool _animated;
        int _displayItemID = -1;

        public MovingEffectView(MovingEffect effect)
            : base(effect)
        {
            _animated = TileData.ItemData[Effect.ItemID & FileManager.ItemIDMask].IsAnimation;
            if (_animated)
            {
                _animData = Provider.GetResource<EffectData>(Effect.ItemID);
                _animated = _animData.FrameCount > 0;
            }
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
                DrawArea = new RectInt(DrawTexture.width / 2 - IsometricRenderer.TILE_SIZE_INTEGER_HALF, DrawTexture.height - IsometricRenderer.TILE_SIZE_INTEGER, DrawTexture.width, DrawTexture.height);
                PickType = PickType.PickNothing;
                DrawFlip = false;
            }
            DrawArea.x = 0 - (int)((Entity.Position.X_offset - Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF);
            DrawArea.y = 0 + (int)((Entity.Position.Z_offset + Entity.Z) * 4) - (int)((Entity.Position.X_offset + Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF);
            Rotation = Effect.AngleToTarget;
            HueVector = Utility.GetHueVector(Entity.Hue);
            return base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }
    }
}
