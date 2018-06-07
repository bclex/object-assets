//
//  ROADRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ROADRecord: Record {
    public override var description: String { return "ROAD:" }
    public var PGRPs: [PGRDRecord.PGRPField]
    public var PGRR: UNKNField

    init() {
    }
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "PGRP":
            PGRPs = [PGRDRecord.PGRPField](); PGRPs.reserveCapacity(dataSize >> 4); for i in 0..<PGRPs.capacity { PGRPs[i] = PGRDRecord.PGRPField(r, dataSize) }
        case "PGRR": PGRR = UNKNField(r, dataSize)
        default: return false
        }
        return true
    }
}
