using OA.Ultima.World.Maps;
using System.Collections.Generic;

namespace OA.Ultima.World.Entities.Items.Containers
{
    public class ContainerItem : Item
    {
        List<Item> _contents;
        bool _contentsUpdated;

        public List<Item> Contents
        {
            get
            {
                if (_contents == null)
                    _contents = new List<Item>();
                return _contents;
            }
        }

        public ContainerItem(Serial serial, Map map)
            : base(serial, map)
        {
            _contentsUpdated = true;
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            if (_contentsUpdated)
            {
                _onUpdated?.Invoke(this);
                _contentsUpdated = false;
            }
        }

        public override void Dispose()
        {
            for (var i = 0; i < Contents.Count; i++)
                Contents[i].Dispose();
            base.Dispose();
        }

        public void AddItem(Item item)
        {
            if (!Contents.Contains(item))
            {
                Contents.Add(item);
                item.Parent = this;
            }
            _contentsUpdated = true;
        }

        public virtual void RemoveItem(Serial serial)
        {
            foreach (var item in Contents)
                if (item.Serial == serial)
                {
                    item.SaveLastParent();
                    Contents.Remove(item);
                    break;
                }
            _contentsUpdated = true;
        }
    }
}
