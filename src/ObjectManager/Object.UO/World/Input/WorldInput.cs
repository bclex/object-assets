//using OA.Core;
//using OA.Ultima.Core;
//using OA.Ultima.World.Entities;
//using UnityEngine;

//namespace OA.Ultima.World.Input
//{
//    /// <summary>
//    /// Handles all the mouse input when the mouse is over the world.
//    /// </summary>
//    class WorldInput
//    {
//        const double c_PauseBeforeMouseMovementMS = 105d;
//        const double c_PauseBeforeKeyboardFacingMS = 55d; // a little more than three frames @ 60fps.
//        const double c_PauseBeforeKeyboardMovementMS = 125d; // a little more than seven frames @ 60fps.
//        bool _continuousMouseMovementCheck;

//        AEntity _draggingEntity;

//        INetworkClient _network;
//        UserInterfaceService _userInterface;
//        IInputService _input;

//        // keyboard movement variables.
//        double _pauseBeforeKeyboardMovementMS;

//        double _timeSinceMovementButtonPressed;
//        Vector2Int m_DragOffset;

//        protected WorldModel World { get; private set; }

//        MacroEngine _macros;

//        public WorldInput(WorldModel world)
//        {
//            // parent reference
//            World = world;
//            // service references
//            _network = Service.Get<INetworkClient>();
//            _userInterface = Service.Get<UserInterfaceService>();
//            _input = Service.Get<IInputService>();
//            // local instances
//            MousePick = new MousePicking();
//            _macros = new MacroEngine();
//        }

//        public MousePicking MousePick { get; private set; }

//        public bool ContinuousMouseMovementCheck
//        {
//            get { return _continuousMouseMovementCheck; }
//            set
//            {
//                if (_continuousMouseMovementCheck != value)
//                {
//                    _continuousMouseMovementCheck = value;
//                    if (_continuousMouseMovementCheck) _userInterface.AddInputBlocker(this);
//                    else _userInterface.RemoveInputBlocker(this);
//                }
//            }
//        }

//        public bool IsMouseOverUI
//        {
//            get
//            {
//                if (_userInterface.IsMouseOverUI)
//                {
//                    var over = _userInterface.MouseOverControl;
//                    return !(over is WorldViewport);
//                }
//                return false;
//            }
//        }

//        public bool IsMouseOverWorld
//        {
//            get
//            {
//                if (_userInterface.IsMouseOverUI)
//                {
//                    var over = _userInterface.MouseOverControl;
//                    return (over is WorldViewport);
//                }
//                return false;
//            }
//        }

//        public Vector2Int MouseOverWorldPosition
//        {
//            get
//            {
//                var world = Service.Get<WorldViewport>();
//                var mouse = new Vector2Int(_input.MousePosition.X - world.ScreenX, _input.MousePosition.Y - world.ScreenY);
//                return mouse;
//            }
//        }

//        public void Dispose()
//        {
//            _userInterface.RemoveInputBlocker(this);
//        }

//        public void Update(double frameMS)
//        {
//            if (WorldModel.IsInWorld && !_userInterface.IsModalControlOpen && _network.IsConnected)
//            {
//                // always parse keyboard. (Is it possible there are some situations in which keyboard input is blocked???)
//                InternalParseKeyboard(frameMS);
//                // In all cases, where we are moving and the move button was released, stop moving.
//                if (ContinuousMouseMovementCheck && _input.HandleMouseEvent(MouseEvent.Up, Settings.UserInterface.Mouse.MovementButton))
//                    ContinuousMouseMovementCheck = false;
//                if (IsMouseOverWorld)
//                    InternalParseMouse(frameMS);
//                // PickType is the kind of objects that will show up as the 'MouseOverObject'
//                if (IsMouseOverWorld)
//                {
//                    MousePick.PickOnly = PickType.PickEverything;
//                    MousePick.Position = MouseOverWorldPosition;
//                    if (Settings.UserInterface.PlayWindowPixelDoubling)
//                        MousePick.Position = MousePick.Position.DivideBy(2);
//                }
//                else MousePick.PickOnly = PickType.PickNothing;
//                doMouseMovement(frameMS);
//            }
//            else MousePick.PickOnly = PickType.PickNothing; // the world is not receiving input this frame. get rid of any mouse picking data.
//            _macros.Update(frameMS);
//        }

