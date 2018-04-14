using OA.Ultima.Network.Client;
using System;
using UnityEngine;

namespace OA.Ultima.World.Entities.Mobiles
{
    /// <summary>
    /// Mobile movement state tracking object. Receives movement packets from the server and sends client move packets to the server.
    /// TODO: This class needs a serious refactor.
    /// </summary>
    public class MobileMovement
    {
        double m_playerMobile_NextMoveInMS;

        public static Action<MoveRequestPacket> SendMoveRequestPacket;

        public static bool NewDiagonalMovement = true;

        public bool RequiresUpdate { get; set; }

        public bool IsRunning
        {
            get { return ((Facing & Direction.Running) == Direction.Running); }
        }

        protected Position3D CurrentPosition
        {
            get { return _entity.Position; }
        }

        public Position3D GoalPosition { get; private set; }

        Direction _playerMobile_NextMove = Direction.Nothing;
        Direction _facing = Direction.Up;
        const int _minimumStaminaToRun = 2;

        public Direction Facing
        {
            get { return _facing; }
            set { _facing = value; }
        }

        public bool IsMoving
        {
            get
            {
                if (GoalPosition == null)
                    return false;
                if ((CurrentPosition.Tile == GoalPosition.Tile) &&
                    !CurrentPosition.IsOffset)
                    return false;
                return true;
            }
        }

        double MoveSequence;

        internal Position3D Position { get { return CurrentPosition; } }

        MobileMoveEvents _moveEvents;
        AEntity _entity;

        public MobileMovement(AEntity entity)
        {
            _entity = entity;
            _moveEvents = new MobileMoveEvents();
        }

        public void PlayerMobile_MoveEventAck(int nSequence)
        {
            _moveEvents.AcknowledgeMoveRequest(nSequence);
        }

        public void PlayerMobile_MoveEventRej(int sequenceID, int x, int y, int z, int direction)
        {
            // immediately return to the designated tile.
            int ax, ay, az, af;
            _moveEvents.RejectMoveRequest(sequenceID, out ax, out ay, out az, out af);
            Move_Instant(x, y, z, direction);
            _moveEvents.ResetMoveSequence();
        }

        public void PlayerMobile_ChangeFacing(Direction facing)
        {
            if (!IsMoving)
            {
                PlayerMobile_SendChangeFacingMsg(facing);
                Facing = facing;
            }
        }

        /// <summary>
        /// If current Facing does not match param facing, will send a message to the server with the new facing.
        /// Does not change the actual facing value.
        /// </summary>
        /// <param name="facing"></param>
        void PlayerMobile_SendChangeFacingMsg(Direction facing)
        {
            if ((Facing & Direction.FacingMask) != (facing & Direction.FacingMask))
                _moveEvents.AddMoveEvent(
                    CurrentPosition.X,
                    CurrentPosition.Y,
                    CurrentPosition.Z,
                    (int)(facing & Direction.FacingMask),
                    true);
        }

        public void PlayerMobile_Move(Direction facing)
        {
            _playerMobile_NextMove = facing;
        }

        public void Mobile_ServerAddMoveEvent(int x, int y, int z, int facing)
        {
            _moveEvents.AddMoveEvent(x, y, z, facing, false);
        }

        public void Move_Instant(int x, int y, int z, int facing)
        {
            _moveEvents.ResetMoveSequence();
            Facing = (Direction)facing;
            CurrentPosition.Set(x, y, z);
            GoalPosition = null;
        }

        public void Update(double frameMS)
        {
            if (!IsMoving)
            {
                if (_entity.IsClientEntity && m_playerMobile_NextMoveInMS <= 0d)
                    PlayerMobile_CheckForMoveEvent();
                MobileMoveEvent moveEvent;
                int sequence;
                if (_entity.IsClientEntity)
                    while ((moveEvent = _moveEvents.GetNextMoveEvent(out sequence)) != null)
                    {
                        Facing = (Direction)moveEvent.Facing;
                        var pl = (Mobile)_entity;
                        if (pl.Stamina.Current == 0)
                            break;
                        if ((pl.Stamina.Current < _minimumStaminaToRun) && (pl.Stamina.Current > 0) && ((Facing & Direction.Running) == Direction.Running))
                            Facing &= Direction.FacingMask;
                        if (moveEvent.CreatedByPlayerInput)
                            SendMoveRequestPacket(new MoveRequestPacket((byte)Facing, (byte)sequence, moveEvent.Fastwalk));
                        var p = new Position3D(moveEvent.X, moveEvent.Y, moveEvent.Z);
                        if (p != CurrentPosition)
                        {
                            GoalPosition = p;
                            break;
                        }
                    }
                else
                {
                    moveEvent = _moveEvents.GetAndForwardToFinalMoveEvent(out sequence);
                    if (moveEvent != null)
                    {
                        Facing = (Direction)moveEvent.Facing;
                        var p = new Position3D(moveEvent.X, moveEvent.Y, moveEvent.Z);
                        if (p != CurrentPosition)
                            GoalPosition = p;
                    }
                }
            }

            // Are we moving? (if our current location != our destination, then we are moving)
            if (IsMoving)
            {
                var diff = (frameMS / MovementSpeed.TimeToCompleteMove(_entity, Facing));
                MoveSequence += diff;
                if (_entity.IsClientEntity)
                    m_playerMobile_NextMoveInMS -= frameMS;
                if (Math.Abs(GoalPosition.X - CurrentPosition.X) > 1 || Math.Abs(GoalPosition.Y - CurrentPosition.Y) > 1)
                {
                    int x, y;
                    if (CurrentPosition.X < GoalPosition.X) x = GoalPosition.X - 1;
                    else if (CurrentPosition.X > GoalPosition.X) x = GoalPosition.X + 1;
                    else x = GoalPosition.X;
                    if (CurrentPosition.Y < GoalPosition.Y) y = GoalPosition.Y - 1;
                    else if (CurrentPosition.Y > GoalPosition.Y) y = GoalPosition.Y + 1;
                    else y = GoalPosition.Y;
                    CurrentPosition.Set(x, y, CurrentPosition.Z);
                }
                if (MoveSequence < 1f)
                    CurrentPosition.Offset = new Vector3(
                        GoalPosition.X - CurrentPosition.X,
                        GoalPosition.Y - CurrentPosition.Y,
                        GoalPosition.Z - CurrentPosition.Z) * (float)MoveSequence;
                else
                {
                    CurrentPosition.Set(GoalPosition.X, GoalPosition.Y, GoalPosition.Z);
                    CurrentPosition.Offset = Vector3.zero;
                    MoveSequence = 0f;
                    if (_entity.IsClientEntity)
                        m_playerMobile_NextMoveInMS = 0;
                }
            }
            else
            {
                MoveSequence = 0f;
                if (_entity.IsClientEntity)
                    m_playerMobile_NextMoveInMS = 0d;
            }
        }

