﻿using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class GRASRecord : Record
    {
        public struct DATAField
        {
            public byte Density;
            public byte MinSlope;
            public byte MaxSlope;
            public ushort UnitFromWaterAmount;
            public uint UnitFromWaterType;
            //Above - At Least,
            //Above - At Most,
            //Below - At Least,
            //Below - At Most,
            //Either - At Least,
            //Either - At Most,
            //Either - At Most Above,
            //Either - At Most Below
            public float PositionRange;
            public float HeightRange;
            public float ColorRange;
            public float WavePeriod;
            public byte Flags;

            public DATAField(UnityBinaryReader r, uint dataSize)
            {
                Density = r.ReadByte();
                MinSlope = r.ReadByte();
                MaxSlope = r.ReadByte();
                r.ReadByte();
                UnitFromWaterAmount = r.ReadLEUInt16();
                r.ReadBytes(2);
                UnitFromWaterType = r.ReadLEUInt32();
                PositionRange = r.ReadLESingle();
                HeightRange = r.ReadLESingle();
                ColorRange = r.ReadLESingle();
                WavePeriod = r.ReadLESingle();
                Flags = r.ReadByte();
                r.ReadBytes(3);
            }
        }

        public override string ToString() => $"GRAS: {EDID.Value}";
        public STRVField EDID { get; set; }
        public MODLGroup MODL;
        public DATAField DATA;

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                case "DATA": DATA = new DATAField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}