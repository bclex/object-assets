//using OA.Core.Input;
//using UnityEngine;

//namespace OA.Core.Extensions
//{
//    static class MouseStateExtensions
//    {
//        public static MouseState CreateWithDPI(this MouseState value, Vector2 dpi)
//        {
//            var x = (int)(value.X / dpi.X);
//            var y = (int)(value.Y / dpi.Y);
//            var state = new MouseState(x, y, value.ScrollWheelValue, 
//                value.LeftButton, value.MiddleButton, value.RightButton, 
//                value.XButton1, value.XButton2);
//            return state;
//        }
//    }
//}
