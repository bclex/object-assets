//
//  ConvertUtils.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ConvertUtils {
    let yardInUnits = 64
    let meterInYards = 1.09361
    public let meterInUnits = meterInYards * yardInUnits
    let exteriorCellSideLengthInUnits = 128 * yardInUnits
    public let exteriorCellSideLengthInMeters = Float(exteriorCellSideLengthInUnits) / MeterInUnits

    public static func rotationMatrixToQuaternion(_ matrix: Matrix4x4) -> Quaternion{
        return Quaternion.lookRotation(matrix.getColumn(2), matrix.getColumn(1))
    }
}