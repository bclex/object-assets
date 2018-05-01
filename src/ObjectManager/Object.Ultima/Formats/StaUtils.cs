using OA.Core;
using UnityEngine;

namespace OA.Ultima.Formats
{
    public static class StaUtils
    {
        public static Vector3 StaVectorToUnityVector(Vector3 staVector)
        {
            Utils.Swap(ref staVector.y, ref staVector.z);
            return staVector;
        }

        public static Vector3 StaPointToUnityPoint(Vector3 staPoint)
        {
            return staPoint * ConvertUtils.MeterInUnits;
        }

        public static Matrix4x4 StaRotationMatrixToUnityRotationMatrix(Matrix4x4 staRotationMatrix)
        {
            var matrix = new Matrix4x4();
            matrix.m00 = staRotationMatrix.m00;
            matrix.m01 = staRotationMatrix.m02;
            matrix.m02 = staRotationMatrix.m01;
            matrix.m03 = 0;
            matrix.m10 = staRotationMatrix.m20;
            matrix.m11 = staRotationMatrix.m22;
            matrix.m12 = staRotationMatrix.m21;
            matrix.m13 = 0;
            matrix.m20 = staRotationMatrix.m10;
            matrix.m21 = staRotationMatrix.m12;
            matrix.m22 = staRotationMatrix.m11;
            matrix.m23 = 0;
            matrix.m30 = 0;
            matrix.m31 = 0;
            matrix.m32 = 0;
            matrix.m33 = 1;
            return matrix;
        }

        public static Quaternion StaRotationMatrixToUnityQuaternion(Matrix4x4 staRotationMatrix)
        {
            return ConvertUtils.RotationMatrixToQuaternion(StaRotationMatrixToUnityRotationMatrix(staRotationMatrix));
        }

        public static Quaternion StaEulerAnglesToUnityQuaternion(Vector3 staEulerAngles)
        {
            var eulerAngles = StaVectorToUnityVector(staEulerAngles);
            var xRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.x, Vector3.right);
            var yRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.y, Vector3.up);
            var zRot = Quaternion.AngleAxis(Mathf.Rad2Deg * eulerAngles.z, Vector3.forward);
            return xRot * zRot * yRot;
        }
    }
}