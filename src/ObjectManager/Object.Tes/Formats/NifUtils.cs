using OA.Core;
using UnityEngine;

namespace OA.Tes.Formats
{
    public static class NifUtils
    {
        public static Vector3 NifVectorToUnityVector(Vector3 NIFVector)
        {
            Utils.Swap(ref NIFVector.y, ref NIFVector.z);
            return NIFVector;
        }

        public static Vector3 NifPointToUnityPoint(Vector3 NIFPoint)
        {
            return NifVectorToUnityVector(NIFPoint) / ConvertUtils.MeterInUnits;
        }

        public static Matrix4x4 NifRotationMatrixToUnityRotationMatrix(Matrix4x4 NIFRotationMatrix)
        {
            var matrix = new Matrix4x4
            {
                m00 = NIFRotationMatrix.m00,
                m01 = NIFRotationMatrix.m02,
                m02 = NIFRotationMatrix.m01,
                m03 = 0,
                m10 = NIFRotationMatrix.m20,
                m11 = NIFRotationMatrix.m22,
                m12 = NIFRotationMatrix.m21,
                m13 = 0,
                m20 = NIFRotationMatrix.m10,
                m21 = NIFRotationMatrix.m12,
                m22 = NIFRotationMatrix.m11,
                m23 = 0,
                m30 = 0,
                m31 = 0,
                m32 = 0,
                m33 = 1
            };
            return matrix;
        }

        public static Quaternion NifRotationMatrixToUnityQuaternion(Matrix4x4 NIFRotationMatrix)
        {
            return ConvertUtils.RotationMatrixToQuaternion(NifRotationMatrixToUnityRotationMatrix(NIFRotationMatrix));
        }

        public static Quaternion NifEulerAnglesToUnityQuaternion(Vector3 NifEulerAngles)
        {
            var eulerAngles = NifVectorToUnityVector(NifEulerAngles);
            var xRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.x, Vector3.right);
            var yRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.y, Vector3.up);
            var zRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.z, Vector3.forward);
            return xRot * zRot * yRot;
        }
    }
}