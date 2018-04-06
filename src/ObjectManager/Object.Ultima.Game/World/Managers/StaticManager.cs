using OA.Ultima.World.Entities.Items;
using System.Collections.Generic;

namespace OA.Ultima.World.Managers
{
    public class StaticManager
    {
        private readonly List<StaticItem> _activeStatics = new List<StaticItem>();

        public void AddStaticThatNeedsUpdating(StaticItem item)
        {
            if (item.IsDisposed || item.Overheads.Count == 0)
                return;
            _activeStatics.Add(item);
        }

        public void Update(double frameMS)
        {
            for (var i = 0; i < _activeStatics.Count; i++)
            {
                _activeStatics[i].Update(frameMS);
                if (_activeStatics[i].IsDisposed || _activeStatics[i].Overheads.Count == 0)
                    _activeStatics.RemoveAt(i);
            }
        }
    }
}
