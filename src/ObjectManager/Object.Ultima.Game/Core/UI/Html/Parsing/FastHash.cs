using System;
using System.Collections;

namespace OA.Core.UI.Html.Parsing
{
    /// <summary>
    /// FastHash: class provides fast look ups at the expense of memory (at least 128k per object).
    /// Its designed primarily for those hashes where majority of lookups are unsuccessful 
    /// (ie object is not present)
    /// 
    /// Status of this work is EXPERIMENTAL, do not make any untested assumptions.
    /// 
    /// History:    15/12/06 Added range check in GetXY
    ///             sometime in 2005: initial imlpementation
    /// 
    /// </summary>
    ///<exclude/>
    public class FastHash : IDisposable
    {
        /// <summary>
        /// Maximum number of chars to be taken into account
        /// </summary>
        const int MAX_CHARS = 256;

        /// <summary>
        /// Maximum number of keys to be stored
        /// </summary>
        const int MAX_KEYS = 32 * 1024;

        /// <summary>
        /// Value indicating there are multiple keys stored in a given position
        /// </summary>
        const ushort MULTIPLE_KEYS = ushort.MaxValue;

        /// <summary>
        /// Hash that will contain keys and will be used at the last resort as looksup are too slow
        /// </summary>
        Hashtable _hash = new Hashtable();

        /// <summary>
        /// Minimum key length 
        /// </summary>
        int _minLen = int.MaxValue;

        /// <summary>
        /// Maximum key length
        /// </summary>
        int _maxLen = int.MinValue;

        /// <summary>
        /// Array in which we will keep char hints
        /// </summary>
        ushort[,] _chars = new ushort[MAX_CHARS, MAX_CHARS];

        /// <summary>
        /// Keys
        /// </summary>
        string[] _keys = new string[MAX_KEYS];

        /// <summary>
        /// Values of keys
        /// </summary>
        object[] _values = new object[MAX_KEYS];

        /// <summary>
        /// Number of keys stored
        /// </summary>
        ushort _count;

        /// <summary>
        /// Gets keys in this hash
        /// </summary>
        public ICollection Keys
        {
            get { return _hash.Keys; }
        }

        public FastHash()
        {
        }

        /// <summary>
        /// Adds key to the fast hash
        /// </summary>
        /// <param name="sKey">Key</param>
        public void Add(string sKey)
        {
            Add(sKey, 0);
        }

        /// <summary>
        /// Adds key and its value to the fast hash
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Add(string key, object value)
        {
            if (_count >= ushort.MaxValue)
                throw new Exception("Fast hash is full and can't add more keys!");
            if (key.Length == 0)
                return;
            _minLen = (int)Math.Min(key.Length, _minLen);
            _maxLen = (int)Math.Max(key.Length, _maxLen);
            GetXY(key, out int x, out int y);
            if (x < MAX_CHARS && y < MAX_CHARS)
            {
                var cutPos = _chars[x, y];
                if (cutPos == 0)
                {
                    _chars[x, y] = (ushort)(_count + 1);
                    _values[_count] = value;
                    _keys[_count] = key;
                }
                // mark this entry with maxvalue indicating that there is more than one key stored there
                else _chars[x, y] = MULTIPLE_KEYS;
                _count++;
            }
            _hash[key] = value;
        }

        public static void GetXY(string key, out int x, out int y)
        {
            // most likely scenario is that we have at least 2 chars
            if (key.Length >= 2)
            {
                x = key[0];
                y = key[1];
            }
            else
            {
                if (key.Length == 0)
                {
                    x = MAX_CHARS + 1;
                    y = MAX_CHARS + 1;
                    return;
                }
                x = key[0];
                y = 0;
            }
            //Console.WriteLine("{0}: {1}-{2}",sKey,iX,iY);
        }

        /// <summary>
        /// Checks if given key is present in the hash
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True if key is present</returns>
        public bool Contains(string key)
        {
            // if requested string is too short or too long then we can quickly return false
            // NOTE: seems to use too much CPU for the amount of useful work it does

            // NOTE 2: better do it than get nasty excepton...
            if (key.Length < _minLen || key.Length > _maxLen)
                return false;
            GetXY(key, out int x, out int y);
            if (x < MAX_CHARS && y < MAX_CHARS)
            {
                var pos = _chars[x, y];
                if (pos == 0)
                    return false;
                // now check if its just one key
                if (pos != MULTIPLE_KEYS && _keys[pos - 1] == key)
                    return true;
            }

            // finally we have no choice but to do a proper hash lookup
            return _hash[key] != null;
        }

        /// <summary>
        /// Access to values via indexer
        /// </summary>
        public object this[string sKey]
        {
            get { return GetValue(sKey); }
            set
            {
                if (!Contains(sKey))
                    Add(sKey, value);
            }
        }

        /// <summary>
        /// Returns value associated with the key or null if key not present
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Null or object convertable to integer as value</returns>
        public object GetValue(string key)
        {
            // if requested string is too short or too long then we can quickly return false
            if (key.Length < _minLen || key.Length > _maxLen)
                return null;
            GetXY(key, out int x, out int y);
            if (x < MAX_CHARS && y < MAX_CHARS)
            {
                var pos = _chars[x, y];
                if (pos == 0)
                    return null;
                // now check string in list of keys
                if (pos != MULTIPLE_KEYS && _keys[pos - 1] == key)
                    return _values[pos - 1];
            }
            // finally we have no choice but to do a proper hash lookup
            //Console.WriteLine("Have to use hash... :(");
            return _hash[key];
        }

        /// <summary>
        /// Returns value of a key that is VERY likely to be present - this avoids doing some checks that
        /// are most likely to be pointless thus making overall faster function
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Null if no value or value itself</returns>
        public object GetLikelyPresentValue(string key)
        {
            if (key.Length < _minLen || key.Length > _maxLen)
                return null;
            GetXY(key, out int x, out int y);
            if (x < MAX_CHARS && y < MAX_CHARS)
            {
                var pos = _chars[x, y];
                if (pos == 0)
                    return null;
                // now check string is the only one
                if (pos != MULTIPLE_KEYS && _keys[pos - 1] == key)
                    return _values[pos - 1];
            }
            // finally we have no choice but to do a proper hash lookup
            return _hash[key];
        }

        /// <summary>
        /// Returns value for likely present keys using first chars (byte)
        /// </summary>
        /// <param name="x">Byte 1 denoting char 1</param>
        /// <param name="y">Byte 2 denoting char 2 (0 if not present)</param>
        /// <returns>Non-null value if it was found, or null if full search for key is required</returns>
        public object GetLikelyPresentValue(byte x, byte y)
        {
            var pos = _chars[x, y];
            if (pos != MULTIPLE_KEYS && pos != 0)
                return _values[pos - 1];
            // finally we have no choice but to do a proper hash lookup
            return null;
        }

        /// <summary>
        /// Quickly checks if given chars POSSIBLY refer to a stored key.
        /// </summary>
        /// <param name="char1">Char 1</param>
        /// <param name="char2">Char 2</param>
        /// <param name="length">Length of string</param>
        /// <returns>False is string is DEFINATELY NOT present, or true if it MAY be present</returns>
        public bool PossiblyContains(char char1, char char2, int length)
        {
            var pos = _chars[char1, char2];
            if (pos == 0)
                return false;
            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool bisposing)
        {
            _hash = null;
            _chars = null;
            _values = null;
        }
    }
}
