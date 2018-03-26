using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OA.Ultima.Resources
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StaticTile : IComparable<StaticTile>
    {
        public short ID;
        public byte X;
        public byte Y;
        public sbyte Z;
        public short Hue;

        public override string ToString()
        {
            var b = new StringBuilder();
            b.AppendLine("ID: " + ID.ToString());
            b.AppendLine("X: " + X.ToString());
            b.AppendLine("Y: " + Y.ToString());
            b.AppendLine("Z: " + Z.ToString());
            b.AppendLine("Hue: " + Hue.ToString());
            return b.ToString();
        }

        public int CompareTo(StaticTile t)
        {
            return (Z - t.Z);
        }
    }
}
