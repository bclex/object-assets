using UnityEngine;

namespace OA.Core.Extensions
{
    public static class PointExtensions
    {
        public static Vector2Int DivideBy(this Vector2Int value, int divisor)
        {
            value.x /= divisor;
            value.y /= divisor;
            return value;
        }
    }
}
