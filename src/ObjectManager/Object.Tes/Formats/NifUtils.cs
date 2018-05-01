﻿using OA.Core;
using UnityEngine;

namespace OA.Tes.Formats
{
    public static class NifUtils
    {
        public static Vector3 NifVectorToUnityVector(Vector3 nifVector)
        {
            Utils.Swap(ref nifVector.y, ref nifVector.z);
            return nifVector;
        }

        public static Vector3 NifPointToUnityPoint(Vector3 vifPoint)
        {
            return NifVectorToUnityVector(vifPoint) / ConvertUtils.MeterInUnits;
        }

        public static Matrix4x4 NifRotationMatrixToUnityRotationMatrix(Matrix4x4 nifRotationMatrix)
        {
            var matrix = new Matrix4x4
            {
                m00 = nifRotationMatrix.m00,
                m01 = nifRotationMatrix.m02,
                m02 = nifRotationMatrix.m01,
                m03 = 0,
                m10 = nifRotationMatrix.m20,
                m11 = nifRotationMatrix.m22,
                m12 = nifRotationMatrix.m21,
                m13 = 0,
                m20 = nifRotationMatrix.m10,
                m21 = nifRotationMatrix.m12,
                m22 = nifRotationMatrix.m11,
                m23 = 0,
                m30 = 0,
                m31 = 0,
                m32 = 0,
                m33 = 1
            };
            return matrix;
        }

        public static Quaternion NifRotationMatrixToUnityQuaternion(Matrix4x4 nifRotationMatrix)
        {
            return ConvertUtils.RotationMatrixToQuaternion(NifRotationMatrixToUnityRotationMatrix(nifRotationMatrix));
        }

        public static Quaternion NifEulerAnglesToUnityQuaternion(Vector3 nifEulerAngles)
        {
            var eulerAngles = NifVectorToUnityVector(nifEulerAngles);
            var xRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.x, Vector3.right);
            var yRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.y, Vector3.up);
            var zRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.z, Vector3.forward);
            return xRot * zRot * yRot;
        }
    }
}