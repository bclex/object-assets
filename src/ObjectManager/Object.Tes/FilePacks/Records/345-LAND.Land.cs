using OA.Core;
using UnityEngine;

namespace OA.Tes.FilePacks.Records
{
    public class LANDRecord : Record
    {
        // TESX
        public struct VNMLField
        {
            public Vector3Int[] Vertexs; // XYZ 8 bit floats

            public VNMLField(UnityBinaryReader r, int dataSize)
            {
                Vertexs = new Vector3Int[dataSize / 3];
                for (var i = 0; i < Vertexs.Length; i++)
                    Vertexs[i] = new Vector3Int(r.ReadByte(), r.ReadByte(), r.ReadByte());
            }
        }

        public struct VHGTField
        {
            public float ReferenceHeight; // A height offset for the entire cell. Decreasing this value will shift the entire cell land down.
            public sbyte[] HeightData; // HeightData

            public VHGTField(UnityBinaryReader r, int dataSize)
            {
                ReferenceHeight = r.ReadLESingle();
                HeightData = new sbyte[dataSize - 4 - 3];
                for (var i = 0; i < HeightData.Length; i++)
                    HeightData[i] = r.ReadSByte();
                r.ReadBytes(3); // Unused
            }
        }

        public struct VCLRField
        {
            public ColorRef[] Colors; // 24-bit RGB

            public VCLRField(UnityBinaryReader r, int dataSize)
            {
                Colors = new ColorRef[dataSize / 24];
                for (var i = 0; i < Colors.Length; i++)
                    Colors[i] = new ColorRef(r.ReadByte(), r.ReadByte(), r.ReadByte());
            }
        }

        public struct VTEXField
        {
            public uint[] TextureIndices;

            public VTEXField(UnityBinaryReader r, int dataSize, GameFormatId formatId)
            {
                if (formatId == GameFormatId.TES3)
                {
                    TextureIndices = new uint[dataSize >> 1];
                    for (var i = 0; i < TextureIndices.Length; i++)
                        TextureIndices[i] = r.ReadLEUInt16();
                }
                else
                {
                    TextureIndices = new uint[dataSize >> 2];
                    for (var i = 0; i < TextureIndices.Length; i++)
                        TextureIndices[i] = r.ReadLEUInt32();
                }
            }
        }

        // TES3
        public struct CORDField
        {
            public override string ToString() => $"{CellX}, {CellY}";
            public int CellX, CellY;

            public CORDField(UnityBinaryReader r, int dataSize)
            {
                CellX = r.ReadLEInt32();
                CellY = r.ReadLEInt32();
            }
        }

        public struct WNAMField
        {
            // Low-LOD heightmap (signed chars)
            public WNAMField(UnityBinaryReader r, int dataSize)
            {
                var heightCount = dataSize;
                for (var i = 0; i < heightCount; i++)
                {
                    var height = r.ReadByte();
                }
            }
        }

        // TES4
        public struct BTXTField
        {
            public uint Texture;
            public byte Quadrant;
            public short Layer;

            public BTXTField(UnityBinaryReader r, int dataSize)
            {
                Texture = r.ReadLEUInt32();
                Quadrant = r.ReadByte();
                r.ReadByte(); // Unused
                Layer = r.ReadLEInt16();
            }
        }

        public struct VTXTField
        {
            public ushort Position;
            public float Opacity;

            public VTXTField(UnityBinaryReader r, int dataSize)
            {
                Position = r.ReadLEUInt16();
                r.ReadBytes(2); // Unused
                Opacity = r.ReadLESingle();
            }
        }

        public class ATXTGroup
        {
            public BTXTField ATXT;
            public VTXTField[] VTXTs;
        }

        public override string ToString() => $"LAND: {INTV}";
        public IN32Field DATA; // Unknown (default of 0x09) Changing this value makes the land 'disappear' in the editor.
        public VNMLField VNML; // A RGB color map 65x65 pixels in size representing the land normal vectors.
                               // The signed value of the 'color' represents the vector's component. Blue
                               // is vertical(Z), Red the X direction and Green the Y direction.Note that
                               // the y-direction of the data is from the bottom up.
        public VHGTField VHGT; // Height data
        public VNMLField? VCLR; // Vertex color array, looks like another RBG image 65x65 pixels in size. (Optional)
        public VTEXField? VTEX; // A 16x16 array of short texture indices. (Optional)
        // TES3
        public CORDField INTV; // The cell coordinates of the cell
        public WNAMField WNAM; // Unknown byte data.
        // TES4
        public BTXTField[] BTXTs = new BTXTField[4]; // Base Layer
        public ATXTGroup[] ATXTs; // Alpha Layer
        ATXTGroup _lastATXT;

        public Vector2i GridCoords => new Vector2i(INTV.CellX, INTV.CellY);

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            switch (type)
            {
                case "DATA": DATA = new IN32Field(r, dataSize); return true;
                case "VNML": VNML = new VNMLField(r, dataSize); return true;
                case "VHGT": VHGT = new VHGTField(r, dataSize); return true;
                case "VCLR": VCLR = new VNMLField(r, dataSize); return true;
                case "VTEX": VTEX = new VTEXField(r, dataSize, formatId); return true;
                // TES3
                case "INTV": INTV = new CORDField(r, dataSize); return true;
                case "WNAM": WNAM = new WNAMField(r, dataSize); return true;
                // TES4
                case "BTXT": var btxt = new BTXTField(r, dataSize); BTXTs[btxt.Quadrant] = btxt; return true;
                case "ATXT": if (ATXTs == null) ATXTs = new ATXTGroup[4]; var atxt = new BTXTField(r, dataSize); _lastATXT = ATXTs[atxt.Quadrant] = new ATXTGroup { ATXT = atxt }; return true;
                case "VTXT": var vtxt = new VTXTField[dataSize >> 3]; for (var i = 0; i < vtxt.Length; i++) vtxt[i] = new VTXTField(r, dataSize); _lastATXT.VTXTs = vtxt; return true;
                default: return false;
            }
        }
    }
}