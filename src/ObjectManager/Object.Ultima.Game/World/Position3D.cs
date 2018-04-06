using System;
using UnityEngine;

namespace OA.Ultima.World
{
    public interface IPoint2D
    {
        int X { get; }
        int Y { get; }
    }

    public class Position3D : IPoint2D
    {
        public static Vector2Int NullTile = new Vector2Int(int.MinValue, int.MinValue);

        Vector2Int _tile;
        int _z;
        Vector3 _offset;

        public Vector2Int Tile
        {
            get { return _tile; }
            set
            {
                if (_tile != value)
                {
                    _tile = value;
                    if (_tile != NullTile && _onTileChanged != null)
                        _onTileChanged(_tile.x, _tile.y);
                }
            }
        }

        public Vector3 Offset { get { return _offset; } set { _offset = value; } }

        public bool IsOffset { get { return _offset != Vector3.zero; } }
        public bool IsNullPosition { get { return _tile == NullTile; } }

        public int X
        {
            get { return _tile.x; }
        }

        public int Y
        {
            get { return _tile.y; }
        }

        public int Z
        {
            get { return _z; }
        }

        public float X_offset { get { return _offset.x % 1.0f; } }
        public float Y_offset { get { return _offset.y % 1.0f; } }
        public float Z_offset { get { return _offset.z; } }

        private Action<int, int> _onTileChanged;

        public Position3D(Action<int, int> onTileChanged)
        {
            Tile = NullTile;
            _onTileChanged = onTileChanged;
        }

        public Position3D(int x, int y, int z)
        {
            Tile = new Vector2Int(x, y);
            _z = z;
        }

        internal void Set(int x, int y, int z)
        {
            _z = z;
            Tile = new Vector2Int(x, y);
            _offset = Vector3.zero;
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (o.GetType() != typeof(Position3D)) return false;
            if (X != ((Position3D)o).X) return false;
            if (Y != ((Position3D)o).Y) return false;
            if (Z != ((Position3D)o).Z) return false;
            return true;
        }

        // Equality operator. Returns dbNull if either operand is dbNull, 
        // otherwise returns dbTrue or dbFalse:
        public static bool operator ==(Position3D x, Position3D y)
        {
            if ((object)x == null)
                return ((object)y == null);
            return x.Equals(y);
        }

        // Inequality operator. Returns dbNull if either operand is
        // dbNull, otherwise returns dbTrue or dbFalse:
        public static bool operator !=(Position3D x, Position3D y)
        {
            if ((object)x == null)
                return ((object)y != null);
            return !x.Equals(y);
        }

        public override int GetHashCode()
        {
            return X ^ Y ^ Z;
        }

        public override string ToString()
        {
            return string.Format("X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

        public string ToStringComplex()
        {
            return
                "P(Tile)=" + ToString() + Environment.NewLine +
                "P(Ofst)=" + string.Format("X:{0:0.00} Y:{1:0.00} Z:{2:0.00}", X_offset, Y_offset, Z_offset) + Environment.NewLine +
                "D(Tile)=" + string.Format("X:{0:0.00} Y:{1:0.00} Z:{2:0.00}", X, Y, Z) + Environment.NewLine +
                "D(Ofst)=" + string.Format("X:{0:0.00} Y:{1:0.00} Z:{2:0.00}", X_offset, Y_offset, Z_offset);
        }
    }
}
