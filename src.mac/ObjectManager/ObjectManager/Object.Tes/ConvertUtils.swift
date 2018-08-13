//
//  ConvertUtils.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import simd

public class ConvertUtils {
    static let yardInUnits = 64
    static let meterInYards: Float = 1.09361
    public static let meterInUnits: Float = meterInYards * Float(yardInUnits)
    static let exteriorCellSideLengthInUnits = 128 * yardInUnits
    public static let exteriorCellSideLengthInMeters = Float(exteriorCellSideLengthInUnits) / meterInUnits
}
