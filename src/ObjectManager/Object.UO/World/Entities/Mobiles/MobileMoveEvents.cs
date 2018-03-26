using System;

namespace OA.Ultima.World.Entities.Mobiles
{
    /// <summary>
    /// Queues moves and maintains the fastwalk key and current sequence value.
    /// TODO: This class needs a serious refactor.
    /// </summary>
    class MobileMoveEvents
    {
        private int _lastSequenceAck;
        private int _sequenceQueued;
        private int _sequenceNextSend;
        private int _fastWalkKey;
        MobileMoveEvent[] _history;

        public bool SlowSync => (_sequenceNextSend > _lastSequenceAck + 4);

        public MobileMoveEvents()
        {
            ResetMoveSequence();
        }

        public void ResetMoveSequence()
        {
            _sequenceQueued = 0;
            _lastSequenceAck = -1;
            _sequenceNextSend = 0;
            _fastWalkKey = new Random().Next(int.MinValue, int.MaxValue);
            _history = new MobileMoveEvent[256];
        }

        public void AddMoveEvent(int x, int y, int z, int facing, bool createdByPlayerInput)
        {
            var moveEvent = new MobileMoveEvent(x, y, z, facing, _fastWalkKey);
            moveEvent.CreatedByPlayerInput = createdByPlayerInput;
            _history[_sequenceQueued] = moveEvent;
            _sequenceQueued += 1;
            if (_sequenceQueued > byte.MaxValue)
                _sequenceQueued = 1;
        }

        public MobileMoveEvent GetNextMoveEvent(out int sequence)
        {
            if (_history[_sequenceNextSend] != null)
            {
                var m = _history[_sequenceNextSend];
                _history[_sequenceNextSend] = null;
                sequence = _sequenceNextSend;
                _sequenceNextSend++;
                if (_sequenceNextSend > byte.MaxValue)
                    _sequenceNextSend = 1;
                return m;
            }
            else
            {
                sequence = 0;
                return null;
            }
        }

        public MobileMoveEvent PeekNextMoveEvent()
        {
            if (_history[_sequenceNextSend] != null)
            {
                var m = _history[_sequenceNextSend];
                return m;
            }
            return null;
        }

        /// <summary>
        /// Retrieves the last move event received. If more than one move event is in the queue, forwards the mobile to the
        /// second-to-last move event destination.
        /// </summary>
        /// <param name="sequence">The sequence index of the final move event. The player mobile needs to track this so
        /// it can send it with each move event and keep in sync with the server.</param>
        /// <returns>The final move event! Null if no move events in the queue</returns>
        public MobileMoveEvent GetAndForwardToFinalMoveEvent(out int sequence)
        {
            MobileMoveEvent moveEvent = null;
            MobileMoveEvent moveEventNext, moveEventLast;
            while ((moveEventNext = GetNextMoveEvent(out sequence)) != null)
            {
                // save the last moveEvent.
                moveEventLast = moveEvent;
                // get the next move event, erasing it from the queued move events.
                moveEvent = moveEventNext;
                // get the next move event, peeking to see if it is null (this does not erase it from the queue).
                moveEventNext = PeekNextMoveEvent();
                // we want to save move events that are the last move event in the queue, and are only facing changes.
                if (moveEventNext == null && moveEventLast != null &&
                    moveEvent.X == moveEventLast.X && moveEvent.Y == moveEventLast.Y && moveEventLast.Z == moveEvent.Z &&
                    moveEvent.Facing != moveEventLast.Facing)
                {
                    // re-queue the final facing change, and return the second-to-last move event.
                    AddMoveEvent(moveEvent.X, moveEvent.Y, moveEvent.Z, moveEvent.Facing, false);
                    return moveEventLast;
                }
            }
            return moveEvent;
        }

        public void AcknowledgeMoveRequest(int sequence)
        {
            _history[sequence] = null;
            _lastSequenceAck = sequence;
        }

        public void RejectMoveRequest(int sequence, out int x, out int y, out int z, out int facing)
        {
            if (_history[sequence] != null)
            {
                var e = _history[sequence];
                x = e.X;
                y = e.Y;
                z = e.Z;
                facing = e.Facing;
            }
            else x = y = z = facing = -1;
            ResetMoveSequence();
        }
    }
}
