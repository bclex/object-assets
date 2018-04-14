using OA.Core.Windows;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Core.Input
{
    public interface IInputService
    {
        bool IsCtrlDown { get; }
        bool IsShiftDown { get; }
        bool IsKeyDown(WinKeys key);
        Vector2Int MousePosition { get; }
        bool IsMouseButtonDown(MouseButton btn);
        List<InputEventKeyboard> GetKeyboardEvents();
        List<InputEventMouse> GetMouseEvents();
        bool HandleKeyboardEvent(KeyboardEvent type, WinKeys key, bool shift, bool alt, bool ctrl);
        bool HandleMouseEvent(MouseEvent type, MouseButton mb);
        void Update(double totalTime, double frameTime);
    }
}
