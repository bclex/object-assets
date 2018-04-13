using OA.Core;
using OA.Tes.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    // TODO: implement DATA subrecord
    public class LANDRecord : Record
    {
        /*public class DATASubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader r) {}
        }*/

        public class VNMLSubRecord : SubRecord
        {
            // XYZ 8 bit floats
            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                var vertexCount = header.dataSize / 3;
                for (var i = 0; i < vertexCount; i++)
                {
                    var xByte = r.ReadByte();
                    var yByte = r.ReadByte();
                    var zByte = r.ReadByte();
                }
            }
        }
        public class VHGTSubRecord : SubRecord
        {
            public float referenceHeight;
            public sbyte[] heightOffsets;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                referenceHeight = r.ReadLESingle();
                var heightOffsetCount = header.dataSize - 4 - 2 - 1;
                heightOffsets = new sbyte[heightOffsetCount];
                for (var i = 0; i < heightOffsetCount; i++)
                    heightOffsets[i] = r.ReadSByte();
                // unknown
                r.ReadLEInt16();
                // unknown
                r.ReadSByte();
            }
        }
        public class WNAMSubRecord : SubRecord
        {
            // Low-LOD heightmap (signed chars)
            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                var heightCount = header.dataSize;
                for (var i = 0; i < heightCount; i++)
                {
                    var height = r.ReadByte();
                }
            }
        }
        public class VCLRSubRecord : SubRecord
        {
            // 24 bit RGB
            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                var vertexCount = header.dataSize / 3;
                for (var i = 0; i < vertexCount; i++)
                {
                    var rByte = r.ReadByte();
                    var gByte = r.ReadByte();
                    var bByte = r.ReadByte();
                }
            }
        }
        public class VTEXSubRecord : SubRecord
        {
            public ushort[] textureIndices;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                var textureIndexCount = header.dataSize / 2;
                textureIndices = new ushort[textureIndexCount];
                for (var i = 0; i < textureIndexCount; i++)
                    textureIndices[i] = r.ReadLEUInt16();
            }
        }

        public Vector2i gridCoords
        {
            get { return new Vector2i(INTV.value0, INTV.value1); }
        }

        public INTVTwoI32SubRecord INTV;
        //public DATASubRecord DATA;
        public VNMLSubRecord VNML;
        public VHGTSubRecord VHGT;
        public WNAMSubRecord WNAM;
        public VCLRSubRecord VCLR;
        public VTEXSubRecord VTEX;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "INTV": INTV = new INTVTwoI32SubRecord(); return INTV;
                /*case "DATA": DATA = new DATASubRecord(); return DATA;*/
                case "VNML": VNML = new VNMLSubRecord(); return VNML;
                case "VHGT": VHGT = new VHGTSubRecord(); return VHGT;
                case "WNAM": WNAM = new WNAMSubRecord(); return WNAM;
                case "VCLR": VCLR = new VCLRSubRecord(); return VCLR;
                case "VTEX": VTEX = new VTEXSubRecord(); return VTEX;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}