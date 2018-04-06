using OA.Ultima.World.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.World.Input
{
    public class MouseOverList
    {
        List<MouseOverItem> _items;

        public readonly Vector2Int MousePosition;
        public PickType PickType = PickType.PickNothing;

        public MouseOverList(MousePicking mousePicking)
        {
            _items = new List<MouseOverItem>();
            MousePosition = mousePicking.Position;
            PickType = mousePicking.PickOnly;
        }

        public MouseOverItem GetForemostMouseOverItem(Vector2Int mousePosition)
        {
            // Parse list backwards to find topmost mouse over object.
            foreach (var item in CreateReverseIterator(_items))
                return item;
            return null;
        }

        public MouseOverItem GetForemostMouseOverItem<T>(Vector2Int mousePosition) where T : AEntity
        {
            // Parse list backwards to find topmost mouse over object.
            foreach (var item in CreateReverseIterator(_items))
                if (item.Entity.GetType() == typeof(T))
                    return item;
            return null;
        }

        static IEnumerable<MouseOverItem> CreateReverseIterator<MouseOverItem>(IList<MouseOverItem> list)
        {
            var count = list.Count;
            for (var i = count - 1; i >= 0; --i)
                yield return list[i];
        }

        public void AddItem(AEntity e, Vector3 position)
        {
            var inTexturePoint = new Vector2Int(MousePosition.x - (int)position.x, MousePosition.y - (int)position.y);
            _items.Add(new MouseOverItem(inTexturePoint, e));
        }

        public bool IsMouseInObjectIsometric(VertexPositionNormalTextureHue[] v)
        {
            if (v.Length != 4)
                return false;
            float high = -50000, low = 50000;
            for (var i = 0; i < 4; i++)
            {
                if (v[i].Position.Y > high)
                    high = v[i].Position.Y;
                if (v[i].Position.Y < low)
                    low = v[i].Position.Y;
            }
            if (high < MousePosition.y)
                return false;
            if (low > MousePosition.y)
                return false;
            if (v[1].Position.X < MousePosition.x)
                return false;
            if (v[2].Position.X > MousePosition.x)
                return false;
            float minX = v[0].Position.X, maxX = v[0].Position.X;
            float minY = v[0].Position.Y, maxY = v[0].Position.Y;
            for (var i = 1; i < v.Length; i++)
            {
                if (v[i].Position.X < minX)
                    minX = v[i].Position.X;
                if (v[i].Position.X > maxX)
                    maxX = v[i].Position.X;
                if (v[i].Position.Y < minY)
                    minY = v[i].Position.Y;
                if (v[i].Position.Y > maxY)
                    maxY = v[i].Position.Y;
            }
            var iBoundingBox = new BoundingBox(new Vector3(minX, minY, 0), new Vector3(maxX, maxY, 10));
            if (iBoundingBox.Contains(new Vector3(MousePosition.x, MousePosition.y, 1)) == ContainmentType.Contains)
            {
                var p = new Vector2Int[4];
                p[0] = new Vector2Int((int)v[0].Position.X, (int)v[0].Position.Y);
                p[1] = new Vector2Int((int)v[1].Position.X, (int)v[1].Position.Y);
                p[2] = new Vector2Int((int)v[3].Position.X, (int)v[3].Position.Y);
                p[3] = new Vector2Int((int)v[2].Position.X, (int)v[2].Position.Y);
                if (PointInPolygon(new Vector2Int((int)MousePosition.x, (int)MousePosition.y), p))
                    return true;
            }
            return false;
        }

        bool PointInPolygon(Vector2Int p, Vector2Int[] poly)
        {
            // Taken from http://social.msdn.microsoft.com/forums/en-US/winforms/thread/95055cdc-60f8-4c22-8270-ab5f9870270a/
            Vector2Int p1, p2;
            var inside = false;
            if (poly.Length < 3)
                return inside;
            var oldPoint = new Vector2Int(poly[poly.Length - 1].x, poly[poly.Length - 1].y);
            for (var i = 0; i < poly.Length; i++)
            {
                var newPoint = new Vector2Int(poly[i].x, poly[i].y);
                if (newPoint.x > oldPoint.x)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }
                if ((newPoint.x < p.x) == (p.x <= oldPoint.x) && (p.y - (long)p1.y) * (p2.x - p1.x) < (p2.y - (long)p1.y) * (p.x - p1.x))
                    inside = !inside;
                oldPoint = newPoint;
            }
            return inside;
        }
    }

    
}
