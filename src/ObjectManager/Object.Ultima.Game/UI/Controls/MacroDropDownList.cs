using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.UI;
using OA.Ultima.Resources;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class MacroDropDownList : AControl
    {
        public int Index;

        public GumpPic ScrollButton;
        public ResizePic _resizePic;

        public bool IsFirstvisible = true;
        public List<string> Items;

        readonly int _width;
        bool _canBeNull;
        bool _IsListOpen;
        int _visibleItems = -1;

        TextLabelAscii _label;
        ResizePic _openResizePic;
        ScrollBar _openScrollBar;
        TextLabelAscii[] _openLabels;

        const int hue_Text = 1107;
        const int hue_TextSelected = 588;

        readonly IFont _font;

        public MacroDropDownList(AControl parent, int x, int y, int width, string[] items, int itemsVisible, int index, bool canBeNull, int ID, bool firstVisible)
                : base(parent)
        {
            _font = Service.Get<IResourceProvider>().GetAsciiFont(1);
            GumpLocalID = ID;
            Position = new Vector2Int(x, y);
            Items = new List<string>(items);
            _width = width;
            Index = index;
            _visibleItems = itemsVisible;
            _canBeNull = canBeNull;
            IsFirstvisible = firstVisible;//hide creating control
            if (IsFirstvisible) //for fill action dropdownlist
                CreateVisual();
            HandlesMouseInput = true;

        }

        public void CreateVisual()
        {
            if (_resizePic != null || _label != null || ScrollButton != null)
                return;
            _resizePic = (ResizePic)AddControl(new ResizePic(this, 0, 0, 3000, _width, _font.Height + 8), 0);
            _resizePic.GumpLocalID = GumpLocalID;
            _resizePic.MouseClickEvent += onClickClosedList;
            _resizePic.MouseOverEvent += onMouseOverClosedList;
            _resizePic.MouseOutEvent += onMouseOutClosedList;
            _resizePic.IsEnabled = false;
            _label = (TextLabelAscii)AddControl(new TextLabelAscii(this, 4, 5, 1, hue_Text, string.Empty), 0);
            _label.GumpLocalID = GumpLocalID;
            ScrollButton = (GumpPic)AddControl(new GumpPic(this, _width - 22, 5, 2086, 0), 0);
            IsFirstvisible = true;//for invisible create control
        }

        public override void Dispose()
        {
            if (_resizePic != null)
            {
                _resizePic.MouseClickEvent -= onClickClosedList;
                _resizePic.MouseOverEvent -= onMouseOverClosedList;
                _resizePic.MouseOutEvent -= onMouseOutClosedList;
            }
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (Index < 0 || Index >= Items.Count)
                Index = -1;
            if (_IsListOpen)
            {
                // if we have moused off the open list, close it. We check to see if the mouse is over:
                // the resizepic for the closed list (because it takes one update cycle to open the list)
                // the resizepic for the open list, and the scroll bar if it is loaded.
                if (UserInterface.MouseOverControl != _openResizePic &&
                    UserInterface.MouseOverControl != _resizePic &&
                    (_openScrollBar != null && UserInterface.MouseOverControl != _openScrollBar))
                {
                    closeOpenList();
                }
                else
                {
                    // update the visible items
                    var itemOffset = _openScrollBar == null ? 0 : _openScrollBar.Value;
                    if (Items.Count != 0)
                        for (var i = 0; i < _visibleItems; i++)
                            _openLabels[i].Text = (i + itemOffset < 0) ? string.Empty : Items[i + itemOffset];
                }
            }
            else if (IsFirstvisible)//for create hide control
            {
                if (Index == -1)
                {
                    if (Items.Count > 0) _label.Text = Items[0];
                    else _label.Text = "";
                }
                else _label.Text = Items[Index];
            }
            base.Update(totalMS, frameMS);
        }

        private void closeOpenList()
        {
            _IsListOpen = false;
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
            for (var i = 0; i < _visibleItems; i++)
                _openLabels[i].Dispose();
        }

        private void onClickClosedList(AControl control, int x, int y, MouseButton button)
        {
            if (Items.Count > 0)
            {
                _IsListOpen = true;
                _openResizePic = new ResizePic(Parent, X, Y, 3000, _width, _font.Height * _visibleItems + 8);
                _openResizePic.GumpLocalID = GumpLocalID;
                _openResizePic.MouseClickEvent += onClickOpenList;
                _openResizePic.MouseOverEvent += onMouseOverOpenList;
                _openResizePic.MouseOutEvent += onMouseOutOpenList;
                ((Gump)Parent).AddControl(_openResizePic, Page);
                if (_visibleItems > Items.Count)
                    _visibleItems = Items.Count;
                // only show the scrollbar if we need to scroll
                if (_visibleItems < Items.Count)
                {
                    _openScrollBar = new ScrollBar(Parent, X + _width - 20, Y + 4, _font.Height * _visibleItems, (_canBeNull ? -1 : 0), Items.Count - _visibleItems, Index);
                    ((Gump)Parent).AddControl(_openScrollBar, Page);
                }
                _openLabels = new TextLabelAscii[_visibleItems];
                for (var i = 0; i < _visibleItems; i++)
                {
                    _openLabels[i] = new TextLabelAscii(Parent, X + 4, Y + 5 + _font.Height * i, 1, 1106, string.Empty);
                    ((Gump)Parent).AddControl(_openLabels[i], Page);
                }
            }
        }

        private void onMouseOverClosedList(AControl control, int x, int y)
        {
            _label.Hue = hue_TextSelected;
        }

        private void onMouseOutClosedList(AControl control, int x, int y)
        {
            _label.Hue = hue_Text;
        }

        public void setIndex(int macroType, int valueID)
        {
            ScrollButton.IsVisible = true;
            IsVisible = true;
            Items.Clear();
            Index = -1;
            switch ((MacroType)macroType)
            {
                case MacroType.UseSkill:
                    foreach (MacroDefinition def in Macros.Skills)
                        Items.Add(def.Name);
                    break;

                case MacroType.CastSpell:
                    foreach (MacroDefinition def in Macros.Spells)
                        Items.Add(def.Name);
                    break;

                case MacroType.OpenGump:
                case MacroType.CloseGump:
                    foreach (MacroDefinition def in Macros.Gumps)
                        Items.Add(def.Name);
                    break;

                case MacroType.Move:
                    foreach (MacroDefinition def in Macros.Moves)
                        Items.Add(def.Name);
                    break;

                case MacroType.ArmDisarm:
                    foreach (MacroDefinition def in Macros.ArmDisarms)
                        Items.Add(def.Name);
                    break;

                case MacroType.Say:
                case MacroType.Emote:
                case MacroType.Whisper:
                case MacroType.Yell:
                    ScrollButton.IsVisible = false;
                    break;

                default:
                    // no sub-ids for these types
                    ScrollButton.IsVisible = false;
                    IsVisible = false;
                    break;
            }
            Index = valueID;
        }

        private void onClickOpenList(AControl control, int x, int y, MouseButton button)
        {
            //for macro options
            var id = control.GumpLocalID;
            var controlValueIndex = -1;
            for (var i = 0; i < Parent.Children.Count; i++)
                if (Parent.Children[i].GumpLocalID == (id + 1000))
                {
                    controlValueIndex = i;
                    break;
                }
            var indexOver = getOpenListIndexFromPoint(x, y);
            if (indexOver != -1)
                Index = indexOver + (_openScrollBar == null ? 0 : _openScrollBar.Value);
            closeOpenList();
            //for macro options
            if (controlValueIndex != -1)
            {
                if (indexOver == -1)
                    return;
                var mType = Macros.Types[Index].Type;
                // background image:
                if (!(Parent.Children[controlValueIndex] as MacroDropDownList).IsFirstvisible)
                    (Parent.Children[controlValueIndex] as MacroDropDownList).CreateVisual();
                // clear and show dropdown/text entry:
                (Parent.Children[controlValueIndex] as MacroDropDownList).Items.Clear();
                (Parent.Children[controlValueIndex] as MacroDropDownList).ScrollButton.IsVisible = true;//easy way for visible dropdown list
                (Parent.Children[controlValueIndex] as MacroDropDownList).IsVisible = true;//easy way for visible dropdown list
                (Parent.Children[controlValueIndex + 1] as TextEntry).Text = string.Empty;
                switch (mType)
                {
                    case MacroType.UseSkill:
                        foreach (MacroDefinition def in Macros.Skills)
                            (Parent.Children[controlValueIndex] as MacroDropDownList).Items.Add(def.Name);
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//textentry disabled because i need dropdownlist
                        break;
                    case MacroType.CastSpell:
                        foreach (MacroDefinition def in Macros.Spells)
                            (Parent.Children[controlValueIndex] as MacroDropDownList).Items.Add(def.Name);
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//textentry disabled because i need dropdownlist
                        break;
                    case MacroType.OpenGump:
                    case MacroType.CloseGump:
                        foreach (MacroDefinition def in Macros.Gumps)
                            (Parent.Children[controlValueIndex] as MacroDropDownList).Items.Add(def.Name);
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//textentry disabled because i need dropdownlist
                        break;
                    case MacroType.Move:
                        foreach (MacroDefinition def in Macros.Moves)
                            (Parent.Children[controlValueIndex] as MacroDropDownList).Items.Add(def.Name);
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//textentry disabled because i need dropdownlist
                        break;
                    case MacroType.ArmDisarm:
                        foreach (MacroDefinition def in Macros.ArmDisarms)
                            (Parent.Children[controlValueIndex] as MacroDropDownList).Items.Add(def.Name);
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//textentry disabled because i need dropdownlist
                        break;
                    case MacroType.Say:
                    case MacroType.Emote:
                    case MacroType.Whisper:
                    case MacroType.Yell:
                        //(Parent.Children[controlValueIndex] as MacroDropDownList).m_scrollButton.IsVisible = false; //as you wish
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = true;//textentry activated
                        break;
                    case MacroType.None:
                        (Parent.Children[controlValueIndex] as MacroDropDownList).ScrollButton.IsVisible = false;//i dont need any control :)
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//i dont need any control :)
                        (Parent.Children[controlValueIndex] as MacroDropDownList).IsVisible = false;//i dont need any control :)
                        break;
                    default:
                        //unnecessary
                        break;
                }
            }
        }

        private void onMouseOverOpenList(AControl control, int x, int y)
        {
            var indexOver = getOpenListIndexFromPoint(x, y);
            for (var i = 0; i < _openLabels.Length; i++)
                if (i == indexOver) _openLabels[i].Hue = hue_TextSelected;
                else _openLabels[i].Hue = hue_Text;
        }

        private void onMouseOutOpenList(AControl control, int x, int y)
        {
            for (var i = 0; i < _openLabels.Length; i++)
                _openLabels[i].Hue = hue_Text;
        }

        private int getOpenListIndexFromPoint(int x, int y)
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