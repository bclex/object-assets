using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using System;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    public class ButtonResizable : AControl
    {
        const int GumpUp = 9400, GumpDown = 9500, GumpOver = 9450;
        ResizePic[] _gumps = new ResizePic[3];
        bool _isMouseDown;
        RenderedText _caption;
        Action _onClickRight;
        readonly Action _onClickLeft;

        internal bool IsMouseDownOnThis => _isMouseDown;

        public string Caption
        {
            get { return _caption.Text; }
            set { _caption.Text = $"<outline><span style='font-family: uni1;' color='#ddd'>{value}"; }
        }

        public ButtonResizable(AControl parent, int x, int y, int width, int height, string caption, Action onClickLeft = null, Action onClickRight = null)
            : base(parent)
        {
            HandlesMouseInput = true;
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);
            _caption = new RenderedText(string.Empty, width, true);
            Caption = caption;
            _gumps[0] = AddControl(new ResizePic(null, 0, 0, GumpUp, Width, Height), 1) as ResizePic;
            _gumps[1] = AddControl(new ResizePic(null, 0, 0, GumpDown, Width, Height), 2) as ResizePic;
            _gumps[2] = AddControl(new ResizePic(null, 0, 0, GumpOver, Width, Height), 3) as ResizePic;
            _onClickLeft = onClickLeft;
            _onClickRight = onClickRight;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (IsMouseDownOnThis) ActivePage = 2;
            else if (IsMouseOver) ActivePage = 3;
            else ActivePage = 1;
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
            if (Caption != string.Empty)
            {
                var yoffset = IsMouseDownOnThis ? 2 : 1;
                _caption.Draw(spriteBatch, new Vector2Int(
                    position.x + (Width - _caption.Width) / 2,
                    position.y + yoffset + (Height - _caption.Height) / 2));
            }
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            return true;
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                _isMouseDown = true;
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                _isMouseDown = false;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    _onClickLeft?.Invoke();
                    break;
                case MouseButton.Right:
                    _onClickRight?.Invoke();
                    break;
            }
        }
    }
}