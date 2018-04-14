using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.UI;
using OA.Ultima.Resources;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class DropDownList : AControl
    {
        public int Index;

        int _width;
        List<string> _items;
        int _visibleItems;
        bool _canBeNull;

        ResizePic _resize;
        TextLabelAscii _label;

        bool _listOpen;
        ResizePic _openResizePic;
        ScrollBar _openScrollBar;
        TextLabelAscii[] _openLabels;

        const int hue_Text = 1107;
        const int hue_TextSelected = 588;
        readonly IFont _font;

        DropDownList(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
            _font = Service.Get<IResourceProvider>().GetAsciiFont(1);
        }

        public DropDownList(AControl parent, int x, int y, int width, string[] items, int itemsVisible, int index, bool canBeNull)
            : this(parent)
        {
            BuildGumpling(x, y, width, items, itemsVisible, index, canBeNull);
        }

        void BuildGumpling(int x, int y, int width, string[] items, int itemsVisible, int index, bool canBeNull)
        {
            Position = new Vector2Int(x, y);
            _items = new List<string>(items);
            _width = width;
            Index = index;
            _visibleItems = itemsVisible;
            _canBeNull = canBeNull;

            _resize = (ResizePic)AddControl(new ResizePic(this, 0, 0, 3000, _width, _font.Height + 8), 0);
            _resize.MouseClickEvent += onClickClosedList;
            _resize.MouseOverEvent += onMouseOverClosedList;
            _resize.MouseOutEvent += onMouseOutClosedList;
            _label = (TextLabelAscii)AddControl(new TextLabelAscii(this, 4, 5, 1, hue_Text, string.Empty), 0);
            AddControl(new GumpPic(this, width - 22, 5, 2086, 0), 0);
        }

        public override void Dispose()
        {
            if (_resize != null)
            {
                _resize.MouseClickEvent -= onClickClosedList;
                _resize.MouseOverEvent -= onMouseOverClosedList;
                _resize.MouseOutEvent -= onMouseOutClosedList;
            }
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (Index < 0 || Index >= _items.Count)
                Index = -1;

            if (_listOpen)
            {
                // if we have moused off the open list, close it. We check to see if the mouse is over:
                // the resizepic for the closed list (because it takes one update cycle to open the list)
                // the resizepic for the open list, and the scroll bar if it is loaded.
                if (UserInterface.MouseOverControl != _openResizePic &&
                    UserInterface.MouseOverControl != _resize &&
                    (_openScrollBar != null && UserInterface.MouseOverControl != _openScrollBar))
                {
                    closeOpenList();
                }
                else
                {
                    // update the visible items
                    var itemOffset = _openScrollBar == null ? 0 : _openScrollBar.Value;
                    for (var i = 0; i < _visibleItems; i++)
                        _openLabels[i].Text = (i + itemOffset < 0) ? string.Empty : _items[i + itemOffset];
                }
            }
            else
            {
                if (Index == -1) _label.Text = "Click here";
                else _label.Text = _items[Index];
            }
            base.Update(totalMS, frameMS);
        }

        void closeOpenList()
        {
            _listOpen = false;
            if (_openResizePic != null)
            {
                _openResizePic.MouseClickEvent -= onClickOpenList;
                _openResizePic.MouseOverEvent -= onMouseOverOpenList;
                _openResizePic.MouseOutEvent -= onMouseOutOpenList;
                _openResizePic.Dispose();
                _openResizePic = null;
            }
            if (_openScrollBar != null)
                _openScrollBar.Dispose();
            for (int i = 0; i < _visibleItems; i++)
                _openLabels[i].Dispose();
        }

        void onClickClosedList(AControl control, int x, int y, MouseButton button)
        {
            _listOpen = true;
            _openResizePic = new ResizePic(Parent, X, Y, 3000, _width, _font.Height * _visibleItems + 8);
            _openResizePic.MouseClickEvent += onClickOpenList;
            _openResizePic.MouseOverEvent += onMouseOverOpenList;
            _openResizePic.MouseOutEvent += onMouseOutOpenList;
            ((Gump)Parent).AddControl(_openResizePic, this.Page);

            if (_visibleItems > _items.Count)
                _visibleItems = _items.Count;

            // only show the scrollbar if we need to scroll
            if (_visibleItems < _items.Count)
            {
                _openScrollBar = new ScrollBar(Parent, X + _width - 20, Y + 4, _font.Height * _visibleItems, (_canBeNull ? -1 : 0), _items.Count - _visibleItems, Index);
                ((Gump)Parent).AddControl(_openScrollBar, this.Page);
            }
            _openLabels = new TextLabelAscii[_visibleItems];
            for (var i = 0; i < _visibleItems; i++)
            {
                _openLabels[i] = new TextLabelAscii(Parent, X + 4, Y + 5 + _font.Height * i, 1, 1106, string.Empty);
                ((Gump)Parent).AddControl(_openLabels[i], this.Page);
            }
        }

        void onMouseOverClosedList(AControl control, int x, int y)
        {
            _label.Hue = hue_TextSelected;
        }

        void onMouseOutClosedList(AControl control, int x, int y)
        {
            _label.Hue = hue_Text;
        }

        void onClickOpenList(AControl control, int x, int y, MouseButton button)
        {
            var indexOver = getOpenListIndexFromPoint(x, y);
            if (indexOver != -1)
                Index = indexOver + (_openScrollBar == null ? 0 : _openScrollBar.Value);
            closeOpenList();
        }

        void onMouseOverOpenList(AControl control, int x, int y)
        {
            var indexOver = getOpenListIndexFromPoint(x, y);
            for (var i = 0; i < _openLabels.Length; i++)
                if (i == indexOver) _openLabels[i].Hue = hue_TextSelected;
                else _openLabels[i].Hue = hue_Text;
        }

        void onMouseOutOpenList(AControl control, int x, int y)
        {
            for (var i = 0; i < _openLabels.Length; i++)
                _openLabels[i].Hue = hue_Text;
        }

        int getOpenListIndexFromPoint(int x, int y)
        {
            var r = new RectInt(4, 5, _width - 20, _font.Height);
            for (var i = 0; i < _openLabels.Length; i++)
            {
                if (r.Contains(new Vector2Int(x, y)))
                    return i;
                r.y += _font.Height;
            }
            return -1;
        }
    }
}
