﻿using OA.Core;
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
            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                var vertexCount = Header.DataSize / 3;
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
            public float ReferenceHeight;
            public sbyte[] HeightOffsets;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                ReferenceHeight = r.ReadLESingle();
                var heightOffsetCount = Header.DataSize - 4 - 2 - 1;
                HeightOffsets = new sbyte[heightOffsetCount];
                for (var i = 0; i < heightOffsetCount; i++)
                    HeightOffsets[i] = r.ReadSByte();
                // unknown
                r.ReadLEInt16();
                // unknown
                r.ReadSByte();
            }
        }
        public class WNAMSubRecord : SubRecord
        {
            // Low-LOD heightmap (signed chars)
            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                var heightCount = Header.DataSize;
                for (var i = 0; i < heightCount; i++)
                {
                    var height = r.ReadByte();
                }
            }
        }
        public class VCLRSubRecord : SubRecord
        {
            // 24 bit RGB
            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                var vertexCount = Header.DataSize / 3;
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
            public ushort[] TextureIndices;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                var textureIndexCount = Header.DataSize / 2;
                TextureIndices = new ushort[textureIndexCount];
                for (var i = 0; i < textureIndexCount; i++)
                    TextureIndices[i] = r.ReadLEUInt16();
            }
        }

        public Vector2i GridCoords
        {
            get { return new Vector2i(INTV.Value0, INTV.Value1); }
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