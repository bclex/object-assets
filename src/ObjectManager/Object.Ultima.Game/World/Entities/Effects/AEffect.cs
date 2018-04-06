using OA.Ultima.World.Maps;
using System.Collections.Generic;

namespace OA.Ultima.World.Entities.Effects
{
    public abstract class AEffect : AEntity
    {
        List<AEffect> _children;
        public List<AEffect> Children
        {
            get
            {
                if (_children == null)
                    _children = new List<AEffect>();
                return _children;
            }
        }

        public int ChildrenCount
        {
            get { return _children == null ? 0 : _children.Count; }
        }

        protected AEntity _source;
        protected AEntity _target;

        protected int _xSource, _ySource, _zSource;
        protected int _xTarget, _yTarget, _zTarget;

        private double _timeActiveMS;
        public int FramesActive
        {
            get
            {
                var frameOffset = (int)(_timeActiveMS / 50d); // one frame every 20ms
                return frameOffset;
            }
        }

        public int BlendMode;

        public AEffect(Map map)
            : base(Serial.Null, map) { }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            if (!IsDisposed)
                _timeActiveMS += frameMS;
        }

        public override void Dispose()
        {
            _source = null;
            _target = null;
            base.Dispose();
        }

        /// <summary>
        /// Adds an effect which will display when the parent effect despawns.
        /// </summary>
        /// <param name="effect"></param>
        public void AddChildEffect(AEffect effect)
        {
            if (_children == null)
                _children = new List<AEffect>();
            _children.Add(effect);
        }

        /// <summary>
        /// Returns the world position that is the source of the effect.
        /// </summary>
        protected void GetSource(out int x, out int y, out int z)
        {
            if (_source == null)
            {
                x = _xSource;
                y = _ySource;
                z = _zSource;
            }
            else
            {
                x = _source.X;
                y = _source.Y;
                z = _source.Z;
            }
        }

        /// <summary>
        /// Returns the world position that is the target of the effect.
        /// </summary>
        protected void GetTarget(out int X, out int Y, out int Z)
        {
            if (_target == null)
            {
                X = _xTarget;
                Y = _yTarget;
                Z = _zTarget;
            }
            else
            {
                X = _target.X;
                Y = _target.Y;
                Z = _target.Z;
            }
        }

        public void SetSource(AEntity source)
        {
            _source = source;
        }

        public void SetSource(int x, int y, int z)
        {
            _source = null;
            _xSource = x;
            _ySource = y;
            _zSource = z;
        }

        public void SetTarget(AEntity target)
        {
            _target = target;
        }

        public void SetTarget(int x, int y, int z)
        {
            _target = null;
            _xTarget = x;
            _yTarget = y;
            _zTarget = z;
        }
    }
}
