using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Core.UI;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    public class HtmlGumpling : AControl
    {
        // private variables
        private IScrollBar _scrollbar;
        private RenderedText _renderedText;
        private bool _isMouseDown;
        private int _mouseDownHREF = -1;
        private int _mouseOverHREF = -1;
        // public variables
        public int ScrollX;
        public int ScrollY;

        public string Text
        {
            get { return _renderedText.Text; }
            set { _renderedText.Text = value; }
        }

        public int Hue
        { get; set; }

        public bool HasBackground
        { get; set; }

        public bool HasScrollbar
        { get; set; }

        public bool UseFlagScrollbar
        { get; private set; }

        public override int Width
        {
            get { return base.Width; }
            set { base.Width = value; }
        }

        public HtmlGumpling(AControl parent, string[] arguements, string[] lines)
            : base(parent)
        {

            var x = int.Parse(arguements[1]);
            var y = int.Parse(arguements[2]);
            var width = int.Parse(arguements[3]);
            var height = int.Parse(arguements[4]);
            var textIndex = int.Parse(arguements[5]);
            var background = int.Parse(arguements[6]);
            var scrollbar = int.Parse(arguements[7]);
            BuildGumpling(x, y, width, height, background, scrollbar, "<font color=#000>" + lines[textIndex]);
        }

        public HtmlGumpling(AControl parent, int x, int y, int width, int height, int background, int scrollbar, string text)
            : base(parent)
        {
            BuildGumpling(x, y, width, height, background, scrollbar, text);
        }

        void BuildGumpling(int x, int y, int width, int height, int background, int scrollbar, string text)
        {
            Position = new Vector2Int(x, y);
            base.Width = width;
            Size = new Vector2Int(width, height);
            HasBackground = (background == 1) ? true : false;
            HasScrollbar = (scrollbar != 0) ? true : false;
            UseFlagScrollbar = (HasScrollbar && scrollbar == 2) ? true : false;
            _renderedText = new RenderedText(text, width - (HasScrollbar ? 15 : 0) - (HasBackground ? 8 : 0));
            if (HasBackground)
            {
                this.AddControl(new ResizePic(this, 0, 0, 0x2486, Width - (HasScrollbar ? 15 : 0), Height));
                LastControl.HandlesMouseInput = false;
            }
            if (HasScrollbar)
            {
                if (UseFlagScrollbar)
                {
                    AddControl(new ScrollFlag(this));
                    _scrollbar = LastControl as IScrollBar;
                    _scrollbar.Position = new Vector2Int(Width - 14, 0);
                }
                else
                {
                    AddControl(new ScrollBar(this));
                    _scrollbar = LastControl as IScrollBar;
                    _scrollbar.Position = new Vector2Int(Width - 14, 0);
                }
                _scrollbar.Height = Height;
                _scrollbar.MinValue = 0;
                _scrollbar.MaxValue = _renderedText.Height - Height + (HasBackground ? 8 : 0);
                ScrollY = _scrollbar.Value;
            }
            if (Width != _renderedText.Width)
                Width = _renderedText.Width;
        }

        public override void Update(double totalMS, double frameMS)
        {
            _mouseOverHREF = -1; // this value is changed every frame if we mouse over a region.
            HandlesMouseInput = (_renderedText.Regions.Count > 0);
            if (HasScrollbar)
            {
                _scrollbar.Height = Height;
                _scrollbar.MinValue = 0;
                _scrollbar.MaxValue = _renderedText.Height - Height + (HasBackground ? 8 : 0);
                ScrollY = _scrollbar.Value;
            }
            if (Width != _renderedText.Width)
                Width = _renderedText.Width;
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);

            _renderedText.MouseOverRegionID = _mouseOverHREF;
            _renderedText.IsMouseDown = _isMouseDown;
            _renderedText.Draw(spriteBatch,
                new RectInt(position.x + (HasBackground ? 4 : 0), position.y + (HasBackground ? 4 : 0),
                    Width - (HasBackground ? 8 : 0), Height - (HasBackground ? 8 : 0)), ScrollX, ScrollY);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            if (HasScrollbar)
            {
                if (_scrollbar.PointWithinControl(x - _scrollbar.Position.X, y - _scrollbar.Position.Y))
                    return true;
            }

            if (_renderedText.Regions.Count > 0)
            {
                var region = _renderedText.Regions.RegionfromPoint(new Vector2Int(x + ScrollX, y + ScrollY));
                if (region != null)
                {
                    _mouseOverHREF = region.Index;
                    return true;
                }
            }
            return false;
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            _isMouseDown = true;
            _mouseDownHREF = _mouseOverHREF;
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            _isMouseDown = false;
            _mouseDownHREF = -1;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (_mouseOverHREF != -1 && _mouseDownHREF == _mouseOverHREF)
                if (button == MouseButton.Left)
                    if (_renderedText.Regions.Region(_mouseOverHREF).HREF != null)
                        OnHtmlInputEvent(_renderedText.Regions.Region(_mouseOverHREF).HREF, MouseEvent.Click);
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (_isMouseDown && _mouseDownHREF != -1 && _mouseDownHREF != _mouseOverHREF)
                OnHtmlInputEvent(_renderedText.Regions.Region(_mouseDownHREF).HREF, MouseEvent.DragBegin);
        }

        protected override void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
            if (_mouseOverHREF != -1 && _mouseDownHREF == _mouseOverHREF)
                if (button == MouseButton.Left)
                    if (_renderedText.Regions.Region(_mouseOverHREF).HREF != null)
                        OnHtmlInputEvent(_renderedText.Regions.Region(_mouseOverHREF).HREF, MouseEvent.DoubleClick);
        }
    }
}
