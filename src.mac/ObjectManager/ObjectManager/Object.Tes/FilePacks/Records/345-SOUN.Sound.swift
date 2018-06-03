//
//  SOUNRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SOUNRecord: Record, IHaveEDID {
    [Flags]
    public enum SOUNFlags : ushort
    {
        RandomFrequencyShift = 0x0001,
        PlayAtRandom = 0x0002,
        EnvironmentIgnored = 0x0004,
        RandomLocation = 0x0008,
        Loop = 0x0010,
        MenuSound = 0x0020,
        _2D = 0x0040,
        _360LFE = 0x0080,
    }

    // TESX
    public class DATAField
    {
        public byte Volume; // (0=0.00, 255=1.00)
        public byte MinRange; // Minimum attenuation distance
        public byte MaxRange; // Maximum attenuation distance
        // Tes4
        public sbyte FrequencyAdjustment; // Frequency adjustment %
        public ushort Flags; // Flags
        public ushort StaticAttenuation; // Static Attenuation (db)
        public byte StopTime; // Stop time
        public byte StartTime; // Start time

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            Volume = formatId == GameFormatId.TES3 ? r.ReadByte() : (byte)0;
            MinRange = r.ReadByte();
            MaxRange = r.ReadByte();
            if (formatId == GameFormatId.TES3)
                return;
            FrequencyAdjustment = (sbyte)r.ReadByte();
            r.ReadByte(); // Unused
            Flags = r.ReadLEUInt16();
            r.ReadLEUInt16(); // Unused
            if (dataSize == 8)
                return;
            StaticAttenuation = r.ReadLEUInt16();
            StopTime = r.ReadByte();
            StartTime = r.ReadByte();
        }
    }

    public var description: String { return "SOUN: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public FILEField FNAM; // Sound Filename (relative to Sounds\)
    public DATAField DATA; // Sound Data

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "FNAM": FNAM = FILEField(r, dataSize)
        case "SNDX": DATA = DATAField(r, dataSize, format)
        case "SNDD": DATA = DATAField(r, dataSize, format)
        case "DATA": DATA = DATAField(r, dataSize, format)
        default: return false
        }
        return true
    }
}