        bool PlayerMobile_CheckForMoveEvent()
        {
            if (_playerMobile_NextMove != Direction.Nothing)
            {
                var nextMove = _playerMobile_NextMove;
                _playerMobile_NextMove = Direction.Nothing;
                // get the next tile and the facing necessary to reach it.
                Direction facing;
                var targetTile = MobileMovementCheck.OffsetTile(CurrentPosition, nextMove);
                int nextZ;
                Vector2Int nextTile;
                if (PlayerMobile_GetNextTile(CurrentPosition, targetTile, out facing, out nextTile, out nextZ))
                {
                    // Check facing and send change facing message to server if necessary.
                    PlayerMobile_SendChangeFacingMsg(facing);
                    // copy the running flag to our local facing if we are running, zero it out if we are not.
                    if ((nextMove & Direction.Running) != 0) facing |= Direction.Running;
                    else facing &= Direction.FacingMask;
                    if (CurrentPosition.X != nextTile.x ||
                        CurrentPosition.Y != nextTile.y ||
                        CurrentPosition.Z != nextZ)
                    {
                        _moveEvents.AddMoveEvent(nextTile.x, nextTile.y, nextZ, (int)(facing), true);
                        return true;
                    }
                }
                // blocked
                else return false;
            }
            return false;
        }

        bool PlayerMobile_GetNextTile(Position3D current, Vector2Int goal, out Direction facing, out Vector2Int nextPosition, out int nextZ)
        {
            bool moveIsOkay;
            // attempt to move in the direction specified.
            facing = getNextFacing(current, goal);
            var initialFacing = facing;
            nextPosition = MobileMovementCheck.OffsetTile(current, facing);
            moveIsOkay = MobileMovementCheck.CheckMovement((Mobile)_entity, current, facing, out nextZ);
            // The legacy client only allows alternative direction checking when moving in a cardinal (NSEW) direction.
            // This is checked by only checked alterate directions when the initial facing modulo 2 is 1.
            // By contrast, this client allows, when enabled, alternative direction checking in any direction.
            if (NewDiagonalMovement || ((int)initialFacing % 2 == 1))
            {
                // if blocked, attempt moving in the direction 1/8 counterclockwise to the direction specified.
                if (!moveIsOkay)
                {
                    facing = (facing - 1) & Direction.ValueMask;
                    nextPosition = MobileMovementCheck.OffsetTile(current, facing);
                    moveIsOkay = MobileMovementCheck.CheckMovement((Mobile)_entity, current, facing, out nextZ);
                }
                // if blocked, attempt moving in the direction 1/8 clockwise to the direction specified.
                if (!moveIsOkay)
                {
                    facing = (facing + 2) & Direction.ValueMask;
                    nextPosition = MobileMovementCheck.OffsetTile(current, facing);
                    moveIsOkay = MobileMovementCheck.CheckMovement((Mobile)_entity, current, facing, out nextZ);
                }
            }
            // if we were able to move, then set the running flag (if necessary) and return true.
            if (moveIsOkay)
            {
                if (IsRunning)
                    facing |= Direction.Running;
                return true;
            }
            // otherwise return false, indicating that the player is blocked.
            else return false;
        }

        Direction getNextFacing(Position3D current, Vector2Int goal)
        {
            Direction facing;
            if (goal.x < current.X)
            {
                if (goal.y < current.Y) facing = Direction.Up;
                else if (goal.y > current.Y) facing = Direction.Left;
                else facing = Direction.West;
            }
            else if (goal.x > current.X)
            {
                if (goal.y < current.Y) facing = Direction.Right;
                else if (goal.y > current.Y) facing = Direction.Down;
                else facing = Direction.East;
            }
            else
            {
                if (goal.y < current.Y) facing = Direction.North;
                else if (goal.y > current.Y) facing = Direction.South;
                // We should never reach 
                else facing = Facing & Direction.FacingMask;
            }
            return facing;
        }
    }
}
