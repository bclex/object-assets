using System;
using UnityEngine;

namespace OA.Core.Input
{
    public class MouseState
    {
        public ButtonState LeftButton { get; set; }
        public ButtonState MiddleButton { get; set; }
        public ButtonState RightButton { get; set; }
        public ButtonState XButton1 { get; set; }
        public ButtonState XButton2 { get; set; }
        public int X { get; }
        public int Y { get; }

        public static MouseState CreateWithDPI(Vector2 vector2)
        {
            return new MouseState();
        }
    }
}
