using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    // TODO: implement DATA subrecord
    public class LANDRecord : Record
    {
        /*public class DATAField : Field
        {
            public override void Read(UnityBinaryReader r, uint dataSize) {}
        }*/

        public class VNMLField : Field
        {
            // XYZ 8 bit floats
            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                var vertexCount = dataSize / 3;
                for (var i = 0; i < vertexCount; i++)
                {
                    var xByte = r.ReadByte();
                    var yByte = r.ReadByte();
                    var zByte = r.ReadByte();
                }
            }
        }
        public class VHGTField : Field
        {
            public float ReferenceHeight;
            public sbyte[] HeightOffsets;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                ReferenceHeight = r.ReadLESingle();
                var heightOffsetCount = dataSize - 4 - 2 - 1;
                HeightOffsets = new sbyte[heightOffsetCount];
                for (var i = 0; i < heightOffsetCount; i++)
                    HeightOffsets[i] = r.ReadSByte();
                // unknown
                r.ReadLEInt16();
                // unknown
                r.ReadSByte();
            }
        }
        public class WNAMField : Field
        {
            // Low-LOD heightmap (signed chars)
            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                var heightCount = dataSize;
                for (var i = 0; i < heightCount; i++)
                {
                    var height = r.ReadByte();
                }
            }
        }
        public class VCLRField : Field
        {
            // 24 bit RGB
            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                var vertexCount = dataSize / 3;
                for (var i = 0; i < vertexCount; i++)
                {
                    var rByte = r.ReadByte();
                    var gByte = r.ReadByte();
                    var bByte = r.ReadByte();
                }
            }
        }
        public class VTEXField : Field
        {
            public ushort[] TextureIndices;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                var textureIndexCount = dataSize / 2;
                TextureIndices = new ushort[textureIndexCount];
                for (var i = 0; i < textureIndexCount; i++)
                    TextureIndices[i] = r.ReadLEUInt16();
            }
        }

        public Vector2i GridCoords
        {
            get { return new Vector2i(INTV.Value0, INTV.Value1); }
        }

        public INTVTwoI32Field INTV;
        //public DATAField DATA;
        public VNMLField VNML;
        public VHGTField VHGT;
        public WNAMField WNAM;
        public VCLRField VCLR;
        public VTEXField VTEX;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "INTV": INTV = new INTVTwoI32Field(); return INTV;
                /*case "DATA": DATA = new DATASubRecord(); return DATA;*/
                case "VNML": VNML = new VNMLField(); return VNML;
                case "VHGT": VHGT = new VHGTField(); return VHGT;
                case "WNAM": WNAM = new WNAMField(); return WNAM;
                case "VCLR": VCLR = new VCLRField(); return VCLR;
                case "VTEX": VTEX = new VTEXField(); return VTEX;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}