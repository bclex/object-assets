//
//  REGNRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class REGNRecord: Record, IHaveEDID {
    // TESX
    public class RDATField
    {
        public enum REGNType : byte
        {
            Objects = 0x02,
            Weather = 0x03,
            Map = 0x04,
            Landscape = 0x05, // (CK will save but not show data entered here)
            Grass = 0x06,
            Sound = 0x07,
        }

        public uint Type;
        public REGNType Flags;
        public byte Priority;
        // groups
        public RDOTField[] RDOTs // Objects
        public STRVField RDMP // MapName
        public RDGSField[] RDGSs // Grasses
        public UI32Field RDMD // Music Type
        public RDSDField[] RDSDs // Sounds
        public RDWTField[] RDWTs // Weather Types

        public RDATField() { }
        public RDATField(UnityBinaryReader r, uint dataSize)
        {
            Type = r.readLEUInt32();
            Flags = (REGNType)r.readByte();
            Priority = r.readByte();
            r.skipBytes(2) // Unused
        }
    }

    public struct RDOTField
    {
        public override string ToString() => $"{Object}";
        public FormId<Record> Object;
        public ushort ParentIdx;
        public float Density;
        public byte Clustering;
        public byte MinSlope // (degrees)
        public byte MaxSlope // (degrees)
        public byte Flags;
        public ushort RadiusWrtParent;
        public ushort Radius;
        public float MinHeight;
        public float MaxHeight;
        public float Sink;
        public float SinkVariance;
        public float SizeVariance;
        public Vector3Int AngleVariance;
        public ColorRef VertexShading // RGB + Shading radius (0 - 200) %

        public RDOTField(UnityBinaryReader r, uint dataSize)
        {
            Object = FormId<Record>(r.readLEUInt32());
            ParentIdx = r.readLEUInt16();
            r.skipBytes(2) // Unused
            Density = r.readLESingle();
            Clustering = r.readByte();
            MinSlope = r.readByte();
            MaxSlope = r.readByte();
            Flags = r.readByte();
            RadiusWrtParent = r.readLEUInt16();
            Radius = r.readLEUInt16();
            MinHeight = r.readLESingle();
            MaxHeight = r.readLESingle();
            Sink = r.readLESingle();
            SinkVariance = r.readLESingle();
            SizeVariance = r.readLESingle();
            AngleVariance = Vector3Int(r.readLEUInt16(), r.readLEUInt16(), r.readLEUInt16());
            r.skipBytes(2) // Unused
            VertexShading = ColorRef(r);
        }
    }

    public struct RDGSField
    {
        public override string ToString() => $"{Grass}";
        public FormId<GRASRecord> Grass;

        public RDGSField(UnityBinaryReader r, uint dataSize)
        {
            Grass = FormId<GRASRecord>(r.readLEUInt32());
            r.skipBytes(4) // Unused
        }
    }

    public struct RDSDField
    {
        public override string ToString() => $"{Sound}";
        public FormId<SOUNRecord> Sound;
        public uint Flags;
        public uint Chance;

        public RDSDField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                Sound = FormId<SOUNRecord>(r.readASCIIString(32, ASCIIFormat.ZeroPadded));
                Flags = 0;
                Chance = r.readByte();
                return;
            }
            Sound = FormId<SOUNRecord>(r.readLEUInt32());
            Flags = r.readLEUInt32();
            Chance = r.readLEUInt32() //: float with TES5
        }
    }

    public struct RDWTField
    {
        public override string ToString() => $"{Weather}";
        public FormId<WTHRRecord> Weather;
        public uint Chance;
        public static byte SizeOf(GameFormatId formatId) => formatId == GameFormatId.TES4 ? (byte)8 : (byte)12;
        public FormId<GLOBRecord> Global;

        public RDWTField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            Weather = FormId<WTHRRecord>(r.readLEUInt32());
            Chance = r.readLEUInt32();
            Global = formatId == GameFormatId.TES5 ? FormId<GLOBRecord>(r.readLEUInt32()) : FormId<GLOBRecord>();
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

        public WEATField(UnityBinaryReader r, uint dataSize)
        {
            Clear = r.readByte();
            Cloudy = r.readByte();
            Foggy = r.readByte();
            Overcast = r.readByte();
            Rain = r.readByte();
            Thunder = r.readByte();
            Ash = r.readByte();
            Blight = r.readByte();
            // v1.3 ESM files add 2 bytes to WEAT subrecords.
            if (dataSize == 10)
                r.skipBytes(2);
        }
    }

    // TES4
    public class RPLIField
    {
        public uint EdgeFalloff // (World Units)
        public Vector2[] Points // Region Point List Data

        public RPLIField(UnityBinaryReader r, uint dataSize)
        {
            EdgeFalloff = r.readLEUInt32();
        }

        public void RPLDField(UnityBinaryReader r, uint dataSize)
        {
            Points = Vector2[dataSize >> 3];
            for (var i = 0; i < Points.Length; i++)
                Points[i] = Vector2(r.readLESingle(), r.readLESingle());
        }
    }

    public var description: String { return "REGN: \(EDID)" }
    public STRVField EDID  // Editor ID
    public STRVField ICON // Icon / Sleep creature
    public FMIDField<WRLDRecord> WNAM // Worldspace - Region name
    public CREFField RCLR // Map Color (COLORREF)
    public List<RDATField> RDATs = List<RDATField>() // Region Data Entries / TES3: Sound Record (order determines the sound priority)
    // TES3
    public WEATField? WEAT // Weather Data
    // TES4
    public List<RPLIField> RPLIs = List<RPLIField>() // Region Areas

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "WNAM",
             "FNAM": WNAM = FMIDField<WRLDRecord>(r, dataSize)
        case "WEAT": WEAT = WEATField(r, dataSize)
        case "ICON",
             "BNAM": ICON = STRVField(r, dataSize)
        case "RCLR",
             "CNAM": RCLR = CREFField(r, dataSize)
        case "SNAM": RDATs.append(RDATField(RDSDs: [RDSDField(r, dataSize, format)]))
        case "RPLI": RPLIs.append(RPLIField(r, dataSize))
        case "RPLD": RPLIs.last!.RPLDField(r, dataSize)
        case "RDAT": RDATs.append(RDATField(r, dataSize))
        case "RDOT": var rdot = RDATs.last!.RDOTs = [RDOTField](); rdot.reserveCapactiy(dataSize / 52); for i in 0..<rdot.capacity { rdot[i] = RDOTField(r, dataSize) }
        case "RDMP": RDATs.last!.RDMP = STRVField(r, dataSize)
        case "RDGS": var rdgs = RDATs.last!.RDGSs = [RDGSField](); rdgs.reserveCapacity(dataSize / 8); for i in 0..<rdgs.capacity { rdgs[i] = RDGSField(r, dataSize) }
        case "RDMD": RDATs.last!.RDMD = UI32Field(r, dataSize)
        case "RDSD": var rdsd = RDATs.last!.RDSDs = [RDSDField](); rdsd.reserveCapacity(dataSize / 12); for i in 0..<rdsd.capacity { rdsd[i] = RDSDField(r, dataSize, format) }
        case "RDWT": var rdwt = RDATs.last!.RDWTs = [RDWTField](); rdwt.reserveCapactiy(dataSize / RDWTField.SizeOf(format)); for i in 0..<rdwt.capacity { rdwt[i] = RDWTField(r, dataSize, format) }
        default: return false
        }
        return true
    }
}