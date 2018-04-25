using OA.Core.Input;
using OA.Ultima;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Core.UI
{
    /// <summary>
    /// The base class that all UI controls should inherit from.
    /// </summary>
    public abstract class AControl
    {
        // ============================================================================================================
        // Private variables
        // ============================================================================================================
        RectInt _area = new RectInt();
        ControlMetaData _metaData;
        List<AControl> _children;

        // ============================================================================================================
        // Private services
        // ============================================================================================================
        protected UserInterfaceService UserInterface { get; private set; }

        // ============================================================================================================
        // Public properties
        // ============================================================================================================
        #region Public properties
        /// <summary>
        /// An identifier for this control. Can be used to differentiate controls of the same type. Used by UO as a 'Serial'
        /// </summary>
        public int GumpLocalID { get; set; }

        /// <summary>
        /// A unique identifier, assigned by the server, that is sent by the client when a button is pressed.
        /// </summary>
        public int GumpServerTypeID { get; set; }

        /// <summary>
        /// Information used by UserInterfaceService to display and update this control.
        /// </summary>
        public ControlMetaData MetaData
        {
            get
            {
                if (_metaData == null)
                    _metaData = new ControlMetaData(this);
                return _metaData;
            }
        }

        /// <summary>
        /// Indicates that the control has been disposed, and will be removed on the next Update() of the UserInterface object.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Controls that are not enabled cannot receive keyboard and mouse input, but still Draw.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Indicates whether the control has been Initialized by the UserInterface object, which happens every time the UserInterface updates.
        /// Controls that are not initialized do not update and do not draw.
        /// </summary>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// If true, control can be moved by click-dragging with left mouse button.
        /// A child control can be made a dragger for a parent control with MakeDragger().
        /// </summary>
        public virtual bool IsMoveable { get; set; }

        /// <summary>
        /// If true, gump cannot be closed with right-click.
        /// </summary>
        public bool IsUncloseableWithRMB { get; set; }

        /// <summary>
        /// If true, gump does not close when the player hits the Escape key. This behavior is currently unimplemented.
        /// </summary>
        public bool IsUncloseableWithEsc { get; set; }

        /// <summary>
        /// If true, the gump will draw. Not visible gumps still update and receive mouse input (but not keyboard input).
        /// </summary>
        public bool IsVisible { get; set; }

        public bool IsEditable { get; set; }

        /// <summary>
        /// A list of all the child controls that this control owns.
        /// </summary>
        public List<AControl> Children
        {
            get
            {
                if (_children == null)
                    _children = new List<AControl>();
                return _children;
            }
        }

        #endregion

        #region Position and Area properties

        public int X { get { return _area.x; } }
        public int Y { get { return _area.y; } }

        public int ScreenX
        {
            get { return ParentX + X; }
        }

        public int ScreenY
        {
            get { return ParentY + Y; }
        }

        public virtual int Width
        {
            get { return _area.width; }
            set { _area.width = value; }
        }

        public virtual int Height
        {
            get { return _area.height; }
            set { _area.height = value; }
        }

        public Vector2Int Position
        {
            get { return new Vector2Int(_area.x, _area.y); }
            set
            {
                if (value.x != _area.x || value.y != _area.y)
                {
                    _area.x = value.x;
                    _area.y = value.y;
                    OnMove();
                }
            }
        }

        public Vector2Int Size
        {
            get { return new Vector2Int(_area.width, _area.height); }
            set
            {
                _area.width = value.x;
                _area.height = value.y;
            }
        }

        #endregion

        #region Page

        /// <summary>
        /// This's control's drawing/input page index. On Update() and Draw(), only those controls with Page == 0 or
        /// Page == Parent.ActivePage will accept input and be drawn.
        /// </summary>
        public int Page { get; set; }

        int _activePage; // we always draw m_activePage and Page 0.

        /// <summary>
        /// This control's active page index. On Update and Draw(), this control will send update to and draw all children with Page == 0 or
        /// Page == this.Page.
        /// </summary>
        public int ActivePage
        {
            get { return _activePage; }
            set
            {
                _activePage = value;
                // If we own the current KeyboardFocusControl, then we should clear it.
                // UNLESS page = 0; in which case it still exists and should maintain focus.
                // Clear the current keyboardfocus if we own it and it's page != 0
                // If the page = 0, then it will still exist so it should maintain focus.
                if (UserInterface.KeyboardFocusControl != null)
                    if (Children.Contains(UserInterface.KeyboardFocusControl))
                        if (UserInterface.KeyboardFocusControl.Page != 0)
                            UserInterface.KeyboardFocusControl = null;
                // When ActivePage changes, check to see if there are new text input boxes
                // that we should redirect text input to.
                if (UserInterface.KeyboardFocusControl == null)
                    foreach (var c in Children)
                        if (c.HandlesKeyboardFocus && c.Page == _activePage)
                        {
                            UserInterface.KeyboardFocusControl = c;
                            break;
                        }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// An event that other objects can use to be notified when this control is clicked.
        /// </summary>
        internal event Action<AControl, int, int, MouseButton> MouseClickEvent;

        /// <summary>
        /// An event that other objects can use to be notified when this control is double-clicked.
        /// </summary>
        internal event Action<AControl, int, int, MouseButton> MouseDoubleClickEvent;

        /// <summary>
        /// An event that other objects can use to be notified when this control receives a mouse down event.
        /// </summary>
        internal event Action<AControl, int, int, MouseButton> MouseDownEvent;

        /// <summary>
        /// An event that other objects can use to be notified when this control receives a mouse up event.
        /// </summary>
        internal event Action<AControl, int, int, MouseButton> MouseUpEvent;

        /// <summary>
        /// An event that other objects can use to be notified when this control receives a mouse over event.
        /// </summary>
        internal event Action<AControl, int, int> MouseOverEvent;

        /// <summary>
        /// An event that other objects can use to be notified when this control receives a mouse out event.
        /// </summary>
        internal event Action<AControl, int, int> MouseOutEvent;

        #endregion

        #region Parent control variables

        public AControl Parent { get; protected set; }

        /// <summary>
        /// Gets the root (topmost, or final) parent of this control.
        /// </summary>
        public AControl RootParent
        {
            get
            {
                if (Parent == null)
                    return null;
                var parent = Parent;
                while (parent.Parent != null)
                    parent = parent.Parent;
                return parent;
            }
        }

        int ParentX
        {
            get
            {
                if (Parent != null) return Parent.X + Parent.ParentX;
                else return 0;
            }
        }

        int ParentY
        {
            get
            {
                if (Parent != null) return Parent.Y + Parent.ParentY;
                else return 0;
            }
        }

        #endregion

        // ============================================================================================================
        // Ctor, Init, Dispose, Update, and Draw
        // ============================================================================================================
        protected AControl(AControl parent)
        {
            Parent = parent;
            Page = 0;
            UserInterface = Service.Get<UserInterfaceService>();
        }

        public void Initialize()
        {
            IsDisposed = false;
            IsEnabled = true;
            IsInitialized = true;
            IsVisible = true;
            InitializeControls();
            OnInitialize();
        }

        public virtual void Dispose()
        {
            ClearControls();
            IsDisposed = true;
        }

        public virtual void Update(double totalMS, double frameMS)
        {
            if (!IsInitialized || IsDisposed)
                return;
            InitializeControls();
            UpdateControls(totalMS, frameMS);
            ExpandToFitControls();
        }

        public virtual void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            if (!IsInitialized)
                return;
            foreach (var c in Children)
                if (c.Page == 0 || c.Page == ActivePage)
                    if (c.IsInitialized && c.IsVisible)
                    {
                        var offset = new Vector2Int(c.Position.x + position.x, c.Position.y + position.y);
                        c.Draw(spriteBatch, offset, frameMS);
                    }
        }

        // ============================================================================================================
        // Child control methods
        // ============================================================================================================
        public T AddControl<T>(AControl c, int page = 0) where T : AControl
        {
            c.Page = page;
            Children.Add(c);
            return LastControl as T;
        }

        public AControl AddControl(AControl c, int page = 0)
        {
            c.Page = page;
            Children.Add(c);
            return LastControl;
        }

        public AControl LastControl
        {
            get { return Children[Children.Count - 1]; }
        }

        public void ClearControls()
        {
            if (Children != null)
                foreach (AControl c in Children)
                    c.Dispose();
        }

        void InitializeControls()
        {
            var newlyInitializedChildReceivedKeyboardFocus = false;
            foreach (var c in Children)
                if (!c.IsInitialized)
                {
                    c.Initialize();
                    if (!newlyInitializedChildReceivedKeyboardFocus && c.HandlesKeyboardFocus)
                    {
                        UserInterface.KeyboardFocusControl = c;
                        newlyInitializedChildReceivedKeyboardFocus = true;
                    }
                }
        }

        void UpdateControls(double totalMS, double frameMS)
        {
            foreach (var c in Children)
                c.Update(totalMS, frameMS);
            var disposedControls = new List<AControl>();
            foreach (var c in Children)
                if (c.IsDisposed)
                    disposedControls.Add(c);
            foreach (var c in disposedControls)
                Children.Remove(c);
        }

        bool ExpandToFitControls()
        {
            var changedDimensions = false;
            if (Children.Count > 0)
            {
                int w = 0, h = 0;
                foreach (var c in Children)
                    if (c.Page == 0 || c.Page == ActivePage)
                    {
                        if (w < c.X + c.Width)
                            w = c.X + c.Width;
                        if (h < c.Y + c.Height)
                            h = c.Y + c.Height;
                    }
                if (w != Width || h != Height)
                {
                    Width = w;
                    Height = h;
                    changedDimensions = true;
                }
            }
            return changedDimensions;
        }

        // ============================================================================================================
        // Miscellaneous methods
        // ============================================================================================================
        public void CenterThisControlOnScreen()
        {
            Position = new Vector2Int((UserInterface.Width - Width) / 2, (UserInterface.Height - Height) / 2);
        }

        /// <summary>
        /// Convenience method: Sets this control to (1) handle mouse input and (2) make it moveable (which makes the parent control moveable).
        /// </summary>
        public void MakeThisADragger()
        {
            HandlesMouseInput = true;
            IsMoveable = true;
        }

        public virtual void OnButtonClick(int buttonID)
        {
            if (Parent != null)
                Parent.OnButtonClick(buttonID);
        }

        public virtual void OnKeyboardReturn(int textID, string text)
        {
            if (Parent != null)
                Parent.OnKeyboardReturn(textID, text);
        }

        public virtual void OnHtmlInputEvent(string href, MouseEvent e)
        {
            if (Parent != null)
                Parent.OnHtmlInputEvent(href, e);
        }

        public virtual void ChangePage(int pageIndex)
        {
            if (Parent != null)
                Parent.ChangePage(pageIndex);
        }

        protected int ServerRecievedHueTransform(int hue)
        {
            if (hue > 1)            // hue: if greater than or equal to 2, subtract 2 to get the true hue.
                hue -= 2;
            if (hue < 2)             // hue: if 0 or 1, set to 1 (true black).
                hue = 1;
            return hue;
        }

        // ============================================================================================================
        // Overrideable methods
        // ============================================================================================================

        #region OverrideableMethods

        protected virtual void OnMouseDown(int x, int y, MouseButton button)
        {
        }

        protected virtual void OnMouseUp(int x, int y, MouseButton button)
        {
        }

        protected virtual void OnMouseOver(int x, int y)
        {
        }

        protected virtual void OnMouseOut(int x, int y)
        {
        }

        protected virtual void OnMouseClick(int x, int y, MouseButton button)
        {
        }

        protected virtual void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
        }

        protected virtual void OnKeyboardInput(InputEventKeyboard e)
        {
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnMove()
        {
        }

        protected virtual bool IsPointWithinControl(int x, int y)
        {
            return true;
        }

        #endregion

        // ============================================================================================================
        // Tooltip handling code - shows text when the player mouses over this control.
        // ============================================================================================================

        #region Tooltip

        string _tooltip;

        public string Tooltip => _tooltip;

        public bool HasTooltip => PlayerState.ClientFeatures.TooltipsEnabled && (_tooltip != null);

        public void SetTooltip(string caption)
        {
            if (string.IsNullOrEmpty(caption)) ClearTooltip();
            else _tooltip = caption;
        }

        public void ClearTooltip()
        {
            _tooltip = null;
        }

        #endregion

        // ============================================================================================================
        // Mouse handling code
        // ============================================================================================================

        #region MouseInput

        // private variables
        bool _handlesMouseInput;
        float _maxTimeForDoubleClick;
        Vector2Int _lastClickPosition;

        // public methods
        public bool IsMouseOver
        {
            get
            {
                if (UserInterface.MouseOverControl == this)
                    return true;
                return false;
            }
        }

        public virtual bool HandlesMouseInput
        {
            get { return (IsEnabled && IsInitialized && !IsDisposed && _handlesMouseInput); }
            set { _handlesMouseInput = value; }
        }

        public void MouseDown(Vector2Int position, MouseButton button)
        {
            _lastClickPosition = position;
            var x = (int)position.x - X - ParentX;
            var y = (int)position.y - Y - ParentY;
            OnMouseDown(x, y, button);
            MouseDownEvent?.Invoke(this, x, y, button);
        }

        public void MouseUp(Vector2Int position, MouseButton button)
        {
            var x = (int)position.x - X - ParentX;
            var y = (int)position.y - Y - ParentY;
            OnMouseUp(x, y, button);
            MouseUpEvent?.Invoke(this, x, y, button);
        }

        public void MouseOver(Vector2Int position)
        {
            // Does not double-click if you move your mouse more than x pixels from where you first clicked.
            if (Math.Abs(_lastClickPosition.x - position.x) + Math.Abs(_lastClickPosition.y - position.y) > 3)
                _maxTimeForDoubleClick = 0.0f;
            var x = (int)position.x - X - ParentX;
            var y = (int)position.y - Y - ParentY;
            OnMouseOver(x, y);
            MouseOverEvent?.Invoke(this, x, y);
        }

        public void MouseOut(Vector2Int position)
        {
            var x = (int)position.x - X - ParentX;
            var y = (int)position.y - Y - ParentY;
            OnMouseOut(x, y);
            MouseOutEvent?.Invoke(this, x, y);
        }

        public void MouseClick(Vector2Int position, MouseButton button)
        {
            var x = position.x - X - ParentX;
            var y = position.y - Y - ParentY;
            var totalMS = (float)Service.Get<UltimaGame>().TotalMS;
            var doubleClick = false;
            if (_maxTimeForDoubleClick != 0f)
            {
                if (totalMS <= _maxTimeForDoubleClick)
                {
                    _maxTimeForDoubleClick = 0f;
                    doubleClick = true;
                }
            }
            else _maxTimeForDoubleClick = totalMS + UltimaGameSettings.UserInterface.Mouse.DoubleClickMS;
            if (button == MouseButton.Right && !IsUncloseableWithRMB)
            {
                CloseWithRightMouseButton();
                return;
            }
            if (doubleClick)
            {
                OnMouseDoubleClick(x, y, button);
                MouseDoubleClickEvent?.Invoke(this, x, y, button);
            }
            else
            {
                OnMouseClick(x, y, button);
                MouseClickEvent?.Invoke(this, x, y, button);
            }
        }

        protected virtual void CloseWithRightMouseButton()
        {
            if (IsUncloseableWithRMB)
                return;
            var parent = Parent;
            while (parent != null)
            {
                if (parent.IsUncloseableWithRMB)
                    return;
                parent = parent.Parent;
            }
            // dispose of this, or the parent if it has one, which will close this as a child.
            if (Parent == null) Dispose();
            else Parent.CloseWithRightMouseButton();
        }

        public AControl[] HitTest(Vector2Int position, bool alwaysHandleMouseInput)
        {
            var focusedControls = new List<AControl>();
            var inBounds = _area.Contains(new Vector2Int((int)position.x - ParentX, (int)position.y - ParentY));
            if (inBounds)
                if (IsPointWithinControl((int)position.x - X - ParentX, (int)position.y - Y - ParentY))
                {
                    if (alwaysHandleMouseInput || HandlesMouseInput)
                        focusedControls.Insert(0, this);
                    for (var i = 0; i < Children.Count; i++)
                    {
                        var c = Children[i];
                        if (c.Page == 0 || c.Page == ActivePage)
                        {
                            var c1 = c.HitTest(position, false);
                            if (c1 != null)
                                for (var j = c1.Length - 1; j >= 0; j--)
                                    focusedControls.Insert(0, c1[j]);
                        }
                    }
                }
            if (focusedControls.Count == 0) return null;
            else return focusedControls.ToArray();
        }

        #endregion

        // ============================================================================================================
        // Keyboard handling code
        // ============================================================================================================

        #region KeyboardInput

        // private variables
        bool _handlesKeyboardFocus;

        // public methods
        public virtual bool HandlesKeyboardFocus
        {
            get
            {
                if (!IsEnabled || !IsInitialized || IsDisposed || !IsVisible)
                    return false;
                if (_handlesKeyboardFocus)
                    return true;
                if (_children == null)
                    return false;
                foreach (var c in _children)
                    if (c.HandlesKeyboardFocus)
                        return true;
                return false;
            }
            set { _handlesKeyboardFocus = value; }
        }

        public void KeyboardInput(InputEventKeyboard e)
        {
            OnKeyboardInput(e);
        }

        /// <summary>
        /// Called when the Control that current has keyboard focus releases that focus; for example, when Tab is pressed.
        /// </summary>
        /// <param name="c">The control that is releasing focus.</param>
        internal void KeyboardTabToNextFocus(AControl c)
        {
            var startIndex = Children.IndexOf(c);
            for (var i = startIndex + 1; i < Children.Count; i++)
                if (Children[i].HandlesKeyboardFocus)
                {
                    UserInterface.KeyboardFocusControl = Children[i];
                    return;
                }
            for (var i = 0; i < startIndex; i++)
                if (Children[i].HandlesKeyboardFocus)
                {
                    UserInterface.KeyboardFocusControl = Children[i];
                    return;
                }
        }

        public AControl FindControlThatAcceptsKeyboardFocus()
        {
            if (_handlesKeyboardFocus)
                return this;
            if (_children == null)
                return null;
            foreach (var c in _children)
                if (c.HandlesKeyboardFocus)
                    return c.FindControlThatAcceptsKeyboardFocus();
            return null;
        }

        #endregion

        // ============================================================================================================
        // Debug control boundary drawing code
        // ============================================================================================================

        #region DebugBoundaryDrawing

        Texture2DInfo _boundsTexture;

        protected void DebugDrawBounds(SpriteBatchUI spriteBatch, Vector2Int position, Color color)
        {
            if (!IsVisible)
                return;
            if (_boundsTexture == null)
            {
                _boundsTexture = new Texture2DInfo(1, 1);
                //_boundsTexture.SetData(new Color[] { Color.white });
            }
            var drawArea = new RectInt(ScreenX, ScreenY, Width, Height);
            spriteBatch.Draw2D(_boundsTexture, new RectInt(position.x, position.y, Width, 1), Vector3.zero);
            spriteBatch.Draw2D(_boundsTexture, new RectInt(position.x, position.y + Height - 1, Width, 1), Vector3.zero);
            spriteBatch.Draw2D(_boundsTexture, new RectInt(position.x, position.y, 1, Height), Vector3.zero);
            spriteBatch.Draw2D(_boundsTexture, new RectInt(position.x + Width - 1, position.y, 1, Height), Vector3.zero);
        }

        #endregion
    }
}