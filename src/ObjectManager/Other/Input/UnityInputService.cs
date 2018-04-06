using OA.Core.Windows;
using OA.XR;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Core.Input
{
    public class UnityInputService : IInputService
    {
        public bool IsCtrlDown => NativeMethods.GetKeyState((int)WinKeys.ControlKey) < 0;

        public bool IsShiftDown => NativeMethods.GetKeyState((int)WinKeys.ShiftKey) < 0;

        public bool IsKeyDown(WinKeys key) => NativeMethods.GetKeyState((int)key) < 0;

        public Vector2Int MousePosition => new Vector2Int((int)UnityEngine.Input.mousePosition.x, (int)UnityEngine.Input.mousePosition.y);

        public List<InputEventKeyboard> GetKeyboardEvents()
        {
            return null;
        }

        public List<InputEventMouse> GetMouseEvents()
        {
            return null;
        }

        public bool HandleKeyboardEvent(KeyboardEvent type, WinKeys key, bool shift, bool alt, bool ctrl)
        {
            return true;
        }

        public bool HandleMouseEvent(MouseEvent type, MouseButton mb)
        {
            return true;
        }

        public bool IsMouseButtonDown(MouseButton btn)
        {
            return false;
        }

        public void Update(double totalTime, double frameTime)
        {
        }
    }
}