//        void doMouseMovement(double frameMS)
//        {
//            var player = (Mobile)WorldModel.Entities.GetPlayerEntity();
//            if (player == null)
//                return;
//            // if the move button is pressed, change facing and move based on mouse cursor direction.
//            if (ContinuousMouseMovementCheck)
//            {
//                var resolution = Settings.UserInterface.PlayWindowGumpResolution;
//                var centerScreen = new Vector2Int(resolution.Width / 2, resolution.Height / 2);
//                var mouseDirection = DirectionHelper.DirectionFromPoints(centerScreen, MouseOverWorldPosition);
//                _timeSinceMovementButtonPressed += frameMS;
//                if (_timeSinceMovementButtonPressed >= c_PauseBeforeMouseMovementMS)
//                {
//                    // Get the move direction.
//                    var moveDirection = mouseDirection;
//                    // add the running flag if the mouse cursor is far enough away from the center of the screen.
//                    var distanceFromCenterOfScreen = Utility.DistanceBetweenTwoPoints(centerScreen, MouseOverWorldPosition);
//                    if (distanceFromCenterOfScreen >= 150.0f || Settings.UserInterface.AlwaysRun)
//                        moveDirection |= Direction.Running;
//                    player.PlayerMobile_Move(moveDirection);
//                }
//                else
//                {
//                    // Get the move direction.
//                    var facing = mouseDirection;
//                    if (player.Facing != facing)
//                    {
//                        // Tell the player entity to change facing to this direction.
//                        player.PlayerMobile_ChangeFacing(facing);
//                        // reset the time since the mouse cursor was pressed - allows multiple facing changes.
//                        _timeSinceMovementButtonPressed = 0d;
//                    }
//                }
//            }
//            else
//            {
//                _timeSinceMovementButtonPressed = 0d;
//                // Tell the player to stop moving.
//                player.PlayerMobile_Move(Direction.Nothing);
//            }
//        }

//        void doKeyboardMovement(double frameMS)
//        {
//            var player = (Mobile)WorldModel.Entities.GetPlayerEntity();
//            if (player == null)
//                return;
//            if (_pauseBeforeKeyboardMovementMS < c_PauseBeforeKeyboardMovementMS)
//            {
//                if (_input.HandleKeyboardEvent(KeyboardEvent.Up, WinKeys.Up, false, false, false))
//                    _pauseBeforeKeyboardMovementMS = 0;
//                if (_input.HandleKeyboardEvent(KeyboardEvent.Up, WinKeys.Down, false, false, false))
//                    _pauseBeforeKeyboardMovementMS = 0;
//                if (_input.HandleKeyboardEvent(KeyboardEvent.Up, WinKeys.Left, false, false, false))
//                    _pauseBeforeKeyboardMovementMS = 0;
//                if (_input.HandleKeyboardEvent(KeyboardEvent.Up, WinKeys.Right, false, false, false))
//                    _pauseBeforeKeyboardMovementMS = 0;
//            }
//            var up = _input.IsKeyDown(WinKeys.Up);
//            var left = _input.IsKeyDown(WinKeys.Left);
//            var right = _input.IsKeyDown(WinKeys.Right);
//            var down = _input.IsKeyDown(WinKeys.Down);
//            if (up | left | right | down)
//            {
//                // Allow a short span of time (50ms) to get all the keys pressed.
//                // Otherwise, when moving diagonally, we would only get the first key
//                // in most circumstances and the second key a frame or two later - but
//                // too late, we would already be moving in the non-diagonal direction :(
//                _pauseBeforeKeyboardMovementMS += frameMS;
//                if (_pauseBeforeKeyboardMovementMS >= c_PauseBeforeKeyboardFacingMS)
//                {
//                    var facing = Direction.Up;
//                    if (up)
//                    {
//                        if (left) facing = Direction.West;
//                        else if (_input.IsKeyDown(WinKeys.Right)) facing = Direction.North;
//                        else facing = Direction.Up;
//                    }
//                    else if (down)
//                    {
//                        if (left) facing = Direction.South;
//                        else if (right) facing = Direction.East;
//                        else facing = Direction.Down;
//                    }
//                    else
//                    {
//                        if (left) facing = Direction.Left;
//                        else if (right) facing = Direction.Right;
//                    }
//                    // only send messages if we're not moving.
//                    if (!player.IsMoving)
//                    {
//                        if (_pauseBeforeKeyboardMovementMS >= c_PauseBeforeKeyboardMovementMS)
//                            player.PlayerMobile_Move(facing);
//                        else if (player.Facing != facing)
//                            player.PlayerMobile_ChangeFacing(facing);
//                    }
//                }
//            }
//            else  _pauseBeforeKeyboardMovementMS = 0;
//        }

