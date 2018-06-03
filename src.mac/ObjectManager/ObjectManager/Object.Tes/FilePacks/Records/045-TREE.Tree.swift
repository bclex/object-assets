//
//  TREERecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class TREERecord: Record {
    public struct SNAMField
    {
        public int[] Values;

        public SNAMField(UnityBinaryReader r, uint dataSize)
        {
            Values = new int[dataSize >> 2];
            for (var i = 0; i < Values.Length; i++)
                Values[i] = r.ReadLEInt32();
        }
    }

    public struct CNAMField
    {
        public float LeafCurvature;
        public float MinimumLeafAngle;
        public float MaximumLeafAngle;
        public float BranchDimmingValue;
        public float LeafDimmingValue;
        public int ShadowRadius;
        public float RockSpeed;
        public float RustleSpeed;

        public CNAMField(UnityBinaryReader r, uint dataSize)
        {
            LeafCurvature = r.ReadLESingle();
            MinimumLeafAngle = r.ReadLESingle();
            MaximumLeafAngle = r.ReadLESingle();
            BranchDimmingValue = r.ReadLESingle();
            LeafDimmingValue = r.ReadLESingle();
            ShadowRadius = r.ReadLEInt32();
            RockSpeed = r.ReadLESingle();
            RustleSpeed = r.ReadLESingle();
        }
    }

    public struct BNAMField
    {
        public float Width;
        public float Height;

        public BNAMField(UnityBinaryReader r, uint dataSize)
        {
            Width = r.ReadLESingle();
            Height = r.ReadLESingle();
        }
    }

    public var description: String { return "TREE: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL; // Model
    public FILEField ICON; // Leaf Texture
    public SNAMField SNAM; // SpeedTree Seeds, array of ints
    public CNAMField CNAM; // Tree Parameters
    public BNAMField BNAM; // Billboard Dimensions

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "ICON": ICON = FILEField(r, dataSize)
        case "SNAM": SNAM = SNAMField(r, dataSize)
        case "CNAM": CNAM = CNAMField(r, dataSize)
        case "BNAM": BNAM = BNAMField(r, dataSize)
        default: return false
        }
        return true
    }
}
