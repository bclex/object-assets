using OA.Core.Windows;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Core.Input
{
    public class WndProcInputService : IInputService
    {
        const float DoubleClickMS = 400f; // Settings.UserInterface.Mouse.DoubleClickMS

        WndProc _wndProc;
        List<InputEvent> _events = new List<InputEvent>();
        List<InputEvent> _eventsNext = new List<InputEvent>();
        MouseState _mouseState;
        MouseState _mouseStateLast;

        // Mouse dragging support
        const int MouseDragBeginDistance = 2;
        const int MouseClickMaxDelta = 2;
        InputEventMouse _lastMouseClick;
        float _lastMouseClickTime;
        InputEventMouse _lastMouseDown;
        float _lastMouseDownTime;
        bool _mouseIsDragging;
        float _theTime = -1f;

        public WndProcInputService(IntPtr handle)
        {
            _wndProc = new WndProc(handle);
            _wndProc.MouseWheel += AddEvent;
            _wndProc.MouseMove += OnMouseMove;
            _wndProc.MouseUp += OnMouseUp;
            _wndProc.MouseDown += OnMouseDown;
            _wndProc.KeyDown += OnKeyDown;
            _wndProc.KeyUp += OnKeyUp;
            _wndProc.KeyChar += OnKeyChar;
        }

        public WndProcInputService()
        {
        }

        public void Dispose()
        {
            _wndProc.MouseWheel -= AddEvent;
            _wndProc.MouseMove -= OnMouseMove;
            _wndProc.MouseUp -= OnMouseUp;
            _wndProc.MouseDown -= OnMouseDown;
            _wndProc.KeyDown -= OnKeyDown;
            _wndProc.KeyUp -= OnKeyUp;
            _wndProc.KeyChar -= OnKeyChar;
            _wndProc.Dispose();
            _wndProc = null;
        }

        public void Update(double totalTime, double frameTime)
        {
            _theTime = (float)totalTime;
            _mouseStateLast = _mouseState;
            _mouseState = MouseState.CreateWithDPI(DpiManager.GetSystemDpiScalar());
            lock (_eventsNext)
            {
                _events.Clear();
                foreach (var e in _eventsNext)
                    _events.Add(e);
                _eventsNext.Clear();
            }
        }

        public bool IsCtrlDown => NativeMethods.GetKeyState((int)WinKeys.ControlKey) < 0;

        public bool IsShiftDown => NativeMethods.GetKeyState((int)WinKeys.ShiftKey) < 0;

        public bool IsKeyDown(WinKeys key) => NativeMethods.GetKeyState((int)key) < 0;

        public Vector2Int MousePosition => new Vector2Int(_mouseState.X, _mouseState.Y);

        public bool IsMouseButtonDown(MouseButton btn)
        {
            switch (btn)
            {
                case MouseButton.Left: return _mouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle: return _mouseState.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right: return _mouseState.RightButton == ButtonState.Pressed;
                case MouseButton.XButton1: return _mouseState.XButton1 == ButtonState.Pressed;
                case MouseButton.XButton2: return _mouseState.XButton2 == ButtonState.Pressed;
                default: return false;
            }
        }

        InputEventKeyboard LastKeyPressEvent
        {
            get
            {
                for (var i = _eventsNext.Count; i >= 0; i--)
                    if ((_eventsNext[i - 1] as InputEventKeyboard)?.EventType == KeyboardEvent.Press)
                        return _eventsNext[i - 1] as InputEventKeyboard;
                return null;
            }
        }

        public List<InputEventKeyboard> GetKeyboardEvents()
        {
            var list = new List<InputEventKeyboard>();
            foreach (var e in _events)
                if (!e.Handled && e is InputEventKeyboard)
                    list.Add(e as InputEventKeyboard);
            return list;
        }

        public List<InputEventMouse> GetMouseEvents()
        {
            var list = new List<InputEventMouse>();
            foreach (var e in _events)
                if (!e.Handled && e is InputEventMouse)
                    list.Add(e as InputEventMouse);
            return list;
        }

        public bool HandleKeyboardEvent(KeyboardEvent type, WinKeys key, bool shift, bool alt, bool ctrl)
        {
            foreach (var e in _events)
                if (!e.Handled && e is InputEventKeyboard)
                {
                    var ek = e as InputEventKeyboard;
                    if (ek.EventType == type && ek.KeyCode == key && ek.Shift == shift && ek.Alt == alt && ek.Control == ctrl)
                    {
                        e.Handled = true;
                        return true;
                    }
                }
            return false;
        }

        public bool HandleMouseEvent(MouseEvent type, MouseButton mb)
        {
            foreach (var e in _events)
                if (!e.Handled && e is InputEventMouse)
                {
                    var em = (InputEventMouse)e;
                    if (em.EventType == type && em.Button == mb)
                    {
                        e.Handled = true;
                        return true;
                    }
                }
            return false;
        }

        void OnMouseDown(InputEventMouse e)
        {
            _lastMouseDown = e;
            _lastMouseDownTime = _theTime;
            AddEvent(_lastMouseDown);
        }

        void OnMouseUp(InputEventMouse e)
        {
            if (_mouseIsDragging)
            {
                AddEvent(new InputEventMouse(MouseEvent.DragEnd, e));
                _mouseIsDragging = false;
            }
            else if (_lastMouseDown != null && !DistanceBetweenPoints(_lastMouseDown.Position, e.Position, MouseClickMaxDelta))
            {
                AddEvent(new InputEventMouse(MouseEvent.Click, e));
                if ((_theTime - _lastMouseClickTime <= DoubleClickMS) && !DistanceBetweenPoints(_lastMouseClick.Position, e.Position, MouseClickMaxDelta))
                {
                    _lastMouseClickTime = 0f;
                    AddEvent(new InputEventMouse(MouseEvent.DoubleClick, e));
                }
                else
                {
                    _lastMouseClickTime = _theTime;
                    _lastMouseClick = e;
                }
            }
            AddEvent(new InputEventMouse(MouseEvent.Up, e));
            _lastMouseDown = null;
        }

        void OnMouseMove(InputEventMouse e)
        {
            AddEvent(new InputEventMouse(MouseEvent.Move, e));
            if (!_mouseIsDragging && _lastMouseDown != null)
                if (DistanceBetweenPoints(_lastMouseDown.Position, e.Position, MouseDragBeginDistance))
                {
                    AddEvent(new InputEventMouse(MouseEvent.DragBegin, e));
                    _mouseIsDragging = true;
                }
        }

        void OnKeyDown(InputEventKeyboard e)
        {
            if (e.DataPreviousState == 0)
                AddEvent(new InputEventKeyboard(KeyboardEvent.Down, e));
            for (var i = 0; i < e.DataRepeatCount; i++)
                AddEvent(new InputEventKeyboard(KeyboardEvent.Press, e));
        }

        void OnKeyUp(InputEventKeyboard e)
        {
            AddEvent(new InputEventKeyboard(KeyboardEvent.Up, e));
        }

        void OnKeyChar(InputEventKeyboard e)
        {
            // Control key sends a strange wm_char message ...
            if (e.Control && !e.Alt)
                return;
            var ek = LastKeyPressEvent;
            if (ek == null) Utils.Warning("No corresponding KeyPress event for a WM_CHAR message.");
            else ek.OverrideKeyChar(e.KeyCode);
        }

        void AddEvent(InputEvent e)
        {
            _eventsNext.Add(e);
        }

        bool DistanceBetweenPoints(Vector2Int initial, Vector2Int final, int distance)
        {
            if (Math.Abs(final.x - initial.x) + Math.Abs(final.y - initial.y) > distance)
                return true;
            return false;
        }
    }
}