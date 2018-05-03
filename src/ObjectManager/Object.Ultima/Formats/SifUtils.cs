using OA.Core;
using UnityEngine;

namespace OA.Ultima.Formats
{
    public static class SifUtils
    {
        public static Vector3 SifVectorToUnityVector(Vector3 vector)
        {
            Utils.Swap(ref vector.y, ref vector.z);
            return vector;
        }

        public static Vector3 SifPointToUnityPoint(Vector3 point)
        {
            return SifVectorToUnityVector(point) * 1; // ConvertUtils.MeterInUnits;
        }

        public static Matrix4x4 SifRotationMatrixToUnityRotationMatrix(Matrix4x4 rotationMatrix)
        {
            var matrix = new Matrix4x4
            {
                m00 = rotationMatrix.m00,
                m01 = rotationMatrix.m02,
                m02 = rotationMatrix.m01,
                m03 = 0,
                m10 = rotationMatrix.m20,
                m11 = rotationMatrix.m22,
                m12 = rotationMatrix.m21,
                m13 = 0,
                m20 = rotationMatrix.m10,
                m21 = rotationMatrix.m12,
                m22 = rotationMatrix.m11,
                m23 = 0,
                m30 = 0,
                m31 = 0,
                m32 = 0,
                m33 = 1
            };
            return matrix;
        }

        public static Quaternion SifRotationMatrixToUnityQuaternion(Matrix4x4 rotationMatrix)
        {
            return ConvertUtils.RotationMatrixToQuaternion(SifRotationMatrixToUnityRotationMatrix(rotationMatrix));
        }

        public static Quaternion SifEulerAnglesToUnityQuaternion(Vector3 eulerAngles)
        {
            var eulerAngles2 = SifVectorToUnityVector(eulerAngles);
            var xRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles2.x, Vector3.right);
            var yRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles2.y, Vector3.up);
            var zRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles2.z, Vector3.forward);
            return xRot * zRot * yRot;
        }
    }
}