using OA.Core.Diagnostics;
using OA.Ultima.Core.IO;
using OA.Ultima.IO;
using System.Linq;
using UnityEngine;

namespace OA.Ultima.Resources
{
    public class MultiData
    {
        static MultiComponentList[] _components = new MultiComponentList[0x4000];
        public static MultiComponentList[] Cache { get { return _components; } }

        static readonly AFileIndex _fileIndex = FileManager.CreateFileIndex("Multi.idx", "Multi.mul", 0x4000, 14);
        public static AFileIndex FileIndex { get { return _fileIndex; } }

        public static MultiComponentList GetComponents(int index)
        {
            MultiComponentList mcl;
            index &= FileManager.ItemIDMask;
            if (index >= 0 && index < _components.Length)
            {
                mcl = _components[index];
                if (mcl == null)
                    _components[index] = mcl = Load(index);
            }
            else mcl = MultiComponentList.Empty;
            return mcl;
        }

        public static MultiComponentList Load(int index)
        {
            try
            {
                var r = _fileIndex.Seek(index, out int length, out int extra, out bool patched);
                if (r == null)
                    return MultiComponentList.Empty;
                return new MultiComponentList(r, length / 12);
            }
            catch { return MultiComponentList.Empty; }
        }
    }

    public class MultiComponentList
    {
        Vector2Int _min, _max, _center;
        int _width, _height;

        public static readonly MultiComponentList Empty = new MultiComponentList();

        public Vector2Int Min { get { return _min; } }
        public Vector2Int Max { get { return _max; } }
        public Vector2Int Center { get { return _center; } }
        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public MultiItem[] Items { get; private set; }

        public struct MultiItem
        {
            public short ItemID;
            public short OffsetX, OffsetY, OffsetZ;
            public int Flags;

            public override string ToString()
            {
                return string.Format("{0:X4} {1} {2} {3} {4:X4}", ItemID, OffsetX, OffsetY, OffsetZ, Flags);
            }
        }

        public MultiComponentList(BinaryFileReader r, int count)
        {
            var metrics_dataread_start = (int)r.Position;
            _min = _max = Vector2Int.zero;
            Items = new MultiItem[count];
            for (var i = 0; i < count; ++i)
            {
                Items[i].ItemID = r.ReadShort();
                Items[i].OffsetX = r.ReadShort();
                Items[i].OffsetY = r.ReadShort();
                Items[i].OffsetZ = r.ReadShort();
                Items[i].Flags = r.ReadInt();
                if (Items[i].OffsetX < _min.x)
                    _min.x = Items[i].OffsetX;
                if (Items[i].OffsetY < _min.y)
                    _min.y = Items[i].OffsetY;
                if (Items[i].OffsetX > _max.x)
                    _max.x = Items[i].OffsetX;
                if (Items[i].OffsetY > _max.y)
                    _max.y = Items[i].OffsetY;
            }
            _center = new Vector2Int(-_min.x, -_min.y);
            _width = (_max.x - _min.x) + 1;
            _height = (_max.y - _min.y) + 1;
            // SortMultiComponentList();
            Metrics.ReportDataRead((int)r.Position - metrics_dataread_start);
        }

        private void SortMultiComponentList()
        {
            Items = Items.OrderBy(a => a.OffsetY).ThenBy(a => a.OffsetX).ToArray();
        }

        private MultiComponentList()
        {
            Items = new MultiItem[0];
        }
    }
}