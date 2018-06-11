//
//  NifReader.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public static class NifUtils {
    public static func nifVectorToUnityVector(_ vector: Vector3) -> Vector3 {
        Utils.Swap(&vector.y, &vector.z)
        return vector
    }

    public static func nifPointToUnityPoint(_ point: Vector3) -> Vector3 {
        return nifVectorToUnityVector(point) / ConvertUtils.meterInUnits
    }

    public static nifRotationMatrixToUnityRotationMatrix(rotationMatrix: Matrix4x4) -> Matrix4x4 {
        return Matrix4x4(
            m11: rotationMatrix.m00,
            m12: rotationMatrix.m02,
            m13: rotationMatrix.m01,
            m14: 0,
            m21: rotationMatrix.m20,
            m22: rotationMatrix.m22,
            m23: rotationMatrix.m21,
            m24: 0,
            m31: rotationMatrix.m10,
            m32: rotationMatrix.m12,
            m33: rotationMatrix.m11,
            m34: 0,
            m41: 0,
            m42: 0,
            m43: 0,
            m44: 1
        )
    }

    public static func NifRotationMatrixToUnityQuaternion(_ rotationMatrix: Matrix4x4) -> Quaternion {
        return ConvertUtils.rotationMatrixToQuaternion(nifRotationMatrixToUnityRotationMatrix(rotationMatrix))
    }

    public static func nifEulerAnglesToUnityQuaternion(_ nifEulerAngles: Vector3) -> Quaternion {
        let eulerAngles2 = nifVectorToUnityVector(nifEulerAngles)
        let xRot = Quaternion.angleAxis(Float.rad2Deg * eulerAngles2.x, Vector3.right)
        let yRot = Quaternion.angleAxis(Float.rad2Deg * eulerAngles2.y, Vector3.up)
        let zRot = Quaternion.angleAxis(Float.rad2Deg * eulerAngles2.z, Vector3.forward)
        return xRot * zRot * yRot
    }
}