//        void onMoveButton(InputEventMouse e)
//        {
//            if (e.EventType == MouseEvent.Down)
//            {
//                // keep moving as long as the move button is down.
//                ContinuousMouseMovementCheck = true;
//            }
//            else if (e.EventType == MouseEvent.Up)
//            {
//                // If the movement mouse button has been released, stop moving.
//                ContinuousMouseMovementCheck = false;
//            }
//            e.Handled = true;
//        }

//        void onInteractButton(InputEventMouse e, AEntity overEntity, Point overEntityPoint)
//        {
//            if (e.EventType == MouseEvent.Down)
//            {
//                // prepare to pick this item up.
//                _draggingEntity = overEntity;
//                m_DragOffset = overEntityPoint;
//            }
//            else if (e.EventType == MouseEvent.Click)
//            {
//                if (overEntity is Ground)
//                {
//                    // no action.
//                }
//                else if (overEntity is StaticItem)
//                {
//                    // pop up name of item.
//                    overEntity.AddOverhead(MessageTypes.Label, overEntity.Name, 3, 0, false);
//                    WorldModel.Statics.AddStaticThatNeedsUpdating(overEntity as StaticItem);
//                }
//                else if (overEntity is Item)
//                {
//                    World.Interaction.SingleClick(overEntity);
//                }
//                else if (overEntity is Mobile)
//                {
//                    World.Interaction.SingleClick(overEntity);
//                }
//            }
//            else if (e.EventType == MouseEvent.DoubleClick)
//            {
//                if (overEntity is Ground)
//                {
//                    // no action.
//                }
//                else if (overEntity is StaticItem)
//                {
//                    // no action.
//                }
//                else if (overEntity is Item)
//                {
//                    // request context menu
//                    World.Interaction.DoubleClick(overEntity);
//                }
//                else if (overEntity is Mobile)
//                {
//                    // Send double click packet.
//                    // Set LastTarget == targeted Mobile.
//                    // If in WarMode, set Attacking == true.
//                    Mobile mobile = overEntity as Mobile;
//                    World.Interaction.LastTarget = overEntity.Serial;

//                    if (WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode)
//                    {
//                        World.Interaction.AttackRequest(mobile);
//                    }
//                    else
//                    {
//                        World.Interaction.DoubleClick(overEntity);
//                    }
//                }
//            }
//            else if (e.EventType == MouseEvent.DragBegin)
//            {
//                if (overEntity is Ground)
//                {
//                    // no action.
//                }
//                else if (overEntity is StaticItem)
//                {
//                    // no action.
//                }
//                else if (overEntity is Item)
//                {
//                    // attempt to pick up item.
//                    World.Interaction.PickupItem((Item)overEntity, new Point((int)m_DragOffset.X, (int)m_DragOffset.Y));
//                }
//                else if (overEntity is Mobile)
//                {
//                    if (PlayerState.Partying.GetMember(overEntity.Serial) != null)//is he in your party// number of 0x11 packet dont have information about stamina/mana k(IMPORTANT!!!)
//                        return;
//                    // request basic stats - gives us the name rename flag
//                    _network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, overEntity.Serial));
//                    // drag off a status gump for this mobile.
//                    MobileHealthTrackerGump gump = new MobileHealthTrackerGump(overEntity as Mobile);
//                    _userInterface.AddControl(gump, e.X - 77, e.Y - 30);
//                    _userInterface.AttemptDragControl(gump, new Point(e.X, e.Y), true);
//                }
//            }

//            e.Handled = true;
//        }

