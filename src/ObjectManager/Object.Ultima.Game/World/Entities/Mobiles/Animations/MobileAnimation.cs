using OA.Core;
using OA.Ultima.Resources;

namespace OA.Ultima.World.Entities.Mobiles.Animations
{
    /// <summary>
    /// Maintains and updates a mobile's animations. Receives animations from server, and when moving, updates the movement animation.
    /// TODO: This class needs a serious refactor.
    /// </summary>
    public class MobileAnimation
    {
        Mobile Parent;
        MobileAction _action;
        bool _actionCanBeInteruptedByStand;
        
        int _actionIndex;
        public int ActionIndex
        {
            get
            {
                if (Parent.Body == 5 || Parent.Body == 6) // birds have weird action indexes. not sure if this is correct.
                    if (_actionIndex > 8)
                        return _actionIndex + 8;
                return _actionIndex;
            }
        }

        public bool IsAnimating
        {
            get
            {
                if (!_actionCanBeInteruptedByStand &&
                    (_action == MobileAction.None ||
                    _action == MobileAction.Stand || 
                    _action == MobileAction.Walk || 
                    _action == MobileAction.Run))
                    return false;
                return true;
            }
        }

        public bool IsStanding
        {
            get { return _action == MobileAction.Stand; }
        }

        float _animationFrame;
        public float AnimationFrame
        {
            get
            {
                if (_animationFrame >= _frameCount) return _frameCount - 0.1f;
                else return _animationFrame;
            }
        }

        // We use these variables to 'hold' the last frame of an animation before 
        // switching to Stand Action.
        bool _isAnimatationPaused;
        int _animationPausedMS;
        private int PauseAnimationMS
        {
            get
            {
                if (Parent.IsClientEntity) return 100;
                else return 350;
            }
        }

        public MobileAnimation(Mobile parent)
        {
            Parent = parent;
        }

        public void Update(double frameMS)
        {
            // create a local copy of ms since last update.
            var msSinceLastUpdate = (int)frameMS;

            // If we are holding the current animation, then we should wait until our hold time is over
            // before switching to the queued Stand animation.
            if (_isAnimatationPaused)
            {
                _animationPausedMS -= msSinceLastUpdate;
                if (_animationPausedMS >= 0)
                    // we are still holding. Do not update the current Animation frame.
                    return;
                else
                {
                    // hold time is over, continue to Stand animation.
                    UnPauseAnimation();
                    _action = MobileAction.Stand;
                    _actionIndex = ActionTranslator.GetActionIndex(Parent, MobileAction.Stand);
                    _animationFrame = 0f;
                    _frameCount = 1;
                    _frameDelay = 0;
                }
            }

            if (_action != MobileAction.None)
            {
                var msPerFrame = ((900f * (_frameDelay + 1)) / _frameCount);
                // Mounted movement is ~2x normal frame rate
                if (Parent.IsMounted && ((_action == MobileAction.Walk) || (_action == MobileAction.Run)))
                    msPerFrame /= 2.272727f;
                if (msPerFrame < 0)
                    return;
                _animationFrame += (float)(frameMS / msPerFrame);
                if (UltimaGameSettings.Audio.FootStepSoundOn)
                {
                    if (_action == MobileAction.Walk || _action == MobileAction.Run)
                        MobileSounds.DoFootstepSounds(Parent as Mobile, _animationFrame / _frameCount);
                    else
                        MobileSounds.ResetFootstepSounds(Parent as Mobile);
                }

                // When animations reach their last frame, if we are queueing to stand, then
                // hold the animation on the last frame.
                if (_animationFrame >= _frameCount)
                {
                    if (_repeatCount > 0)
                    {
                        _animationFrame -= _frameCount;
                        _repeatCount--;
                    }
                    else
                    {
                        // any requested actions are ended.
                        _actionCanBeInteruptedByStand = false;
                        // Hold the last frame of the current action if animation is not Stand.
                        if (_action == MobileAction.Stand)
                            _animationFrame = 0;
                        else
                        {
                            // for most animations, hold the last frame. For Move animations, cycle through.
                            if (_action == MobileAction.Run || _action == MobileAction.Walk)
                                _animationFrame -= _frameCount;
                            else
                                _animationFrame = _frameCount - 0.001f;
                            PauseAnimation();
                        }
                            
                    }
                }
            }
        }

        /// <summary>
        /// Immediately clears all animation data, sets mobile action to stand.
        /// </summary>
        public void Clear()
        {
            _action = MobileAction.Stand;
            _animationFrame = 0;
            _frameCount = 1;
            _frameDelay = 0;
            _isAnimatationPaused = true;
            _repeatCount = 0;
            _actionIndex = ActionTranslator.GetActionIndex(Parent, MobileAction.Stand);
        }

        public void UpdateAnimation()
        {
            animate(_action, _actionIndex, 0, false, false, 0, false);
        }

        public void Animate(MobileAction action)
        {
            var actionIndex = ActionTranslator.GetActionIndex(Parent, action);
            animate(action, actionIndex, 0, false, false, 0, false);
        }

        public void Animate(int requestedIndex, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            // note that frameCount is NOT used. Not sure if this counts as a bug.
            var action = ActionTranslator.GetActionFromIndex(Parent.Body, requestedIndex);
            var actionIndex = ActionTranslator.GetActionIndex(Parent, action, requestedIndex);
            animate(action, actionIndex, repeatCount, reverse, repeat, delay, true);
        }

        int _frameCount, _frameDelay, _repeatCount;
        private void animate(MobileAction action, int actionIndex, int repeatCount, bool reverse, bool repeat, int delay, bool isRequestedAction)
        {
            if (_action == action)
                if (_isAnimatationPaused)
                    UnPauseAnimation();

            if (isRequestedAction)
                _actionCanBeInteruptedByStand = true;

            if (_action != action || _actionIndex != actionIndex)
            {
                // If we are switching from any action to a stand action, then hold the last frame of the 
                // current animation for a moment. Only Stand actions are held; thus when any hold ends,
                // then we know we were holding for a Stand action.
                if (!(_action == MobileAction.None) && (action == MobileAction.Stand && _action != MobileAction.Stand))
                {
                    if (_action != MobileAction.None)
                        PauseAnimation();
                }
                else
                {
                    _action = action;
                    UnPauseAnimation();
                    _actionIndex = actionIndex;
                    _animationFrame = 0f;

                    // get the frames of the base body - we need to count the number of frames in this animation.
                    var provider = Service.Get<IResourceProvider>();
                    int body = Parent.Body, hue = 0;
                    var frames = provider.GetAnimation(body, ref hue, actionIndex, (int)Parent.DrawFacing);
                    if (frames != null)
                    {
                        _frameCount = frames.Length;
                        _frameDelay = delay;
                        if (repeat == false) _repeatCount = 0;
                        else _repeatCount = repeatCount;
                    }
                }
            }
        }

        private void PauseAnimation()
        {
            if (!_isAnimatationPaused)
            {
                _isAnimatationPaused = true;
                _animationPausedMS = PauseAnimationMS;
            }
        }

        private void UnPauseAnimation()
        {
            _isAnimatationPaused = false;
        }
    }
}
