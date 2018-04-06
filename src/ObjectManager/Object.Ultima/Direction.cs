using System;
using UnityEngine;

namespace OA.Ultima
{
    [Flags]
    public enum Direction : byte
    {
        North = 0x0,
        Right = 0x1,
        East = 0x2,
        Down = 0x3,
        South = 0x4,
        Left = 0x5,
        West = 0x6,
        Up = 0x7,
        FacingMask = 0x7,
        Running = 0x80,
        ValueMask = 0x87,
        Nothing = 0xED
    }

    public static class DirectionHelper
    {
        public static Direction DirectionFromPoints(Vector2Int from, Vector2Int to)
        {
            return DirectionFromVectors(new Vector2(from.x, from.y), new Vector2(to.x, to.y));
        }

        public static Direction DirectionFromVectors(Vector2 fromPosition, Vector2 toPosition)
        {
            var Angle = Math.Atan2(toPosition.y - fromPosition.y, toPosition.x - fromPosition.x);
            if (Angle < 0)
                Angle = Math.PI + (Math.PI + Angle);
            var piPerSegment = (Math.PI * 2f) / 8f;
            var segmentValue = (Math.PI * 2f) / 16f;
            var direction = int.MaxValue;
            for (var i = 0; i < 8; i++)
            {
                if (Angle >= segmentValue && Angle <= (segmentValue + piPerSegment))
                {
                    direction = i + 1;
                    break;
                }
                segmentValue += piPerSegment;
            }
            if (direction == int.MaxValue)
                direction = 0;
            direction = direction >= 7 ? direction - 7 : direction + 1;
            return (Direction)direction;
        }

        public static Direction GetCardinal(Direction inDirection)
        {
            return inDirection & (Direction)0x6; // contains bitmasks for 0x0, 0x2, 0x4, and 0x6
        }

        public static Direction Reverse(Direction inDirection)
        {
            return (Direction)((int)inDirection + 0x04) & Direction.FacingMask;
        }
    }
}
