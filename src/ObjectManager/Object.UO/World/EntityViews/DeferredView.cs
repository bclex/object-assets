using OA.Ultima.Core.Graphics;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Mobiles;
using OA.Ultima.World.Input;
using OA.Ultima.World.Maps;
using UnityEngine;

namespace OA.Ultima.World.EntityViews
{
    public class DeferredView : AEntityView
    {
        readonly Vector3 _drawPosition;
        readonly AEntityView _baseView;

        public DeferredView(DeferredEntity entity, Vector3 drawPosition, AEntityView baseView)
            : base(entity)
        {
            _drawPosition = drawPosition;
            _baseView = baseView;
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (_baseView.Entity is Mobile)
            {
                var mobile = _baseView.Entity as Mobile;
                if (!mobile.IsAlive || mobile.IsDisposed || mobile.Body == 0)
                {
                    Entity.Dispose();
                    return false;
                }
            }
            /*_baseView.SetYClipLine(_drawPosition.Y - 22 -
                ((_baseView.Entity.Position.Z + _baseView.Entity.Position.Z_offset) * 4) +
                ((_baseView.Entity.Position.X_offset + _baseView.Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF));*/
            var success = _baseView.DrawInternal(spriteBatch, _drawPosition, mouseOver, map, roofHideFlag);
            /*_baseView.ClearYClipLine();*/
            return success;
        }
    }
}
