//
//  NifReader.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import simd

public class NifUtils {
    
    public static func nifVectorToUnityVector(_ vector: Float3) -> Float3 {
        var unity = vector
        let z = unity.z
        unity.z = unity.y
        unity.y = z
        return unity
    }
    public static func nifVectorToUnityVector(_ vector: float3) -> float3 {
        var unity = vector
        let z = unity.z
        unity.z = unity.y
        unity.y = z
        return unity
    }
    
    public static func nifPointToUnityPoint(_ point: Float3) -> Float3 {
        return nifVectorToUnityVector(point) / ConvertUtils.meterInUnits
    }
    public static func nifPointToUnityPoint(_ point: float3) -> float3 {
        return nifVectorToUnityVector(point) / ConvertUtils.meterInUnits
    }

    public static func nifRotationMatrixToUnityRotationMatrix(_ rotationMatrix: simd_float4x4) -> simd_float4x4 {
        return simd_float4x4(
            float4(rotationMatrix[0].x, rotationMatrix[0].z, rotationMatrix[0].y, 0),
            float4(rotationMatrix[2].x, rotationMatrix[2].z, rotationMatrix[2].y, 0),
            float4(rotationMatrix[1].x, rotationMatrix[1].z, rotationMatrix[1].y, 0),
            float4(0, 0, 0, 1))
    }

    public static func nifRotationMatrixToUnityQuaternion(_ rotationMatrix: simd_float4x4) -> simd_quatf {
        return simd_quatf(nifRotationMatrixToUnityRotationMatrix(rotationMatrix))
    }

    public static func nifEulerAnglesToUnityQuaternion(_ nifEulerAngles: float3) -> simd_quatf {
        let eulerAngles2 = nifVectorToUnityVector(nifEulerAngles)
        let xRot = simd_quatf(angle: rad2Deg * eulerAngles2.x, axis: float3.right)
        let yRot = simd_quatf(angle: rad2Deg * eulerAngles2.y, axis: float3.up)
        let zRot = simd_quatf(angle: rad2Deg * eulerAngles2.z, axis: float3.forward)
        return xRot * zRot * yRot
    }
}
