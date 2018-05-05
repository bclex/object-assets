using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Input;
using OA.Ultima.World.Maps;
using OA.Ultima.World.WorldViews;
using UnityEngine;

namespace OA.Ultima.World.EntityViews
{
    class ItemView : AEntityView
    {
        new Item Entity
        {
            get { return (Item)base.Entity; }
        }

        int _displayItemID = -1;

        public ItemView(Item item)
            : base(item)
        {
            if (Entity.ItemData.IsWet)
            {
                SortZ += 1;
            }
        }

        protected override void Pick(MouseOverList mouseOver, VertexPositionNormalTextureHue[] vertexBuffer)
        {
            var x = mouseOver.MousePosition.X - (int)vertexBuffer[0].Position.X;
            var y = mouseOver.MousePosition.Y - (int)vertexBuffer[0].Position.Y;
            if (Provider.IsPointInItemTexture(_displayItemID, x, y, 1))
                mouseOver.AddItem(Entity, vertexBuffer[0].Position);
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (Entity.NoDraw)
                return false;
            // Update Display texture, if necessary.
            if (Entity.DisplayItemID != _displayItemID)
            {
                _displayItemID = Entity.DisplayItemID;
                DrawTexture = Provider.GetItemTexture(_displayItemID);
                if (DrawTexture == null) // ' no draw ' item.
                    return false;
                DrawArea = new RectInt(DrawTexture.Width / 2 - IsometricRenderer.TILE_SIZE_INTEGER_HALF, DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER + (Entity.Z * 4), DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickObjects;
                DrawFlip = false;
            }
            if (DrawTexture == null) // ' no draw ' item.
                return false;
            DrawArea.y = DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER + (Entity.Z * 4);
            HueVector = Utility.GetHueVector(Entity.Hue, Entity.ItemData.IsPartialHue, false, false);
            if (Entity.Amount > 1 && Entity.ItemData.IsGeneric && Entity.DisplayItemID == Entity.ItemID)
            {
                var offset = Entity.ItemData.Unknown4;
                var offsetDrawPosition = new Vector3(drawPosition.x - 5, drawPosition.y - 5, 0);
                base.Draw(spriteBatch, offsetDrawPosition, mouseOver, map, roofHideFlag);
            }
            var drawn = base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
            DrawOverheads(spriteBatch, drawPosition, mouseOver, map, DrawArea.y - IsometricRenderer.TILE_SIZE_INTEGER_HALF);
            return drawn;
        }
    }
}
