using System.Runtime.InteropServices;

namespace OA.Ultima.IO
{
    [StructLayout(LayoutKind.Sequential, Pack = 0x1)]
    public struct FileIndexEntry3D
    {
        public int Lookup;
        public int Length;
        public int Extra;

        public FileIndexEntry3D(int lookup, int length, int extra)
        {
            Lookup = lookup;
            Length = length;
            Extra = extra;
        }
    }
}
