using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class LANDRecord : Record
    {
        public struct CORDField
        {
            public override string ToString() => $"{CellX}, {CellY}";
            public int CellX, CellY;

            public CORDField(UnityBinaryReader r, uint dataSize)
            {
                CellX = r.ReadLEInt32();
                CellY = r.ReadLEInt32();
            }
        }

        public struct VNMLField
        {
            // XYZ 8 bit floats
            public VNMLField(UnityBinaryReader r, uint dataSize)
            {
                var vertexCount = dataSize / 3;
                for (var i = 0; i < vertexCount; i++)
                {
                    var x = r.ReadByte();
                    var y = r.ReadByte();
                    var z = r.ReadByte();
                }
            }
        }

        public struct VHGTField
        {
            public float ReferenceHeight; // A height offset for the entire cell. Decreasing this value will shift the entire cell land down.
            public sbyte[] HeightData; // HeightData

            public VHGTField(UnityBinaryReader r, uint dataSize)
            {
                ReferenceHeight = r.ReadLESingle();
                var heightDataCount = dataSize - 4 - 2 - 1;
                HeightData = new sbyte[heightDataCount];
                for (var i = 0; i < heightDataCount; i++)
                    HeightData[i] = r.ReadSByte();
                r.ReadLEInt16(); // unknown
                r.ReadSByte(); // unknown
            }
        }

        public struct WNAMField
        {
            // Low-LOD heightmap (signed chars)
            public WNAMField(UnityBinaryReader r, uint dataSize)
            {
                var heightCount = dataSize;
                for (var i = 0; i < heightCount; i++)
                {
                    var height = r.ReadByte();
                }
            }
        }

        public struct VCLRField
        {
            // 24 bit RGB
            public VCLRField(UnityBinaryReader r, uint dataSize)
            {
                var vertexCount = dataSize / 3;
                for (var i = 0; i < vertexCount; i++)
                {
                    var r_ = r.ReadByte();
                    var g = r.ReadByte();
                    var b = r.ReadByte();
                }
            }
        }

        public struct VTEXField
        {
            public ushort[] TextureIndices;

            public VTEXField(UnityBinaryReader r, uint dataSize)
            {
                var textureIndexCount = dataSize / 2;
                TextureIndices = new ushort[textureIndexCount];
                for (var i = 0; i < textureIndexCount; i++)
                    TextureIndices[i] = r.ReadLEUInt16();
            }
        }


        public override string ToString() => $"LAND: {INTV}";
        public CORDField INTV; // The cell coordinates of the cell
        public IN32Field DATA; // Unknown (default of 0x09) Changing this value makes the land 'disappear' in the editor.
        public VNMLField VNML; // A RGB color map 65x65 pixels in size representing the land normal vectors.
                               // The signed value of the 'color' represents the vector's component. Blue
                               // is vertical(Z), Red the X direction and Green the Y direction.Note that
                               // the y-direction of the data is from the bottom up.
        public VHGTField VHGT; // Height data
        public WNAMField WNAM; // Unknown byte data.
        public VCLRField? VCLR; // Vertex color array, looks like another RBG image 65x65 pixels in size. (Optional)
        public VTEXField? VTEX; // A 16x16 array of short texture indices. (Optional)

        public Vector2i GridCoords => new Vector2i(INTV.CellX, INTV.CellY);

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "INTV": INTV = new CORDField(r, dataSize); return true;
                    case "DATA": DATA = new IN32Field(r, dataSize); return true;
                    case "VNML": VNML = new VNMLField(r, dataSize); return true;
                    case "VHGT": VHGT = new VHGTField(r, dataSize); return true;
                    case "WNAM": WNAM = new WNAMField(r, dataSize); return true;
                    case "VCLR": VCLR = new VCLRField(r, dataSize); return true;
                    case "VTEX": VTEX = new VTEXField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}