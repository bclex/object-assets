using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using OA.Ultima.World.Entities.Items;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class ItemGumplingPaperdoll : ItemGumpling
    {
        public int SlotIndex;
        public bool IsFemale;

        int _hueOverride;
        int _gumpIndex;

        public ItemGumplingPaperdoll(AControl parent, int x, int y, Item item)
            : base(parent, item)
        {
            Position = new Vector2Int(x, y);
            HighlightOnMouseOver = false;
        }

        protected override Vector2Int InternalGetPickupOffset(Vector2Int offset)
        {
            return offset;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            if (_texture == null)
            {
                var provider = Service.Get<IResourceProvider>();
                _gumpIndex = Item.ItemData.AnimID + (IsFemale ? 60000 : 50000);
                if (GumpDefTranslator.ItemHasGumpTranslation(_gumpIndex, out int indexTranslated, out int hueTranslated))
                {
                    _gumpIndex = indexTranslated;
                    _hueOverride = hueTranslated;
                }
                _texture = provider.GetUITexture(_gumpIndex);
                Size = new Vector2Int(_texture.width, _texture.height);
            }
            var hue = Item.Hue == 0 & _hueOverride != 0 ? _hueOverride : Item.Hue;
            spriteBatch.Draw2D(_texture, new Vector3(position.x, position.y, 0), Utility.GetHueVector(hue));
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            var provider = Service.Get<IResourceProvider>();
            return provider.IsPointInUITexture(_gumpIndex, x, y);
        }
    }
}
