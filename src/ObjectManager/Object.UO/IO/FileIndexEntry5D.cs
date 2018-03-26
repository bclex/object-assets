using System.Runtime.InteropServices;

namespace OA.Ultima.IO
{
    [StructLayout(LayoutKind.Sequential, Pack = 0x1)]
    public struct FileIndexEntry5D
    {
        public int file;
        public int index;
        public int lookup;
        public int length;
        public int extra;
    }
}