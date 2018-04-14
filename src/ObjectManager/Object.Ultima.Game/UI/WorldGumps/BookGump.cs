using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items;
using System.Collections.Generic;

namespace OA.Ultima.UI.WorldGumps
{
    public class BookGump : Gump
    {
        BaseBook _book;
        GumpPic _bookBackground;
        GumpPic _pageCornerLeft;
        GumpPic _pageCornerRight;
        List<TextEntryPage> _pages = new List<TextEntryPage>();
        TextEntry _titleTextEntry;
        TextEntry _authorTextEntry;
        int _lastPage;
        WorldModel _world;

        // ================================================================================
        // Ctor, Dispose, BuildGump, and SetActivePage
        // ================================================================================
        public BookGump(BaseBook entity)
            : base(entity.Serial, 0)
        {
            _book = entity;
            _book.SetCallbacks(OnEntityUpdate, OnEntityDispose);
            _lastPage = (_book.PageCount + 2) / 2;
            IsMoveable = true;
            _world = Service.Get<WorldModel>(false);
            BuildGump();
        }

        public override void Dispose()
        {
            _book.ClearCallBacks(OnEntityUpdate, OnEntityDispose);
            if (_pageCornerLeft != null)
            {
                _pageCornerLeft.MouseClickEvent -= PageCorner_MouseClickEvent;
                _pageCornerLeft.MouseDoubleClickEvent -= PageCorner_MouseDoubleClickEvent;
            }
            if (_pageCornerRight != null)
            {
                _pageCornerRight.MouseClickEvent -= PageCorner_MouseClickEvent;
                _pageCornerRight.MouseDoubleClickEvent -= PageCorner_MouseDoubleClickEvent;
            }
            base.Dispose();
        }

        void BuildGump()
        {
            ClearControls();
            if (_book.ItemID >= 0xFEF && _book.ItemID <= 0xFF2)
            {
                _bookBackground = new GumpPic(this, 0, 0, 0x1FE, 0);
                _pageCornerLeft = new GumpPic(this, 0, 0, 0x1FF, 0);
                _pageCornerRight = new GumpPic(this, 356, 0, 0x200, 0);
            }
            AddControl(_bookBackground);   // book background gump
            AddControl(_pageCornerLeft);   // page turn left
            _pageCornerLeft.GumpLocalID = 0;
            _pageCornerLeft.MouseClickEvent += PageCorner_MouseClickEvent;
            _pageCornerLeft.MouseDoubleClickEvent += PageCorner_MouseDoubleClickEvent;
            AddControl(_pageCornerRight);  // page turn right
            _pageCornerRight.GumpLocalID = 1;
            _pageCornerRight.MouseClickEvent += PageCorner_MouseClickEvent;
            _pageCornerRight.MouseDoubleClickEvent += PageCorner_MouseDoubleClickEvent;
            // Draw the title and author page
            _titleTextEntry = new TextEntry(this, 45, 50, 155, 300, 1, 0, 0, _book.Title);
            _titleTextEntry.MakeThisADragger();
            _titleTextEntry.IsEditable = _book.IsEditable;
            _authorTextEntry = new TextEntry(this, 45, 110, 160, 300, 1, 0, 0, _book.Author);
            _authorTextEntry.MakeThisADragger();
            _authorTextEntry.IsEditable = _book.IsEditable;
            AddControl(_titleTextEntry, 1);
            AddControl(new HtmlGumpling(this, 45, 90, 155, 300, 0, 0, "<font color=#444>By"), 1);
            AddControl(_authorTextEntry, 1);
            // Add book pages to active pages
            var isRight = true;
            var color = _book.IsEditable ? "800" : "000";
            for (var i = 0; i < _book.PageCount; i++)
            {
                var onGumpPage = (i + 3) / 2;
                var x = isRight ? 235 : 45;
                _pages.Add(new TextEntryPage(this, x, 32, 155, 300, i));
                _pages[i].SetMaxLines(8, OnPageOverflow, OnPageUnderflow);
                _pages[i].SetKeyboardPageControls(OnPreviousPage, OnNextPage);
                _pages[i].MakeThisADragger();
                _pages[i].IsEditable = _book.IsEditable;
                _pages[i].LeadingHtmlTag = $"<font color=#{color}>";
                _pages[i].Text = _book.Pages[i].GetAllLines();
                AddControl(_pages[i], onGumpPage);
                AddControl(new HtmlGumpling(this, x, 195, 135, 20, 0, 0, $"<center><font color=#444>{i + 1}"), onGumpPage);
                isRight = !isRight;
            }
            var service = Service.Get<AudioService>();
            service.PlaySound(0x058);
            SetActivePage(1);
            UserInterface.KeyboardFocusControl = _pages[0];
            _pages[0].CaratAt = _pages[0].Text.Length;
        }

        void SetActivePage(int page)
        {
            if (page == ActivePage)
                return;
            CheckForContentChanges();
            if (page < 1)
                page = 1;
            if (page > _lastPage)
                page = _lastPage;
            ActivePage = page;
            // Hide the page corners if we're at the first or final page.
            _pageCornerLeft.Page = (page != 1) ? 0 : int.MaxValue;
            _pageCornerRight.Page = (page != _lastPage) ? 0 : int.MaxValue;
            var textEntryPageIndex = (page - 1) * 2 - 1;
            if (textEntryPageIndex == -1)
                textEntryPageIndex = 0;
            if (_pages[textEntryPageIndex] != null)
            {
                UserInterface.KeyboardFocusControl = _pages[textEntryPageIndex];
                _pages[textEntryPageIndex].CaratAt = _pages[textEntryPageIndex].Text.Length;
            }
        }

