using OA.Core;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Tes.FilePacks.Records
{
    public class REGNRecord : Record, IHaveEDID
    {
        // TESX
        public class RDATField
        {
            public enum REGNType : byte
            {
                Objects = 2, Weather, Map, Landscape, Grass, Sound
            }

            public uint Type;
            public REGNType Flags;
            public byte Priority;
            // groups
            public RDOTField[] RDOTs; // Objects
            public STRVField RDMP; // MapName
            public RDGSField[] RDGSs; // Grasses
            public UI32Field RDMD; // Music Type
            public RDSDField[] RDSDs; // Sounds
            public RDWTField[] RDWTs; // Weather Types

            public RDATField() { }
            public RDATField(UnityBinaryReader r, int dataSize)
            {
                Type = r.ReadLEUInt32();
                Flags = (REGNType)r.ReadByte();
                Priority = r.ReadByte();
                r.SkipBytes(2); // Unused
            }
        }

        public struct RDOTField
        {
            public override string ToString() => $"{Object}";
            public FormId<Record> Object;
            public ushort ParentIdx;
            public float Density;
            public byte Clustering;
            public byte MinSlope; // (degrees)
            public byte MaxSlope; // (degrees)
            public byte Flags;
            public ushort RadiusWrtParent;
            public ushort Radius;
            public float MinHeight;
            public float MaxHeight;
            public float Sink;
            public float SinkVariance;
            public float SizeVariance;
            public Vector3Int AngleVariance;
            public ColorRef VertexShading; // RGB + Shading radius (0 - 200) %

            public RDOTField(UnityBinaryReader r, int dataSize)
            {
                Object = new FormId<Record>(r.ReadLEUInt32());
                ParentIdx = r.ReadLEUInt16();
                r.SkipBytes(2); // Unused
                Density = r.ReadLESingle();
                Clustering = r.ReadByte();
                MinSlope = r.ReadByte();
                MaxSlope = r.ReadByte();
                Flags = r.ReadByte();
                RadiusWrtParent = r.ReadLEUInt16();
                Radius = r.ReadLEUInt16();
                MinHeight = r.ReadLESingle();
                MaxHeight = r.ReadLESingle();
                Sink = r.ReadLESingle();
                SinkVariance = r.ReadLESingle();
                SizeVariance = r.ReadLESingle();
                AngleVariance = new Vector3Int(r.ReadLEUInt16(), r.ReadLEUInt16(), r.ReadLEUInt16());
                r.SkipBytes(2); // Unused
                VertexShading = new ColorRef(r);
            }
        }

        public struct RDGSField
        {
            public override string ToString() => $"{Grass}";
            public FormId<GRASRecord> Grass;

            public RDGSField(UnityBinaryReader r, int dataSize)
            {
                Grass = new FormId<GRASRecord>(r.ReadLEUInt32());
                r.SkipBytes(4); // Unused
            }
        }

        public struct RDSDField
        {
            public override string ToString() => $"{Sound}";
            public FormId<SOUNRecord> Sound;
            public uint Flags;
            public uint Chance;

            public RDSDField(UnityBinaryReader r, int dataSize, GameFormatId format)
            {
                if (format == GameFormatId.TES3)
                {
                    Sound = new FormId<SOUNRecord>(r.ReadASCIIString(32, ASCIIFormat.ZeroPadded));
                    Flags = 0;
                    Chance = r.ReadByte();
                    return;
                }
                Sound = new FormId<SOUNRecord>(r.ReadLEUInt32());
                Flags = r.ReadLEUInt32();
                Chance = r.ReadLEUInt32(); //: float with TES5
            }
        }

        public struct RDWTField
        {
            public override string ToString() => $"{Weather}";
            public static byte SizeOf(GameFormatId format) => format == GameFormatId.TES4 ? (byte)8 : (byte)12;
            public FormId<WTHRRecord> Weather;
            public uint Chance;
            public FormId<GLOBRecord> Global;

            public RDWTField(UnityBinaryReader r, int dataSize, GameFormatId format)
            {
                Weather = new FormId<WTHRRecord>(r.ReadLEUInt32());
                Chance = r.ReadLEUInt32();
                Global = format == GameFormatId.TES5 ? new FormId<GLOBRecord>(r.ReadLEUInt32()) : new FormId<GLOBRecord>();
            }
        }

        // TES3
        public struct WEATField
        {
            public byte Clear;
            public byte Cloudy;
            public byte Foggy;
            public byte Overcast;
            public byte Rain;
            public byte Thunder;
            public byte Ash;
            public byte Blight;

            public WEATField(UnityBinaryReader r, int dataSize)
            {
                Clear = r.ReadByte();
                Cloudy = r.ReadByte();
                Foggy = r.ReadByte();
                Overcast = r.ReadByte();
                Rain = r.ReadByte();
                Thunder = r.ReadByte();
                Ash = r.ReadByte();
                Blight = r.ReadByte();
                // v1.3 ESM files add 2 bytes to WEAT subrecords.
                if (dataSize == 10)
                    r.SkipBytes(2);
            }
        }

        // TES4
        public class RPLIField
        {
            public uint EdgeFalloff; // (World Units)
            public Vector2[] Points; // Region Point List Data

            public RPLIField(UnityBinaryReader r, int dataSize)
            {
                EdgeFalloff = r.ReadLEUInt32();
            }

            public void RPLDField(UnityBinaryReader r, int dataSize)
            {
                Points = new Vector2[dataSize >> 3];
                for (var i = 0; i < Points.Length; i++) Points[i] = new Vector2(r.ReadLESingle(), r.ReadLESingle());
            }
        }

        public override string ToString() => $"REGN: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public STRVField ICON; // Icon / Sleep creature
        public FMIDField<WRLDRecord> WNAM; // Worldspace - Region name
        public CREFField RCLR; // Map Color (COLORREF)
        public List<RDATField> RDATs = new List<RDATField>(); // Region Data Entries / TES3: Sound Record (order determines the sound priority)
        // TES3
        public WEATField? WEAT; // Weather Data
        // TES4
        public List<RPLIField> RPLIs = new List<RPLIField>(); // Region Areas

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "WNAM":
                case "FNAM": WNAM = new FMIDField<WRLDRecord>(r, dataSize); return true;
                case "WEAT": WEAT = new WEATField(r, dataSize); return true; //: TES3
                case "ICON":
                case "BNAM": ICON = new STRVField(r, dataSize); return true;
                case "RCLR":
                case "CNAM": RCLR = new CREFField(r, dataSize); return true;
                case "SNAM": RDATs.Add(new RDATField { RDSDs = new[] { new RDSDField(r, dataSize, format) } }); return true;
                case "RPLI": RPLIs.Add(new RPLIField(r, dataSize)); return true;
                case "RPLD": ArrayUtils.Last(RPLIs).RPLDField(r, dataSize); return true;
                case "RDAT": RDATs.Add(new RDATField(r, dataSize)); return true;
                case "RDOT":
                    var rdot = ArrayUtils.Last(RDATs).RDOTs = new RDOTField[dataSize / 52];
                    for (var i = 0; i < rdot.Length; i++) rdot[i] = new RDOTField(r, dataSize); return true;
                case "RDMP": ArrayUtils.Last(RDATs).RDMP = new STRVField(r, dataSize); return true;
                case "RDGS":
                    var rdgs = ArrayUtils.Last(RDATs).RDGSs = new RDGSField[dataSize / 8];
                    for (var i = 0; i < rdgs.Length; i++) rdgs[i] = new RDGSField(r, dataSize); return true;
                case "RDMD": ArrayUtils.Last(RDATs).RDMD = new UI32Field(r, dataSize); return true;
                case "RDSD":
                    var rdsd = ArrayUtils.Last(RDATs).RDSDs = new RDSDField[dataSize / 12];
                    for (var i = 0; i < rdsd.Length; i++) rdsd[i] = new RDSDField(r, dataSize, format); return true;
                case "RDWT":
                    var rdwt = ArrayUtils.Last(RDATs).RDWTs = new RDWTField[dataSize / RDWTField.SizeOf(format)];
                    for (var i = 0; i < rdwt.Length; i++) rdwt[i] = new RDWTField(r, dataSize, format); return true;
                default: return false;
            }
        }
    }
}