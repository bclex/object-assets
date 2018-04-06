using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;

namespace OA.Ultima.Core.IO
{
    public sealed class AsyncWriter : GenericWriter
    {
        static int _threadCount;

        readonly int BufferSize;

        readonly bool PrefixStrings;

        readonly FileStream _file;

        readonly Queue _writeQueue;
        BinaryWriter _bin;
        bool _closed;
        long _curPos;
        long _lastPos;
        MemoryStream _mem;
        Thread _workerThread;

        public AsyncWriter(string filename, bool prefix)
            : this(filename, 1048576, prefix) { } //1 mb buffer

        public AsyncWriter(string filename, int buffSize, bool prefix)
        {
            PrefixStrings = prefix;
            _closed = false;
            _writeQueue = Queue.Synchronized(new Queue());
            BufferSize = buffSize;
            _file = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            _mem = new MemoryStream(BufferSize + 1024);
            _bin = new BinaryWriter(_mem, Utility.UTF8WithEncoding);
        }

        public static int ThreadCount
        {
            get { return _threadCount; }
        }

        public MemoryStream MemStream
        {
            get { return _mem; }
            set
            {
                if (_mem.Length > 0)
                    Enqueue(_mem);
                _mem = value;
                _bin = new BinaryWriter(_mem, Utility.UTF8WithEncoding);
                _lastPos = 0;
                _curPos = _mem.Length;
                _mem.Seek(0, SeekOrigin.End);
            }
        }

        public override long Position
        {
            get { return _curPos; }
        }

        private void Enqueue(MemoryStream mem)
        {
            _writeQueue.Enqueue(mem);
            if (_workerThread == null || !_workerThread.IsAlive)
            {
                _workerThread = new Thread(new WorkerThread(this).Worker);
                _workerThread.Priority = ThreadPriority.BelowNormal;
                _workerThread.Start();
            }
        }

        private void OnWrite()
        {
            var curlen = _mem.Length;
            _curPos += curlen - _lastPos;
            _lastPos = curlen;
            if (curlen >= BufferSize)
            {
                Enqueue(_mem);
                _mem = new MemoryStream(BufferSize + 1024);
                _bin = new BinaryWriter(_mem, Utility.UTF8WithEncoding);
                _lastPos = 0;
            }
        }

        public override void Close()
        {
            Enqueue(_mem);
            _closed = true;
        }

        public override void Write(IPAddress value)
        {
            _bin.Write(Utility.GetLongAddressValue(value));
            OnWrite();
        }

        public override void Write(string value)
        {
            if (PrefixStrings)
            {
                if (value == null)
                    _bin.Write((byte)0);
                else
                {
                    _bin.Write((byte)1);
                    _bin.Write(value);
                }
            }
            else _bin.Write(value);
            OnWrite();
        }

        public override void WriteDeltaTime(DateTime value)
        {
            var ticks = value.Ticks;
            var now = DateTime.Now.Ticks;
            TimeSpan d;
            try
            {
                d = new TimeSpan(ticks - now);
            }
            catch
            {
                if (ticks < now) d = TimeSpan.MaxValue;
                else d = TimeSpan.MaxValue;
            }
            Write(d);
        }

        public override void Write(DateTime value)
        {
            _bin.Write(value.Ticks);
            OnWrite();
        }

        public override void Write(TimeSpan value)
        {
            _bin.Write(value.Ticks);
            OnWrite();
        }

        public override void Write(decimal value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void Write(long value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void Write(ulong value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void WriteEncodedInt(int value)
        {
            var v = (uint)value;
            while (v >= 0x80)
            {
                _bin.Write((byte)(v | 0x80));
                v >>= 7;
            }
            _bin.Write((byte)v);
            OnWrite();
        }

        public override void Write(int value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void Write(uint value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void Write(short value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void Write(ushort value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void Write(double value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void Write(float value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void Write(char value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void Write(byte value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void Write(byte[] value)
        {
            for (var i = 0; i < value.Length; i++)
                _bin.Write(value[i]);
            OnWrite();
        }

        public override void Write(sbyte value)
        {
            _bin.Write(value);
            OnWrite();
        }

        public override void Write(bool value)
        {
            _bin.Write(value);
            OnWrite();
        }

        private class WorkerThread
        {
            readonly AsyncWriter _parent;

            public WorkerThread(AsyncWriter parent)
            {
                _parent = parent;
            }

            public void Worker()
            {
                _threadCount++;
                while (_parent._writeQueue.Count > 0)
                {
                    var mem = (MemoryStream)_parent._writeQueue.Dequeue();
                    if (mem != null && mem.Length > 0)
                        mem.WriteTo(_parent._file);
                }
                if (_parent._closed)
                    _parent._file.Close();
                _threadCount--;
                if (_threadCount <= 0)
                {
                    // Program.NotifyDiskWriteComplete();
                }
            }
        }
    }
}