using OA.Core.UI;
using OA.Ultima.Resources;
using OA.Ultima.UI.Controls;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items.Containers;
using System.Collections.Generic;

namespace OA.Ultima.UI.WorldGumps
{
    class ContainerGump : Gump
    {
        ContainerData _data;
        ContainerItem _item;

        public ContainerGump(AEntity containerItem, int gumpID)
            : base(containerItem.Serial, 0)
        {
            _data = ContainerData.Get(gumpID);
            _item = (ContainerItem)containerItem;
            _item.SetCallbacks(OnItemUpdated, OnItemDisposed);
            IsMoveable = true;
            AddControl(new GumpPicContainer(this, 0, 0, _data.GumpID, 0, _item));
        }

        public override void Dispose()
        {
            _item.ClearCallBacks(OnItemUpdated, OnItemDisposed);
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        void OnItemUpdated(AEntity entity)
        {
            // delete any items in our pack that are no longer in the container.
            var ControlsToRemove = new List<AControl>();
            foreach (var c in Children)
                if (c is ItemGumpling && !_item.Contents.Contains(((ItemGumpling)c).Item))
                    ControlsToRemove.Add(c);
            foreach (var c in ControlsToRemove)
                Children.Remove(c);
            // add any items in the container that are not in our pack.
            foreach (var item in _item.Contents)
            {
                var controlForThisItem = false;
                foreach (var c in Children)
                    if (c is ItemGumpling && ((ItemGumpling)c).Item == item)
                    {
                        controlForThisItem = true;
                        break;
                    }
                if (!controlForThisItem)
                    AddControl(new ItemGumpling(this, item));
            }
        }

        void OnItemDisposed(AEntity entity)
        {
            Dispose();
        }
    }
}
