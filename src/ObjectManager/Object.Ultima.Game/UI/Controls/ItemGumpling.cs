using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using OA.Ultima.World;
using OA.Ultima.World.Entities.Items;
using System;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class ItemGumpling : AControl
    {
        public bool CanPickUp = true;
        public bool HighlightOnMouseOver = true;

        protected Texture2DInfo _texture;
        HtmlGumpling _label;

        bool _clickedCanDrag;
        float _pickUpTime;
        Vector2Int _clickPoint;
        bool _sendClickIfNoDoubleClick;
        float _singleClickTime;

        readonly WorldModel _world;

        public Item Item { get; private set; }

        public ItemGumpling(AControl parent, Item item)
            : base(parent)
        {
            BuildGumpling(item);
            HandlesMouseInput = true;
            _world = Service.Get<WorldModel>();
        }

        void BuildGumpling(Item item)
        {
            Position = item.InContainerPosition;
            Item = item;
        }

        public override void Dispose()
        {
            UpdateLabel(true);
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (Item.IsDisposed)
            {
                Dispose();
                return;
            }
            if (_clickedCanDrag && totalMS >= _pickUpTime)
            {
                _clickedCanDrag = false;
                AttemptPickUp();
            }
            if (_sendClickIfNoDoubleClick && totalMS >= _singleClickTime)
            {
                _sendClickIfNoDoubleClick = false;
                _world.Interaction.SingleClick(Item);
            }
            UpdateLabel();
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            if (_texture == null)
            {
                var provider = Service.Get<IResourceProvider>();
                _texture = provider.GetItemTexture(Item.DisplayItemID);
                Size = new Vector2Int(_texture.Width, _texture.Height);
            }
            var hue = Utility.GetHueVector(IsMouseOver && HighlightOnMouseOver ? WorldView.MouseOverHue : Item.Hue);
            if (Item.Amount > 1 && Item.ItemData.IsGeneric && Item.DisplayItemID == Item.ItemID)
            {
                var offset = Item.ItemData.Unknown4;
                spriteBatch.Draw2D(_texture, new Vector3(position.X - 5, position.Y - 5, 0), hue);
            }
            spriteBatch.Draw2D(_texture, new Vector3(position.X, position.Y, 0), hue);
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            // Allow selection if there is a non-transparent pixel below the mouse cursor or at an offset of
            // (-1,0), (0,-1), (1,0), or (1,1). This will allow selection even when the mouse cursor is directly
            // over a transparent pixel, and will also increase the 'selection space' of an item by one pixel in
            // each dimension - thus a very thin object (2-3 pixels wide) will be increased.
            if (IsPointInTexture(x, y))
                return true;
            if (Item.Amount > 1 && Item.ItemData.IsGeneric)
            {
                var offset = Item.ItemData.Unknown4;
                if (IsPointInTexture(x + offset, y + offset))
                    return true;
            }
            return false;
        }

        bool IsPointInTexture(int x, int y)
        {
            var provider = Service.Get<IResourceProvider>();
            return provider.IsPointInItemTexture(Item.DisplayItemID, x, y, 1);
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            // if click, we wait for a moment before picking it up. This allows a single click.
            _clickedCanDrag = true;
            var totalMS = (float)Service.Get<UltimaGame>().TotalMS;
            _pickUpTime = totalMS + UltimaGameSettings.UserInterface.Mouse.ClickAndPickupMS;
            _clickPoint = new Vector2Int(x, y);
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            _clickedCanDrag = false;
        }

        protected override void OnMouseOver(int x, int y)
        {
            // if we have not yet picked up the item, AND we've moved more than 3 pixels total away from the original item, pick it up!
            if (_clickedCanDrag && (Math.Abs(_clickPoint.x - x) + Math.Abs(_clickPoint.y - y) > 3))
            {
                _clickedCanDrag = false;
                AttemptPickUp();
            }
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (_clickedCanDrag)
            {
                _clickedCanDrag = false;
                _sendClickIfNoDoubleClick = true;
                var totalMS = (float)Service.Get<UltimaGame>().TotalMS;
                _singleClickTime = totalMS + UltimaGameSettings.UserInterface.Mouse.DoubleClickMS;
            }
        }

        protected override void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
            _world.Interaction.DoubleClick(Item);
            _sendClickIfNoDoubleClick = false;
        }

        protected virtual Vector2Int InternalGetPickupOffset(Vector2Int offset)
        {
            return offset;
        }

        void AttemptPickUp()
        {
            if (CanPickUp)
            {
                if (this is ItemGumplingPaperdoll)
                {
                    var provider = Service.Get<IResourceProvider>();
                    provider.GetItemDimensions(Item.DisplayItemID, out int w, out int h);
                    var click_point = new Vector2Int(w / 2, h / 2);
                    _world.Interaction.PickupItem(Item, InternalGetPickupOffset(click_point));
                }
                else _world.Interaction.PickupItem(Item, InternalGetPickupOffset(_clickPoint));
            }
        }

        void UpdateLabel(bool isDisposing = false)
        {
            if (!isDisposing && Item.Overheads.Count > 0)
            {
                if (_label == null)
                {
                    var input = Service.Get<IInputService>();
                    UserInterface.AddControl(_label = new HtmlGumpling(null, 0, 0, 200, 32, 0, 0,
                        string.Format("<center><span style='font-family: ascii3;'>{0}</center>", Item.Overheads[0].Text)),
                        input.MousePosition.X - 100, input.MousePosition.Y - 12);
                    _label.MetaData.Layer = UILayer.Over;
                }
            }
            else if (_label != null)
            {
                _label.Dispose();
                _label = null;
            }
        }
    }
}
