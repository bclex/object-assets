using OA.Ultima.Data;
using OA.Ultima.World.EntityViews;
using OA.Ultima.World.Maps;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.World.Entities
{
    /// <summary>
    /// Base class for all entities which exist in the world model.
    /// </summary>
    public abstract class AEntity
    {
        // ============================================================================================================
        // Properties
        // ============================================================================================================

        public readonly Serial Serial;

        public PropertyList PropertyList = new PropertyList();

        public bool IsDisposed;

        public bool IsClientEntity;

        public virtual int Hue { set; get; }

        public virtual string Name
        {
            get { return "AEntity"; }
            set { } // do nothing - exists only to allow inheriting class to override.
        }

        // ============================================================================================================
        // Position
        // ============================================================================================================

        public Map Map { get; private set; }

        public void SetMap(Map map)
        {
            if (map != Map)
            {
                Map = map;
                Position.Tile = Position3D.NullTile;
            }
        }

        MapTile _tile;
        protected MapTile Tile
        {
            set
            {
                if (_tile != null)
                    _tile.OnExit(this);
                _tile = value;
                if (_tile != null)
                    _tile.OnEnter(this);
                else
                {
                    if (!IsClientEntity)
                        if (!IsDisposed)
                            Dispose();
                }
            }
            get { return _tile; }
        }

        protected virtual void OnTileChanged(int x, int y)
        {
            if (Map != null)
            {
                if (IsClientEntity && Map.Index >= 0)
                    Map.CenterPosition = new Vector2Int(x, y);
                Tile = Map.GetMapTile(x, y);
            }
            else
            {
                if (!IsClientEntity)
                    Dispose();
            }
        }

        public int X
        {
            get { return Position.X; }
        }

        public int Y
        {
            get { return Position.Y; }
        }

        public int Z
        {
            get { return Position.Z; }
        }

        Position3D _position;
        public virtual Position3D Position { get { return _position; } }

        // ============================================================================================================
        // Methods
        // ============================================================================================================

        public AEntity(Serial serial, Map map)
        {
            Serial = serial;
            Map = map;
            _position = new Position3D(OnTileChanged);
        }

        public virtual void Update(double frameMS)
        {
            if (IsDisposed)
                return;
            InternalUpdateOverheads(frameMS);
        }

        public virtual void Dispose()
        {
            _onDisposed?.Invoke(this);
            _onDisposed = null;
            _onUpdated = null;
            IsDisposed = true;
            Tile = null;
        }

        public override string ToString()
        {
            return Serial.ToString();
        }

        // ============================================================================================================
        // Callbacks
        // ============================================================================================================

        protected Action<AEntity> _onUpdated;
        protected Action<AEntity> _onDisposed;

        public void SetCallbacks(Action<AEntity> onUpdate, Action<AEntity> onDispose)
        {
            if (onUpdate != null)
                _onUpdated += onUpdate;
            if (onDispose != null)
                _onDisposed += onDispose;
        }

        public void ClearCallBacks(Action<AEntity> onUpdate, Action<AEntity> onDispose)
        {
            if (_onUpdated.GetInvocationList().Contains(onUpdate))
                _onUpdated -= onUpdate;
            if (_onDisposed.GetInvocationList().Contains(onDispose))
                _onDisposed -= onDispose;
        }

        // ============================================================================================================
        // Draw and View handling code
        // ============================================================================================================

        AEntityView _view;

        protected virtual AEntityView CreateView()
        {
            return null;
        }

        public AEntityView GetView()
        {
            if (_view == null)
                _view = CreateView();
            return _view;
        }

        internal virtual void Draw(MapTile tile, Position3D position)
        {
        }

        // ============================================================================================================
        // Overhead handling code (labels, chat, etc.)
        // ============================================================================================================

        List<Overhead> _overheads = new List<Overhead>();
        public List<Overhead> Overheads
        {
            get { return _overheads; }
        }

        public Overhead AddOverhead(MessageTypes msgType, string text, int fontID, int hue, bool asUnicode)
        {
            Overhead overhead;
            text = string.Format("<outline style='font-family: {2}{0};'>{1}", fontID, text, asUnicode ? "uni" : "ascii");
            for (var i = 0; i < _overheads.Count; i++)
            {
                overhead = _overheads[i];
                // is this overhead an already active label?
                if (msgType == MessageTypes.Label && overhead.Text == text && overhead.MessageType == msgType && !overhead.IsDisposed)
                {
                    // reset the timer for the object so it lasts longer.
                    overhead.ResetTimer();
                    // update hue?
                    overhead.Hue = hue;
                    // insert it at the bottom of the queue so it displays closest to the player.
                    _overheads.RemoveAt(i);
                    InternalInsertOverhead(overhead);
                    return overhead;
                }
            }
            overhead = new Overhead(this, msgType, text);
            overhead.Hue = hue;
            InternalInsertOverhead(overhead);
            return overhead;
        }

        void InternalInsertOverhead(Overhead overhead)
        {
            if (_overheads.Count == 0 || _overheads[0].MessageType != MessageTypes.Label)
                _overheads.Insert(0, overhead);
            else _overheads.Insert(1, overhead);
        }

        internal void InternalDrawOverheads(MapTile tile, Position3D position)
        {
            // base entities do not draw, but they can have overheads, so we draw those.
            foreach (var overhead in _overheads)
                if (!overhead.IsDisposed)
                    overhead.Draw(tile, position);
        }

        void InternalUpdateOverheads(double frameMS)
        {
            // update overheads
            foreach (var overhead in _overheads)
                overhead.Update(frameMS);
            // remove disposed of overheads.
            for (var i = 0; i < _overheads.Count; i++)
                if (_overheads[i].IsDisposed)
                {
                    _overheads.RemoveAt(i);
                    i--;
                }
        }

        // Update range
        public virtual int GetMaxUpdateRange()
        {
            return 18;
        }
    }
}
