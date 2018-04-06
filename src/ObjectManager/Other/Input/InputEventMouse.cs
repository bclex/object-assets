using OA.Core.Windows;
using UnityEngine;

namespace OA.Core.Input
{
    public class InputEventMouse : InputEvent
    {
        const int WHEEL_DELTA = 120;

        public readonly MouseEvent EventType;
        public readonly int X;
        public readonly int Y;
        public int WheelValue => _clicks / WHEEL_DELTA;
        public Vector2Int Position => new Vector2Int(X, Y);

        readonly WinMouseButtons _buttons;
        readonly int _clicks;
        readonly int _mouseData;

        public MouseButton Button
        {
            get
            {
                if ((_buttons & WinMouseButtons.Left) == WinMouseButtons.Left)
                    return MouseButton.Left;
                if ((_buttons & WinMouseButtons.Right) == WinMouseButtons.Right)
                    return MouseButton.Right;
                if ((_buttons & WinMouseButtons.Middle) == WinMouseButtons.Middle)
                    return MouseButton.Middle;
                if ((_buttons & WinMouseButtons.XButton1) == WinMouseButtons.XButton1)
                    return MouseButton.XButton1;
                if ((_buttons & WinMouseButtons.XButton2) == WinMouseButtons.XButton2)
                    return MouseButton.XButton2;
                return MouseButton.None;
            }
        }

        public InputEventMouse(MouseEvent type, WinMouseButtons btn, int clicks, int x, int y, int data, WinKeys modifiers)
            : base(modifiers)
        {
            var dpi = DpiManager.GetSystemDpiScalar();
            EventType = type;
            _buttons = btn;
            _clicks = clicks;
            X = (int)(x / dpi.x);
            Y = (int)(y / dpi.y);
            _mouseData = data;
        }

        public InputEventMouse(MouseEvent eventType, InputEventMouse parent)
            : base(parent)
        {
            EventType = eventType;
            _buttons = parent._buttons;
            _clicks = parent._clicks;
            X = parent.X;
            Y = parent.Y;
            _mouseData = parent._mouseData;
        }
    }
}