//        void InternalParseMouse(double frameMS)
//        {
//            List<InputEventMouse> events = _input.GetMouseEvents();
//            foreach (InputEventMouse e in events)
//            {
//                if (e.Button == Settings.UserInterface.Mouse.MovementButton)
//                {
//                    onMoveButton(e);
//                }
//                else if (e.Button == Settings.UserInterface.Mouse.InteractionButton)
//                {
//                    if (e.EventType == MouseEvent.Click)
//                    {
//                        InternalQueueSingleClick(e, MousePick.MouseOverObject, MousePick.MouseOverObjectPoint);
//                        continue;
//                    }
//                    if (e.EventType == MouseEvent.DoubleClick)
//                    {
//                        ClearQueuedClick();
//                    }
//                    onInteractButton(e, MousePick.MouseOverObject, MousePick.MouseOverObjectPoint);
//                }
//            }

//            InternalCheckQueuedClick(frameMS);
//        }

//        void InternalParseKeyboard(double frameMS)
//        {
//            // macros
//            doMacroInput(_input.GetKeyboardEvents());

//            // all names mode
//            WorldView.AllLabels = (_input.IsShiftDown && _input.IsCtrlDown);

//            // Warmode toggle: (maybe moving to macro manager)
//            if (_input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Tab, false, false, false))
//            {
//                _network.Send(new RequestWarModePacket(!WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode));
//            }

//            // Toggle minimap. Default is Alt-R.
//            if (_input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.R, false, true, false))
//            {
//                MiniMapGump.Toggle();
//            }

//            // movement with arrow keys if the player is not moving and the mouse isn't moving the player.
//            if (!ContinuousMouseMovementCheck)
//            {
//                doKeyboardMovement(frameMS);
//            }

//            // FPS limiting
//            if (_input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.F, false, true, false))
//            {
//                Settings.Engine.IsFixedTimeStep = !Settings.Engine.IsFixedTimeStep;
//            }

//            // Display FPS
//            if (_input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.F, false, true, true))
//            {
//                Settings.Debug.ShowFps = !Settings.Debug.ShowFps;
//            }

//            // Mouse enable / disable
//            if (_input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.M, false, true, false))
//            {
//                Settings.UserInterface.Mouse.IsEnabled = !Settings.UserInterface.Mouse.IsEnabled;
//            }
//        }

//        void doMacroInput(List<InputEventKeyboard> events)
//        {
//            foreach (InputEventKeyboard e in events)
//            {
//                foreach (Action action in Macros.Player.All)
//                {
//                    if (e.EventType == KeyboardEvent.Press &&
//                        action.Keystroke == e.KeyCode &&
//                        action.Alt == e.Alt &&
//                        action.Ctrl == e.Control &&
//                        action.Shift == e.Shift)
//                    {
//                        _macros.Run(action);
//                        e.Handled = true;
//                    }
//                }
//            }
//        }

//        #region QueuedClicks

//        // Legacy Client waits about 0.5 seconds before sending a click event when you click in the world.
//        // This allows time for the player to potentially double-click on an object.
//        // If the player does so, this will cancel the single-click event.
//        AEntity m_QueuedEntity;
//        Point m_QueuedEntityPosition;
//        InputEventMouse m_QueuedEvent;
//        double m_QueuedEvent_DequeueAt;
//        bool m_QueuedEvent_InQueue;

//        void ClearQueuedClick()
//        {
//            m_QueuedEvent_InQueue = false;
//            m_QueuedEvent = null;
//            m_QueuedEntity = null;
//        }

//        void InternalCheckQueuedClick(double frameMS)
//        {
//            if (m_QueuedEvent_InQueue)
//            {
//                m_QueuedEvent_DequeueAt -= frameMS;
//                if (m_QueuedEvent_DequeueAt <= 0d)
//                {
//                    onInteractButton(m_QueuedEvent, m_QueuedEntity, m_QueuedEntityPosition);
//                    ClearQueuedClick();
//                }
//            }
//        }

//        void InternalQueueSingleClick(InputEventMouse e, AEntity overEntity, Point overEntityPoint)
//        {
//            m_QueuedEvent_InQueue = true;
//            m_QueuedEntity = overEntity;
//            m_QueuedEntityPosition = overEntityPoint;
//            m_QueuedEvent_DequeueAt = Settings.UserInterface.Mouse.DoubleClickMS;
//            m_QueuedEvent = e;
//        }

//        #endregion
//    }
//}