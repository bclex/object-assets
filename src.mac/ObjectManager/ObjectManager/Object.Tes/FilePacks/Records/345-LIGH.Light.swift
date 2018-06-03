//
//  LIGHRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LIGHRecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public struct DATAField
    {
        public enum ColorFlags
        {
            Dynamic = 0x0001,
            CanCarry = 0x0002,
            Negative = 0x0004,
            Flicker = 0x0008,
            Fire = 0x0010,
            OffDefault = 0x0020,
            FlickerSlow = 0x0040,
            Pulse = 0x0080,
            PulseSlow = 0x0100
        }

        public float Weight;
        public int Value;
        public int Time;
        public int Radius;
        public ColorRef LightColor;
        public int Flags;
        // TES4
        public float FalloffExponent;
        public float FOV;

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                Weight = r.readLESingle();
                Value = r.readLEInt32();
                Time = r.readLEInt32();
                Radius = r.readLEInt32();
                LightColor = ColorRef(r);
                Flags = r.readLEInt32();
                FalloffExponent = 1;
                FOV = 90;
                return;
            }

            Time = r.readLEInt32();
            Radius = r.readLEInt32();
            LightColor = ColorRef(r);
            Flags = r.readLEInt32();
            if (dataSize == 32)
            {
                FalloffExponent = r.readLESingle();
                FOV = r.readLESingle();
            }
            else
            {
                FalloffExponent = 1;
                FOV = 90;
            }
            Value = r.readLEInt32();
            Weight = r.readLESingle();
        }
    }

    public var description: String { return "LIGH: \(EDID)" }
    public STRVField EDID  // Editor ID
    public MODLGroup MODL  // Model
    public STRVField? FULL // Item Name (optional)
    public DATAField DATA // Light Data
    public STRVField? SCPT // Script Name (optional)??
    public FMIDField<SCPTRecord>? SCRI // Script FormId (optional)
    public FILEField? ICON // Male Icon (optional)
    public FLTVField FNAM // Fade Value
    public FMIDField<SOUNRecord> SNAM // Sound FormId (optional)

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "FULL": FULL = STRVField(r, dataSize)
        case "FNAM": if format != .TES3 { FNAM = FLTVField(r, dataSize) } else { FULL = STRVField(r, dataSize) }
        case "DATA":
        case "LHDT": DATA = DATAField(r, dataSize, format)
        case "SCPT": SCPT = STRVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "SNAM": SNAM = FMIDField<SOUNRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}