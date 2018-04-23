using OA.Core;

namespace OA.Ultima.FilePacks.Records
{
    public class LANDRecord : Record
    {
        public class VHGTSubRecord
        {
            public float referenceHeight;
            public sbyte[] heightOffsets;
        }

        public class VTEXSubRecord
        {
            public ushort[] textureIndices;
        }

        public VHGTSubRecord VHGT;
        public VTEXSubRecord VTEX;

        public int X;
        public int Y;

        public Vector2i GridCoords
        {
            get { return new Vector2i(X, Y); }
        }
    }
}