        // ================================================================================
        // OnEntityUpdate - called when book entity is updated by server.
        // OnEntityDispose - called when book entity is disposed by server.
        // ================================================================================
        void OnEntityUpdate(AEntity entity)
        {
            _book = entity as BaseBook;
            BuildGump();
        }

        void OnEntityDispose(AEntity entity)
        {
            Dispose();
        }

        // ================================================================================
        // Mouse Control
        // ================================================================================
        void PageCorner_MouseClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            if (sender.GumpLocalID == 0) SetActivePage(ActivePage - 1);
            else SetActivePage(ActivePage + 1);
            var service = Service.Get<AudioService>();
            service.PlaySound(0x055);
        }

        void PageCorner_MouseDoubleClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            if (sender.GumpLocalID == 0) SetActivePage(1);
            else SetActivePage(_lastPage);
        }

        protected override void CloseWithRightMouseButton()
        {
            CheckForContentChanges();
            var service = Service.Get<AudioService>();
            service.PlaySound(0x058);
            base.CloseWithRightMouseButton();
        }

        // ================================================================================
        // Keyboard/Text Control
        // ================================================================================
        void OnNextPage(int pageIndex)
        {
            if (pageIndex < _pages.Count - 1)
            {
                var nextPage = pageIndex + 1;
                SetActivePage((nextPage + 1) / 2 + 1);
                UserInterface.KeyboardFocusControl = _pages[nextPage];
                _pages[nextPage].CaratAt = 0;
            }
        }

        void OnPreviousPage(int pageIndex)
        {
            if (pageIndex > 0)
            {
                var prevPage = pageIndex - 1;
                SetActivePage((prevPage + 1) / 2 + 1);
                UserInterface.KeyboardFocusControl = _pages[prevPage];
                _pages[prevPage].CaratAt = _pages[prevPage].Text.Length;
            }
        }

        /// <summary>
        /// Called when the user hits backspace at index 0 on a page. 
        /// </summary>
        void OnPageUnderflow(int pageIndex)
        {
            if (pageIndex <= 0)
                return;
            var underflowFrom = pageIndex;
            var underflowTo = pageIndex - 1;
            var underflowFromText = _pages[underflowFrom].Text;
            var underflowToText = _pages[underflowTo].Text.Substring(0, (_pages[underflowTo].Text.Length > 0 ? _pages[underflowTo].Text.Length - 1 : 0));
            var carat = underflowToText.Length - _pages[underflowFrom].CaratAt;
            _pages[underflowFrom].Text = string.Empty;
            _pages[underflowTo].Text = $"{underflowToText}{underflowFromText}";
            if (carat <= _pages[underflowTo].Text.Length)
            {
                SetActivePage((underflowTo + 1) / 2 + 1);
                UserInterface.KeyboardFocusControl = _pages[underflowTo];
                _pages[underflowTo].CaratAt = carat;
            }
            else
            {
                SetActivePage((underflowFrom + 1) / 2 + 1);
                UserInterface.KeyboardFocusControl = _pages[underflowFrom];
                _pages[underflowFrom].CaratAt = carat - _pages[underflowTo].Text.Length;
            }
        }

        /// <summary>
        /// Called when text on a page is too large to be held in the page. The text overflows to the next page.
        /// </summary>
        void OnPageOverflow(int page, string overflow)
        {
            var overflowFrom = page;
            var overflowTo = page + 1;
            if (overflowTo < _pages.Count)
            {
                _pages[overflowTo].Text = _pages[overflowTo].Text.Insert(0, overflow);
                SetActivePage((overflowTo + 1) / 2 + 1);
                UserInterface.KeyboardFocusControl = _pages[overflowTo];
                _pages[overflowTo].CaratAt = overflow.Length;
            }
        }

        void CheckForContentChanges()
        {
            if (ActivePage < 1)
                return;
            var leftIndex = ActivePage * 2 - 3;
            var rightIndex = leftIndex + 1;
            // Check title, author, and the first page if leftPageIndex < 0
            // Else if leftPageIndex >= 0, they are all pages
            if (leftIndex < 0)
            {
                if (_titleTextEntry.Text != _book.Title || _authorTextEntry.Text != _book.Author)
                    _world?.Interaction.BookHeaderNewChange(_book.Serial, _titleTextEntry.Text, _authorTextEntry.Text);
                if (rightIndex < _pages.Count && _pages[rightIndex].TextWithLineBreaks != _book.Pages[rightIndex].GetAllLines())
                    _world?.Interaction.BookPageChange(_book.Serial, rightIndex, GetTextEntryAsArray(_pages[rightIndex]));
            }
            else
            {
                if (_pages[leftIndex].TextWithLineBreaks != _book.Pages[leftIndex].GetAllLines())
                    _world?.Interaction.BookPageChange(_book.Serial, leftIndex, GetTextEntryAsArray(_pages[leftIndex]));
                if (rightIndex < _pages.Count - 1 && _pages[rightIndex].TextWithLineBreaks != _book.Pages[rightIndex].GetAllLines())
                    _world?.Interaction.BookPageChange(_book.Serial, rightIndex, GetTextEntryAsArray(_pages[rightIndex]));
            }
        }

        string[] GetTextEntryAsArray(TextEntryPage text) => text.TextWithLineBreaks.Split('\n');
    }
}
