using System.Collections.Generic;

namespace OA.Ultima.Data
{
    public class ContextMenuData
    {
        readonly List<ContextMenuItem> _entries = new List<ContextMenuItem>();
        readonly Serial _serial;

        public ContextMenuData(Serial serial)
        {
            _serial = serial;
        }

        public Serial Serial => _serial;

        public int Count => _entries.Count;

        public ContextMenuItem this[int index]
        {
            get
            {
                if (index < 0 || index >= _entries.Count)
                    return null;
                return _entries[index];
            }
        }

        // Add a new context menu entry.
        internal void AddItem(int responseCode, int stringID, int flags, int hue)
        {
            _entries.Add(new ContextMenuItem(responseCode, stringID, flags, hue));
        }
    }
}