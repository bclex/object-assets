using OA.Core.Input;
using OA.Ultima.Core.Graphics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Core.UI
{
    public class UserInterfaceService
    {
        /// <summary>
        /// An array of all open root controls in the user interface.
        /// </summary>
        public readonly List<AControl> OpenControls;
        public int Width => _spriteBatch.GraphicsDevice.Viewport.Width;
        public int Height => _spriteBatch.GraphicsDevice.Viewport.Height;

        IInputService _input;
        SpriteBatchUI _spriteBatch;
        AControl[] _mouseDownControl = new AControl[5];

        // ============================================================================================================
        // Ctor, Dispose, Update, and Draw
        // ============================================================================================================

        public UserInterfaceService()
        {
            _input = Service.Get<IInputService>();
            _spriteBatch = Service.Get<SpriteBatchUI>();
            OpenControls = new List<AControl>();
        }

        public void Dispose()
        {
            Reset();
        }

        /// <summary>
        /// Disposes of all controls.
        /// </summary>
        public void Reset()
        {
            foreach (var c in OpenControls)
                c.Dispose();
            OpenControls.Clear();
        }

        internal ICursor Cursor { get; set; }

        /// <summary>
        /// Returns the control directly under the Cursor.
        /// </summary>
        public AControl MouseOverControl { get; private set; }

        /// <summary>
        /// Returns True if the Cursor is over the UserInterface.
        /// </summary>
        public bool IsMouseOverUI => MouseOverControl != null;

        AControl _keyboardFocusControl;
        public AControl KeyboardFocusControl
        {
            get
            {
                if (IsModalControlOpen)
                    return null;
                if (_keyboardFocusControl == null)
                    foreach (var c in OpenControls)
                        if (!c.IsDisposed && c.IsVisible && c.IsEnabled && c.HandlesKeyboardFocus)
                        {
                            _keyboardFocusControl = c.FindControlThatAcceptsKeyboardFocus();
                            if (_keyboardFocusControl != null)
                                break;
                        }
                return _keyboardFocusControl;
            }
            set { _keyboardFocusControl = value; }
        }

        public bool IsModalControlOpen
        {
            get
            {
                foreach (var c in OpenControls)
                    if (c.MetaData.IsModal)
                        return true;
                return false;
            }
        }

        /// <summary>
        /// Adds or toggles the passed control to the list of active controls.
        /// If control succesfully added to active control list, returns control. If add unsuccessful, returns null.
        /// </summary>
        public AControl AddControl(AControl control, int x, int y)
        {
            if (control.IsDisposed)
                return null;
            control.Position = new Vector2Int(x, y);
            OpenControls.Insert(0, control);
            return control;
        }

        public void RemoveControl<T>(int? localID = null) where T : AControl
        {
            foreach (var c in OpenControls)
                if (typeof(T).IsAssignableFrom(c.GetType()))
                    if (!localID.HasValue || (c.GumpLocalID == localID))
                        if (!c.IsDisposed)
                            c.Dispose();
        }

        public AControl GetControl(int localID)
        {
            foreach (var c in OpenControls)
                if (!c.IsDisposed && c.GumpLocalID == localID)
                    return c;
            return null;
        }

        public AControl GetControlByTypeID(int typeID)
        {
            foreach (var c in OpenControls)
                if (!c.IsDisposed && c.GumpServerTypeID == typeID)
                    return c;
            return null;
        }

        public T GetControl<T>(int? localID = null) where T : AControl
        {
            foreach (var c in OpenControls)
                if (!c.IsDisposed && c.GetType() == typeof(T) && (!localID.HasValue || c.GumpLocalID == localID))
                    return (T)c;
            return null;
        }

        public void Update(double totalMS, double frameMS)
        {
            OrderControlsBasedOnUILayerMetaData();
            for (var i = 0; i < OpenControls.Count; i++)
            {
                var c = OpenControls[i];
                if (!c.IsInitialized && !c.IsDisposed)
                    c.Initialize();
                c.Update(totalMS, frameMS);
            }
            for (var i = 0; i < OpenControls.Count; i++)
                if (OpenControls[i].IsDisposed)
                {
                    OpenControls.RemoveAt(i);
                    i--;
                }
            if (Cursor != null)
                Cursor.Update();
            InternalHandleKeyboardInput();
            InternalHandleMouseInput();
        }

        public void Draw(double frameMS)
        {
            OrderControlsBasedOnUILayerMetaData();
            _spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Reset();
            foreach (var c in OpenControls.Reverse<AControl>())
                if (c.IsInitialized)
                    c.Draw(_spriteBatch, c.Position, frameMS);
            if (Cursor != null)
                Cursor.Draw(_spriteBatch, _input.MousePosition);
            _spriteBatch.FlushSprites(false);
        }

        void InternalHandleKeyboardInput()
        {
            if (KeyboardFocusControl != null)
            {
                if (_keyboardFocusControl.IsDisposed)
                    _keyboardFocusControl = null;
                else
                {
                    var k_events = _input.GetKeyboardEvents();
                    foreach (var e in k_events)
                        if (e.EventType == KeyboardEvent.Press)
                            _keyboardFocusControl.KeyboardInput(e);
                }
            }
        }

        void OrderControlsBasedOnUILayerMetaData()
        {
            var gumps = new List<AControl>();
            foreach (var c in OpenControls)
                if (c.MetaData.Layer != UILayer.Default)
                    gumps.Add(c);
            foreach (var c in gumps)
                if (c.MetaData.Layer == UILayer.Under)
                {
                    for (var i = 0; i < OpenControls.Count; i++)
                        if (OpenControls[i] == c)
                        {
                            OpenControls.RemoveAt(i);
                            OpenControls.Insert(OpenControls.Count, c);
                        }
                }
                else if (c.MetaData.Layer == UILayer.Over)
                {
                    for (var i = 0; i < OpenControls.Count; i++)
                        if (OpenControls[i] == c)
                        {
                            OpenControls.RemoveAt(i);
                            OpenControls.Insert(0, c);
                        }
                }
        }

        void InternalHandleMouseInput()
        {
            // clip the mouse position
            var clippedPosition = _input.MousePosition;
            ClipMouse(ref clippedPosition);

            // Get the topmost control that is under the mouse and handles mouse input.
            // If this control is different from the previously focused control,
            // send that previous control a MouseOut event.
            var focusedControl = InternalGetMouseOverControl(clippedPosition);
            if (MouseOverControl != null && focusedControl != MouseOverControl)
            {
                MouseOverControl.MouseOut(clippedPosition);
                // Also let the parent control know we've been moused out (for gumps).
                if (MouseOverControl.RootParent != null)
                    if (focusedControl == null || MouseOverControl.RootParent != focusedControl.RootParent)
                        MouseOverControl.RootParent.MouseOut(clippedPosition);
            }

            if (focusedControl != null)
            {
                focusedControl.MouseOver(clippedPosition);
                if (_mouseDownControl[0] == focusedControl)
                    AttemptDragControl(focusedControl, clippedPosition);
                if (_isDraggingControl)
                    DoDragControl(clippedPosition);
            }

            // Set the new MouseOverControl.
            MouseOverControl = focusedControl;

            // Send a MouseOver event to any control that was previously the target of a MouseDown event.
            for (var iButton = 0; iButton < 5; iButton++)
                if (_mouseDownControl[iButton] != null && _mouseDownControl[iButton] != focusedControl)
                    _mouseDownControl[iButton].MouseOver(clippedPosition);

            // The cursor and world input objects occasionally must block input events from reaching the UI:
            // e.g. when the cursor is carrying an object.
            if (IsModalControlOpen == false && ObjectsBlockingInput == true)
                return;
            var events = _input.GetMouseEvents();
            foreach (var e in events)
            {
                // MouseDown event: the currently focused control gets a MouseDown event, and if it handles Keyboard input, gets Keyboard focus as well.
                if (e.EventType == MouseEvent.Down)
                {
                    if (focusedControl != null)
                    {
                        MakeTopMostGump(focusedControl);
                        focusedControl.MouseDown(clippedPosition, e.Button);
                        if (focusedControl.HandlesKeyboardFocus)
                            _keyboardFocusControl = focusedControl;
                        _mouseDownControl[(int)e.Button] = focusedControl;
                    }
                    else
                    {
                        // close modal controls if they can be closed with a mouse down outside their drawn area
                        if (IsModalControlOpen)
                            foreach (var c in OpenControls)
                                if (c.MetaData.IsModal && c.MetaData.ModalClickOutsideAreaClosesThisControl)
                                    c.Dispose();
                    }
                }

                // MouseUp and MouseClick events
                if (e.EventType == MouseEvent.Up)
                {
                    var btn = (int)e.Button;
                    // If there is a currently focused control:
                    // 1.   If the currently focused control is the same control that was MouseDowned on with this button,
                    //      then send that control a MouseClick event.
                    // 2.   Send the currently focused control a MouseUp event.
                    // 3.   If the currently focused control is NOT the same control that was MouseDowned on with this button,
                    //      send that MouseDowned control a MouseUp event (but it does not receive MouseClick).
                    // If there is NOT a currently focused control, then simply inform the control that was MouseDowned on
                    // with this button that the button has been released, by sending it a MouseUp event.
                    EndDragControl(e.Position);
                    if (focusedControl != null)
                    {
                        if (_mouseDownControl[btn] != null && focusedControl == _mouseDownControl[btn])
                            focusedControl.MouseClick(clippedPosition, e.Button);
                        focusedControl.MouseUp(clippedPosition, e.Button);
                        if (_mouseDownControl[btn] != null && focusedControl != _mouseDownControl[btn])
                            _mouseDownControl[btn].MouseUp(clippedPosition, e.Button);
                    }
                    else
                    {
                        if (_mouseDownControl[btn] != null)
                            _mouseDownControl[btn].MouseUp(clippedPosition, e.Button);
                    }
                    _mouseDownControl[btn] = null;
                }
            }
        }

        void MakeTopMostGump(AControl control)
        {
            var c = control;
            while (c.Parent != null)
                c = c.Parent;
            for (var i = 0; i < OpenControls.Count; i++)
                if (OpenControls[i] == c)
                {
                    var cm = OpenControls[i];
                    OpenControls.RemoveAt(i);
                    OpenControls.Insert(0, cm);
                }
        }

        AControl InternalGetMouseOverControl(Vector2Int atPosition)
        {
            if (_isDraggingControl)
                return _draggingControl;
            List<AControl> possibleControls;
            if (IsModalControlOpen)
            {
                possibleControls = new List<AControl>();
                foreach (var c in OpenControls)
                    if (c.MetaData.IsModal)
                        possibleControls.Add(c);
            }
            else possibleControls = OpenControls;

            AControl[] mouseOverControls = null;
            // Get the list of controls under the mouse cursor
            foreach (var c in possibleControls)
            {
                var controls = c.HitTest(atPosition, false);
                if (controls != null)
                {
                    mouseOverControls = controls;
                    break;
                }
            }
            if (mouseOverControls == null)
                return null;
            // Get the topmost control that is under the mouse and handles mouse input.
            // If this control is different from the previously focused control,
            // send that previous control a MouseOut event.
            if (mouseOverControls != null)
                for (var i = 0; i < mouseOverControls.Length; i++)
                    if (mouseOverControls[i].HandlesMouseInput)
                        return mouseOverControls[i];
            return null;
        }

        // ============================================================================================================
        // Input blocking objects
        // ============================================================================================================

        List<object> _inputBlockingObjects = new List<object>();

        /// <summary>
        /// Returns true if there are any active objects blocking input.
        /// </summary>
        bool ObjectsBlockingInput => _inputBlockingObjects.Count > 0;

        /// <summary>
        /// Add an input blocking object. Until RemoveInputBlocker is called with this same parameter,
        /// GUIState will not process any MouseDown, MouseUp, or MouseClick events, or any keyboard events.
        /// </summary>
        public void AddInputBlocker(object obj)
        {
            if (!_inputBlockingObjects.Contains(obj))
                _inputBlockingObjects.Add(obj);
        }

        /// <summary>
        /// Removes an input blocking object. Only when there are no input blocking objects will GUIState
        /// process MouseDown, MouseUp, MouseClick, and all keyboard events.
        /// </summary>
        public void RemoveInputBlocker(object obj)
        {
            if (_inputBlockingObjects.Contains(obj))
                _inputBlockingObjects.Remove(obj);
        }

        // ============================================================================================================
        // Control dragging
        // ============================================================================================================

        AControl _draggingControl;
        bool _isDraggingControl;
        int _dragOriginX;
        int _dragOriginY;

        public void AttemptDragControl(AControl control, Vector2Int mousePosition, bool attemptAlwaysSuccessful = false)
        {
            if (_isDraggingControl)
                return;
            var dragTarget = control;
            if (!dragTarget.IsMoveable)
                return;
            while (dragTarget.Parent != null)
                dragTarget = dragTarget.Parent;
            if (dragTarget.IsMoveable)
            {
                if (attemptAlwaysSuccessful)
                {
                    _draggingControl = dragTarget;
                    _dragOriginX = mousePosition.X;
                    _dragOriginY = mousePosition.Y;
                }
                if (_draggingControl == dragTarget)
                {
                    int deltaX = mousePosition.X - _dragOriginX;
                    int deltaY = mousePosition.Y - _dragOriginY;
                    if (attemptAlwaysSuccessful || Math.Abs(deltaX) + Math.Abs(deltaY) > 4)
                        _isDraggingControl = true;
                }
                else
                {
                    _draggingControl = dragTarget;
                    _dragOriginX = mousePosition.X;
                    _dragOriginY = mousePosition.Y;
                }
            }
            if (_isDraggingControl)
                for (var i = 0; i < 5; i++)
                    if (_mouseDownControl[i] != null && _mouseDownControl[i] != _draggingControl)
                    {
                        _mouseDownControl[i].MouseUp(mousePosition, (MouseButton)i);
                        _mouseDownControl[i] = null;
                    }
        }

        void DoDragControl(Vector2Int mousePosition)
        {
            if (_draggingControl == null)
                return;
            var deltaX = mousePosition.X - _dragOriginX;
            var deltaY = mousePosition.Y - _dragOriginY;
            _draggingControl.Position = new Vector2Int(_draggingControl.X + deltaX, _draggingControl.Y + deltaY);
            _dragOriginX = mousePosition.X;
            _dragOriginY = mousePosition.Y;
        }

        void EndDragControl(Vector2Int mousePosition)
        {
            if (_isDraggingControl)
                DoDragControl(mousePosition);
            _draggingControl = null;
            _isDraggingControl = false;
        }

        void ClipMouse(ref Vector2Int position)
        {
            if (position.X < -8)
                position.X = -8;
            if (position.Y < -8)
                position.Y = -8;
            if (position.X >= Width + 8)
                position.X = Width + 8;
            if (position.Y >= Height + 8)
                position.Y = Height + 8;
        }
    }
}
