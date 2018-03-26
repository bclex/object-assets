using OA.Core.Input;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Core.UI;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Core.UI
{
    /// <summary>
    /// A one dimensional list of rendered text objects which can be scrolled (up and down) and
    /// only display within a designated window.
    /// </summary>
    class RenderedTextList : AControl
    {
        readonly List<RenderedText> _entries;
        IScrollBar _scrollBar;

        bool _isMouseDown;
        int _mouseDownHREF = -1;
        int _mouseDownText = -1;
        int _mouseOverHREF = -1;
        int _mouseOverText = -1;

        /// <summary>
        /// Creates a RenderedTextList.
        /// Note that the scrollBarControl must be created and added to the parent gump before passing it as a param.
        /// </summary>
        public RenderedTextList(AControl parent, int x, int y, int width, int height, IScrollBar scrollBarControl)
            : base(parent)
        {
            _scrollBar = scrollBarControl;
            _scrollBar.IsVisible = false;
            HandlesMouseInput = true;
            Position = new Vector2Int(x, y);
            Width = width;
            Height = height;
            _entries = new List<RenderedText>();
        }

        protected RenderedTextList(AControl parent) : base(parent)
        {
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
            var p = new Vector2Int(position.X, position.Y);
            var height = 0;
            var maxheight = _scrollBar.Value + _scrollBar.Height;
            for (var i = 0; i < _entries.Count; i++)
            {
                if (height + _entries[i].Height <= _scrollBar.Value)
                    // this entry is above the renderable area.
                    height += _entries[i].Height;
                else if (height + _entries[i].Height <= maxheight)
                {
                    var y = height - _scrollBar.Value;
                    if (y < 0)
                    {
                        // this entry starts above the renderable area, but exists partially within it.
                        _entries[i].Draw(spriteBatch, new RectInt(p.X, position.Y, _entries[i].Width, _entries[i].Height + y), 0, -y);
                        p.Y += _entries[i].Height + y;
                    }
                    else
                    {
                        // this entry is completely within the renderable area.
                        _entries[i].Draw(spriteBatch, p);
                        p.Y += _entries[i].Height;
                    }
                    height += _entries[i].Height;
                }
                else
                {
                    var y = maxheight - height;
                    _entries[i].Draw(spriteBatch, new RectInt(p.X, position.Y + _scrollBar.Height - y, _entries[i].Width, y), 0, 0);
                    // can't fit any more entries - so we break!
                    break;
                }
            }
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            _scrollBar.Position = new Vector2Int(X + Width - 14, Y);
            _scrollBar.Height = Height;
            CalculateScrollBarMaxValue();
            _scrollBar.IsVisible = _scrollBar.MaxValue > _scrollBar.MinValue;
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            _mouseOverText = -1; // this value is changed every frame if we mouse over a region.
            _mouseOverHREF = -1; // this value is changed every frame if we mouse over a region.
            var height = 0;
            for (var i = 0; i < _entries.Count; i++)
            {
                var rendered = _entries[i];
                if (rendered.Regions.Count > 0)
                {
                    var region = rendered.Regions.RegionfromPoint(new Vector2Int(x, y - height + _scrollBar.Value));
                    if (region != null)
                    {
                        _mouseOverText = i;
                        _mouseOverHREF = region.Index;
                        return true;
                    }
                }
                height += rendered.Height;
            }
            return false;
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            _isMouseDown = true;
            _mouseDownText = _mouseOverText;
            _mouseDownHREF = _mouseOverHREF;
            if (button == MouseButton.Left)
                if (_entries[_mouseDownText].Regions.Region(_mouseDownHREF).HREF != null)
                    OnHtmlInputEvent(_entries[_mouseDownText].Regions.Region(_mouseDownHREF).HREF, MouseEvent.Down);
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                if (_entries[_mouseDownText].Regions.Region(_mouseDownHREF).HREF != null)
                    OnHtmlInputEvent(_entries[_mouseDownText].Regions.Region(_mouseDownHREF).HREF, MouseEvent.Up);
            _isMouseDown = false;
            _mouseDownText = -1;
            _mouseDownHREF = -1;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (_mouseOverText != -1 && _mouseOverHREF != -1 && _mouseDownText == _mouseOverText && _mouseDownHREF == _mouseOverHREF)
                if (button == MouseButton.Left)
                    if (_entries[_mouseOverText].Regions.Region(_mouseOverHREF).HREF != null)
                        OnHtmlInputEvent(_entries[_mouseOverText].Regions.Region(_mouseOverHREF).HREF, MouseEvent.Click);
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (_isMouseDown && _mouseDownText != -1 && _mouseDownHREF != -1 && _mouseDownHREF != _mouseOverHREF)
                OnHtmlInputEvent(_entries[_mouseDownText].Regions.Region(_mouseDownHREF).HREF, MouseEvent.DragBegin);
        }

        private void CalculateScrollBarMaxValue()
        {
            var maxValue = _scrollBar.Value == _scrollBar.MaxValue;
            var height = 0;
            for (var i = 0; i < _entries.Count; i++)
                height += _entries[i].Height;
            height -= _scrollBar.Height;
            if (height > 0)
            {
                _scrollBar.MaxValue = height;
                if (maxValue)
                    _scrollBar.Value = _scrollBar.MaxValue;
            }
            else
            {
                _scrollBar.MaxValue = 0;
                _scrollBar.Value = 0;
            }
        }

        public void AddEntry(string text)
        {
            var maxScroll = (_scrollBar.Value == _scrollBar.MaxValue);
            while (_entries.Count > 99)
                _entries.RemoveAt(0);
            _entries.Add(new RenderedText(text, Width - 18));
            _scrollBar.MaxValue += _entries[_entries.Count - 1].Height;
            if (maxScroll)
                _scrollBar.Value = _scrollBar.MaxValue;
        }

        public void UpdateEntry(int index, string text)
        {
            if (index < 0 || index >= _entries.Count)
            {
                Utils.Error($"Bad index in RenderedTextList.UpdateEntry: {index}");
                return;
            }
            _entries[index].Text = text;
            CalculateScrollBarMaxValue();
        }
    }
}
