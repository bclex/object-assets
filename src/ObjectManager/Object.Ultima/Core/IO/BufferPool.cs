using System.Collections.Generic;

namespace OA.Ultima.Core.IO
{
    public class BufferPool
    {
        static List<BufferPool> _pools = new List<BufferPool>();

        public static List<BufferPool> Pools { get { return _pools; } set { _pools = value; } }

        string _name;

        int _initialCapacity;
        int _bufferSize;

        int _misses;
        Queue<byte[]> _freeBuffers;

        public void GetInfo(out string name, out int freeCount, out int initialCapacity, out int currentCapacity, out int bufferSize, out int misses)
        {
            lock (this)
            {
                name = _name;
                freeCount = _freeBuffers.Count;
                initialCapacity = _initialCapacity;
                currentCapacity = _initialCapacity * (1 + _misses);
                bufferSize = _bufferSize;
                misses = _misses;
            }
        }

        public BufferPool(string name, int initialCapacity, int bufferSize)
        {
            _name = name;
            _initialCapacity = initialCapacity;
            _bufferSize = bufferSize;
            _freeBuffers = new Queue<byte[]>(initialCapacity);
            for (var i = 0; i < initialCapacity; ++i)
                _freeBuffers.Enqueue(new byte[bufferSize]);
            lock (_pools)
                _pools.Add(this);
        }

        public byte[] AcquireBuffer()
        {
            lock (this)
            {
                if (_freeBuffers.Count > 0)
                    return _freeBuffers.Dequeue();
                ++_misses;
                for (var i = 0; i < _initialCapacity; ++i)
                    _freeBuffers.Enqueue(new byte[_bufferSize]);
                return _freeBuffers.Dequeue();
            }
        }

        public void ReleaseBuffer(byte[] buffer)
        {
            if (buffer == null)
                return;
            lock (this)
                _freeBuffers.Enqueue(buffer);
        }

        public void Free()
        {
            lock (_pools)
                _pools.Remove(this);
        }
    }
}
