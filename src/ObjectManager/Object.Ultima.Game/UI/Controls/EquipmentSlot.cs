using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Resources;
using OA.Ultima.World;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Entities.Mobiles;
using System;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class EquipmentSlot : AControl
    {
        readonly WorldModel _world;

        Mobile _entity;
        EquipLayer _equipLayer;
        Item _item;
        StaticPic _itemGraphic;

        bool _clickedCanDrag;
        float _pickUpTime;
        Vector2Int _clickPoint;
        bool _sendClickIfNoDoubleClick;
        float _singleClickTime;

        public EquipmentSlot(AControl parent, int x, int y, Mobile entity, EquipLayer layer)
            : base(parent)
        {
            HandlesMouseInput = true;

            _entity = entity;
            _equipLayer = layer;

            Position = new Vector2Int(x, y);
            AddControl(new GumpPicTiled(this, 0, 0, 19, 20, 0x243A));
            AddControl(new GumpPic(this, 0, 0, 0x2344, 0));

            _world = Service.Get<WorldModel>();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_item != null && _item.IsDisposed)
            {
                _item = null;
                _itemGraphic.Dispose();
                _itemGraphic = null;
            }

            if (_item != _entity.Equipment[(int)_equipLayer])
            {
                if (_itemGraphic != null)
                {
                    _itemGraphic.Dispose();
                    _itemGraphic = null;
                }
                _item = _entity.Equipment[(int)_equipLayer];
                if (_item != null)
                    _itemGraphic = (StaticPic)AddControl(new StaticPic(this, 0, 0, _item.ItemID, _item.Hue));
            }

            if (_item != null)
            {
                if (_clickedCanDrag && totalMS >= _pickUpTime)
                {
                    _clickedCanDrag = false;
                    AttemptPickUp();
                }
                if (_sendClickIfNoDoubleClick && totalMS >= _singleClickTime)
                {
                    _sendClickIfNoDoubleClick = false;
                    _world.Interaction.SingleClick(_item);
                }
            }

            base.Update(totalMS, frameMS);

            if (_itemGraphic != null)
                _itemGraphic.Position = new Vector2Int(0 - 14, 0);
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            if (_item == null)
                return;

            // if click, we wait for a moment before picking it up. This allows a single click.
            _clickedCanDrag = true;
            float totalMS = (float)Service.Get<UltimaGame>().TotalMS;
            _pickUpTime = totalMS + UltimaGameSettings.UserInterface.Mouse.ClickAndPickupMS;
            _clickPoint = new Vector2Int(x, y);
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (_item == null)
                return;
            // if we have not yet picked up the item, AND we've moved more than 3 pixels total away from the original item, pick it up!
            if (_clickedCanDrag && (Math.Abs(_clickPoint.x - x) + Math.Abs(_clickPoint.y - y) > 3))
            {
                _clickedCanDrag = false;
                AttemptPickUp();
            }
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (_item == null)
                return;
            if (_clickedCanDrag)
            {
                _clickedCanDrag = false;
                _sendClickIfNoDoubleClick = true;
                float totalMS = (float)Service.Get<UltimaGame>().TotalMS;
                _singleClickTime = totalMS + UltimaGameSettings.UserInterface.Mouse.DoubleClickMS;
            }
        }

        protected override void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
            if (_item == null)
                return;

            _world.Interaction.DoubleClick(_item);
            _sendClickIfNoDoubleClick = false;
        }

        private void AttemptPickUp()
        {
            var provider = Service.Get<IResourceProvider>();
            provider.GetItemDimensions(_item.DisplayItemID, out int w, out int h);
            var clickPoint = new Vector2Int(w / 2, h / 2);
            _world.Interaction.PickupItem(_item, clickPoint);
        }
    }
}
