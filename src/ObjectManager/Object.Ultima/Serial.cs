using System;

namespace OA.Ultima
{
    public struct Serial : IComparable, IComparable<Serial>
    {
        public static Serial ProtectedAction = int.MinValue;

        public static Serial Null
        {
            get { return 0; }
        }

        public readonly static Serial World = unchecked((int)0xFFFFFFFF);

        readonly int _serial;

        private Serial(int serial)
        {
            _serial = serial;
        }

        public int Value
        {
            get { return _serial; }
        }

        public bool IsMobile
        {
            get { return (_serial > 0 && _serial < 0x40000000); }
        }

        public bool IsItem
        {
            get { return (_serial >= 0x40000000); }
        }

        public bool IsValid
        {
            get { return (_serial > 0); }
        }

        public bool IsDynamic
        {
            get { return (_serial < 0); }
        }

        static int _nextDynamicSerial = -1;
        public static int NewDynamicSerial
        {
            get { return _nextDynamicSerial--; }
        }

        public override int GetHashCode()
        {
            return _serial;
        }

        public int CompareTo(Serial other)
        {
            return _serial.CompareTo(other._serial);
        }

        public int CompareTo(object other)
        {
            if (other is Serial)
                return CompareTo((Serial)other);
            else if (other == null)
                return -1;
            throw new ArgumentException();
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Serial)) return false;
            return ((Serial)o)._serial == _serial;
        }

        public static bool operator ==(Serial l, Serial r)
        {
            return l._serial == r._serial;
        }

        public static bool operator !=(Serial l, Serial r)
        {
            return l._serial != r._serial;
        }

        public static bool operator >(Serial l, Serial r)
        {
            return l._serial > r._serial;
        }

        public static bool operator <(Serial l, Serial r)
        {
            return l._serial < r._serial;
        }

        public static bool operator >=(Serial l, Serial r)
        {
            return l._serial >= r._serial;
        }

        public static bool operator <=(Serial l, Serial r)
        {
            return l._serial <= r._serial;
        }

        public override string ToString()
        {
            return String.Format("0x{0:X8}", _serial);
        }

        public static implicit operator int(Serial a)
        {
            return a._serial;
        }

        public static implicit operator Serial(int a)
        {
            return new Serial(a);
        }
    }
}
