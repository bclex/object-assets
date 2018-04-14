using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class ExpandableScroll : Gump
    {
        GumpPic _gumplingTop, _gumplingBottom;
        GumpPicTiled _gumplingMiddle;
        Button _gumplingExpander;
        
        int _expandableScrollHeight;
        const int _expandableScrollHeight_Min = 274; // this is the min from the client.
        const int _expandableScrollHeight_Max = 1000; // arbitrary large number.

        int _gumplingMidY { get { return _gumplingTop.Height; } }
        int _gumplingMidHeight { get { return _expandableScrollHeight - _gumplingTop.Height - _gumplingBottom.Height - (_gumplingExpander != null ? _gumplingExpander.Height : 0); } }
        int _gumplingBottomY { get { return _expandableScrollHeight - _gumplingBottom.Height - (_gumplingExpander != null ? _gumplingExpander.Height : 0); } }
        int _gumplingExpanderX { get { return (Width - (_gumplingExpander != null ? _gumplingExpander.Width : 0)) / 2; } }
        int _gumplingExpanderY { get { return _expandableScrollHeight - (_gumplingExpander != null ? _gumplingExpander.Height : 0) - _gumplingExpanderY_Offset; } }
        const int _gumplingExpanderY_Offset = 2; // this is the gap between the pixels of the btm Control texture and the height of the btm Control texture.
        const int _gumplingExpander_ButtonID = 0x7FBEEF;

        bool _isResizable = true;
        bool _isExpanding;
        int _isExpanding_InitialX, _isExpanding_InitialY, _isExpanding_InitialHeight;

        public ExpandableScroll(AControl parent, int x, int y, int height, bool isResizable = true)
            : base(0, 0)
        {
            Parent = parent;
            Position = new Vector2Int(x, y);
            _expandableScrollHeight = height;
            _isResizable = isResizable;
            MakeThisADragger();
        }

        protected override void OnInitialize()
        {
            _gumplingTop = (GumpPic)AddControl(new GumpPic(this, 0, 0, 0x0820, 0));
            _gumplingMiddle = (GumpPicTiled)AddControl(new GumpPicTiled(this, 0, 0, 0, 0, 0x0822));
            _gumplingBottom = (GumpPic)AddControl(new GumpPic(this, 0, 0, 0x0823, 0));

            if (_isResizable)
            {
                _gumplingExpander = (Button)AddControl(new Button(this, 0, 0, 0x082E, 0x82F, ButtonTypes.Activate, 0, _gumplingExpander_ButtonID));
                _gumplingExpander.MouseDownEvent += expander_OnMouseDown;
                _gumplingExpander.MouseUpEvent += expander_OnMouseUp;
                _gumplingExpander.MouseOverEvent += expander_OnMouseOver;
            }
        }

        public override void Dispose()
        {
            if (_gumplingExpander != null)
            {
                _gumplingExpander.MouseDownEvent -= expander_OnMouseDown;
                _gumplingExpander.MouseUpEvent -= expander_OnMouseUp;
                _gumplingExpander.MouseOverEvent -= expander_OnMouseOver;
                _gumplingExpander.Dispose();
                _gumplingExpander = null;
            }
            base.Dispose();
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            var position = new Vector2Int(x + ScreenX, y + ScreenY);
            if (_gumplingTop.HitTest(position, true) != null)
                return true;
            if (_gumplingMiddle.HitTest(position, true) != null)
                return true;
            if (_gumplingBottom.HitTest(position, true) != null)
                return true;
            if (_isResizable && _gumplingExpander.HitTest(position, true) != null)
                return true;
            return false;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_expandableScrollHeight < _expandableScrollHeight_Min)
                _expandableScrollHeight = _expandableScrollHeight_Min;
            if (_expandableScrollHeight > _expandableScrollHeight_Max)
                _expandableScrollHeight = _expandableScrollHeight_Max;
            if (_gumplingTitleGumpIDDelta)
            {
                _gumplingTitleGumpIDDelta = false;
                if (_gumplingTitle != null)
                    _gumplingTitle.Dispose();
                _gumplingTitle = (GumpPic)AddControl(new GumpPic(this, 0, 0, _gumplingTitleGumpID, 0));
            }
            if (!_gumplingTop.IsInitialized)
                IsVisible = false;
            else 
            {
                IsVisible = true;
                _gumplingTop.Position = new Vector2Int(0, 0);
                _gumplingMiddle.Position = new Vector2Int(17, _gumplingMidY);
                _gumplingMiddle.Width = 263;
                _gumplingMiddle.Height = _gumplingMidHeight;
                _gumplingBottom.Position = new Vector2Int(17, _gumplingBottomY);
                if (_isResizable)
                    _gumplingExpander.Position = new Vector2Int(_gumplingExpanderX, _gumplingExpanderY);
                if (_gumplingTitle != null && _gumplingTitle.IsInitialized)
                    _gumplingTitle.Position = new Vector2Int(
                        (_gumplingTop.Width - _gumplingTitle.Width) / 2,
                        (_gumplingTop.Height - _gumplingTitle.Height) / 2);
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
        }

        void expander_OnMouseDown(AControl control, int x, int y, MouseButton button)
        {
            y += _gumplingExpander.Y + ScreenY - Y;
            if (button == MouseButton.Left)
            {
                _isExpanding = true;
                _isExpanding_InitialHeight = _expandableScrollHeight;
                _isExpanding_InitialX = x;
                _isExpanding_InitialY = y;
            }
        }

        void expander_OnMouseUp(AControl control, int x, int y, MouseButton button)
        {
            y += _gumplingExpander.Y + ScreenY - Y;
            if (_isExpanding)
            {
                _isExpanding = false;
                _expandableScrollHeight = _isExpanding_InitialHeight + (y - _isExpanding_InitialY);
            }
        }

        void expander_OnMouseOver(AControl control, int x, int y)
        {
            y += _gumplingExpander.Y + ScreenY - Y;
            if (_isExpanding && (y != _isExpanding_InitialY))
                _expandableScrollHeight = _isExpanding_InitialHeight + (y - _isExpanding_InitialY);
        }

        bool _gumplingTitleGumpIDDelta;
        int _gumplingTitleGumpID;
        GumpPic _gumplingTitle;
        public int TitleGumpID { set { _gumplingTitleGumpID = value; _gumplingTitleGumpIDDelta = true; } }
    }
}
