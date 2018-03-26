using OA.Ultima.Data;
using OA.Ultima.Resources;
using OA.Ultima.World.Entities.Items.Containers;
using OA.Ultima.World.EntityViews;
using OA.Ultima.World.Maps;
using UnityEngine;

namespace OA.Ultima.World.Entities.Items
{
    public class Item : AEntity
    {
        public AEntity Parent;

        public override string Name
        {
            get { return ItemData.Name; }
        }

        public override Position3D Position
        {
            get
            {
                if (Parent != null) return Parent.Position;
                else return base.Position;
            }
        }

        public Vector2Int InContainerPosition = Vector2Int.zero;

        public Item(Serial serial, Map map)
            : base(serial, map) { }

        public override void Dispose()
        {
            base.Dispose();
            // if is worn, let the wearer know we are disposing.
            if (Parent is Mobile)
                ((Mobile)Parent).RemoveItem(Serial);
            else if (Parent is ContainerItem)
                ((ContainerItem)Parent).RemoveItem(Serial);
        }

        protected override AEntityView CreateView()
        {
            return new ItemView(this);
        }

         int _amount;
        public int Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public ItemData ItemData;

        private int _itemID;
        private int? _displayItemID;

        public int ItemID
        {
            get { return _itemID; }
            set
            {
                _itemID = value;
                ItemData = TileData.ItemData[_itemID & 0xFFFF]; // TODO: Does this work on both legacy and UOP clients?
            }
        }

        public int DisplayItemID
        {
            get
            {
                if (_displayItemID.HasValue)
                    return _displayItemID.Value;
                if (IsCoin)
                {
                    if (Amount > 5) return _itemID + 2;
                    else if (Amount > 1) return _itemID + 1;
                }
                return _itemID;
            }
            set { _displayItemID = value; }
        }

        public bool NoDraw
        {
            get { return _itemID <= 1 || _displayItemID <= 1; } // no draw
        }

        public bool IsCoin
        {
            get { return _itemID >= 0xEEA && _itemID <= 0xEF2; }
        }

        public int ContainerSlotIndex;

        public override void Update(double frameMS)
        {
            if (WorldView.AllLabels && !(this is StaticItem) && Parent == null && ItemData.Weight != 255)
                AddOverhead(MessageTypes.Label, Name, 3, 0, false);
            base.Update(frameMS);
        }

        public override string ToString()
        {
            return base.ToString() + " | " + ItemData.Name;
        }

        public bool AtWorldPoint(int x, int y)
        {
            if (Position.X == x && Position.Y == y) return true;
            else return false;
        }

        public virtual bool TryPickUp()
        {
            if (ItemData.Weight == 255) return false;
            else return true;
        }

        // ============================================================================================================
        // Last Parent routines 
        // ============================================================================================================

        AEntity _lastParent;
        public bool HasLastParent
        {
            get { return (_lastParent != null); }
        }

        public void SaveLastParent()
        {
            _lastParent = Parent;
        }

        public void RestoreLastParent()
        {
            if (_lastParent != null)
                ((ContainerItem)_lastParent).AddItem(this);
        }
    }
}
