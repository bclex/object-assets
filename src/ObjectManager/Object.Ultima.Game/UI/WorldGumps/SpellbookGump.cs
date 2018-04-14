using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Data;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items.Containers;
using System.Collections.Generic;

namespace OA.Ultima.UI.WorldGumps
{
    class SpellbookGump : Gump
    {
        SpellBook _spellbook;
        HtmlGumpling[] _circleHeaders;
        HtmlGumpling[] _indexes;
        WorldModel _world;

        public SpellbookGump(SpellBook entity)
            : base(entity.Serial, 0)
        {
            _world = Service.Get<WorldModel>();

            _spellbook = entity;
            _spellbook.SetCallbacks(OnEntityUpdate, OnEntityDispose);

            IsMoveable = true;

            if (_spellbook.BookType != SpellBookTypes.Unknown)
                CreateMageryGumplings();
            else
                // display a default spellbook graphic, based on the default spellbook type for this item ID.
                // right now, I'm just using a magery background, but really the background should change based
                // on the item id.
                // other options? necro? spellweaving?
                AddControl(new GumpPic(this, 0, 0, 0x08AC, 0));
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Dispose()
        {
            _spellbook.ClearCallBacks(OnEntityUpdate, OnEntityDispose);
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

        // ============================================================================================================
        // OnEntityUpdate - called when spellbook entity is updated by server.
        // ============================================================================================================
        void OnEntityUpdate(AEntity entity)
        {
            if (_spellbook.BookType == SpellBookTypes.Magic)
                CreateMageryGumplings();
        }

        void OnEntityDispose(AEntity entity)
        {
            Dispose();
        }

        // ============================================================================================================
        // Child control creation
        // The spellbook is laid out as follows:
        // 1. A list of all spells in the book. Clicking on a spell will turn to that spell's page.
        // 2. One page per spell in the book. Icon, runes, reagents, etc.
        // ============================================================================================================
        GumpPic _pageCornerLeft;
        GumpPic _pageCornerRight;
        int _maxPage;
        readonly List<KeyValuePair<int, int>> _spellList = new List<KeyValuePair<int, int>>();

        void CreateMageryGumplings()
        {
            ClearControls();

            AddControl(new GumpPic(this, 0, 0, 0x08AC, 0)); // spellbook background

            AddControl(_pageCornerLeft = new GumpPic(this, 50, 8, 0x08BB, 0)); // page turn left
            _pageCornerLeft.GumpLocalID = 0;
            _pageCornerLeft.MouseClickEvent += PageCorner_MouseClickEvent;
            _pageCornerLeft.MouseDoubleClickEvent += PageCorner_MouseDoubleClickEvent;

            AddControl(_pageCornerRight = new GumpPic(this, 321, 8, 0x08BC, 0)); // page turn right
            _pageCornerRight.GumpLocalID = 1;
            _pageCornerRight.MouseClickEvent += PageCorner_MouseClickEvent;
            _pageCornerRight.MouseDoubleClickEvent += PageCorner_MouseDoubleClickEvent;

            for (var i = 0; i < 4; i++) // spell circles 1 - 4
            {
                AddControl(new GumpPic(this, 60 + i * 35, 174, 0x08B1 + i, 0));
                LastControl.GumpLocalID = i;
                LastControl.MouseClickEvent += SpellCircle_MouseClickEvent; // unsubscribe from these?
            }
            for (var i = 0; i < 4; i++) // spell circles 5 - 8
            {
                AddControl(new GumpPic(this, 226 + i * 34, 174, 0x08B5 + i, 0));
                LastControl.GumpLocalID = i + 4;
                LastControl.MouseClickEvent += SpellCircle_MouseClickEvent; // unsubscribe from these?
            }

            // indexes are on pages 1 - 4. Spells are on pages 5+.
            _circleHeaders = new HtmlGumpling[8];
            for (var i = 0; i < 8; i++)
                _circleHeaders[i] = (HtmlGumpling)AddControl(
                    new HtmlGumpling(this, 64 + (i % 2) * 148, 10, 130, 200, 0, 0,
                        string.Format("<span color='#004' style='font-family=uni0;'><center>{0}</center></span>", SpellsMagery.CircleNames[i])),
                        1 + (i / 2));
            _indexes = new HtmlGumpling[8];
            for (var i = 0; i < 8; i++)
                _indexes[i] = (HtmlGumpling)AddControl(
                    new HtmlGumpling(this, 64 + (i % 2) * 156, 28, 130, 200, 0, 0, string.Empty),
                    1 + (i / 2));

            _maxPage = 4;

            // Begin checking which spells are in the spellbook and add them to m_Spells list

            var totalSpells = 0;
            _spellList.Clear();
            for (var spellCircle = 0; spellCircle < 8; spellCircle++)
                for (var spellIndex = 1; spellIndex <= 8; spellIndex++)
                    if (_spellbook.HasSpell(spellCircle, spellIndex))
                    {
                        _spellList.Add(new KeyValuePair<int, int>(spellCircle, spellIndex));
                        totalSpells++;
                    }

            _maxPage = _maxPage + ((totalSpells + 1) / 2); // The number of additional spell info pages needed

            SetActivePage(1);
        }

        void CreateSpellPage(int page, bool rightPage, int circle, SpellDefinition spell)
        {
            // header: "NTH CIRCLE"
            AddControl(new HtmlGumpling(this, 64 + (rightPage ? 148 : 0), 10, 130, 200, 0, 0,
                string.Format("<span color='#004' style='font-family=uni0;'><center>{0}</center></span>", SpellsMagery.CircleNames[circle])),
                page);
            // icon and spell name
            AddControl(new HtmlGumpling(this, 56 + (rightPage ? 156 : 0), 38, 130, 44, 0, 0,
                string.Format("<a href='spellicon={0}'><gumpimg src='{1}'/></a>",
                spell.ID, spell.GumpIconID - 0x1298)),
                page);
            AddControl(new HtmlGumpling(this, 104 + (rightPage ? 156 : 0), 38, 88, 40, 0, 0, string.Format(
                "<a href='spell={0}' color='#542' hovercolor='#875' activecolor='#420' style='font-family=uni0; text-decoration=none;'>{1}</a>",
                spell.ID, spell.Name)),
                page);
            // reagents.
            AddControl(new HtmlGumpling(this, 56 + (rightPage ? 156 : 0), 84, 146, 106, 0, 0, string.Format(
                "<span color='#400' style='font-family=uni0;'>Reagents:</span><br/><span style='font-family=ascii6;'>{0}</span>", spell.CreateReagentListString(", "))),
                page);
        }

        public override void OnHtmlInputEvent(string href, MouseEvent e)
        {
            if (e == MouseEvent.DoubleClick)
            {
                var hrefs = href.Split('=');
                if (hrefs.Length != 2)
                    return;
                else if (hrefs[0] == "page")
                {
                    if (int.TryParse(hrefs[1], out int page))
                        _world.Interaction.CastSpell(page - 4);
                }
                else if (hrefs[0] == "spell")
                {
                    if (int.TryParse(hrefs[1], out int spell))
                        _world.Interaction.CastSpell(spell);
                }
                else if (hrefs[0] == "spellicon")
                {
                    if (int.TryParse(hrefs[1], out int spell))
                        _world.Interaction.CastSpell(spell);
                }
            }
            else if (e == MouseEvent.Click)
            {
                var hrefs = href.Split('=');
                if (hrefs.Length != 2)
                    return;
                if (hrefs[0] == "page")
                {
                    if (int.TryParse(hrefs[1], out int page))
                        SetActivePage(page);
                }
            }
            else if (e == MouseEvent.DragBegin)
            {
                var hrefs = href.Split('=');
                if (hrefs.Length != 2)
                    return;
                if (hrefs[0] == "spellicon")
                {
                    int spellIndex;
                    if (!int.TryParse(hrefs[1], out spellIndex))
                        return;
                    var spell = SpellsMagery.GetSpell(spellIndex);
                    if (spell.ID == spellIndex)
                    {
                        var input = Service.Get<IInputService>();
                        var gump = new UseSpellButtonGump(spell);
                        UserInterface.AddControl(gump, input.MousePosition.X - 22, input.MousePosition.Y - 22);
                        UserInterface.AttemptDragControl(gump, input.MousePosition, true);
                    }
                }
            }
        }

        void SpellCircle_MouseClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            SetActivePage(sender.GumpLocalID / 2 + 1);
        }

        void PageCorner_MouseClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            if (sender.GumpLocalID == 0) SetActivePage(ActivePage - 1);
            else SetActivePage(ActivePage + 1);
        }

        void PageCorner_MouseDoubleClickEvent(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            if (sender.GumpLocalID == 0) SetActivePage(1);
            else SetActivePage(_maxPage);
        }

        void SetActivePage(int page)
        {
            if (page < 1)
                page = 1;
            if (page > _maxPage)
                page = _maxPage;
            var currentPage = page;
            var currentSpellCircle = currentPage * 2 - 2; // chooses the right spell circle to print on index page
            var currentSpellInfoIndex = currentPage * 2 - 10; // keeps track of which spell info page to print
            for (var currentCol = 0; currentCol < 2; currentCol++)
            {
                var isRightPage = (currentCol + 1 == 2);
                currentSpellInfoIndex += currentCol;

                // Create Spell Index page
                if (currentPage <= 4)
                {
                    _indexes[currentSpellCircle].Text = "";
                    foreach (var spell in _spellList)
                        if (spell.Key == currentSpellCircle)
                        {
                            var currentSpellInfoPage = _spellList.IndexOf(spell) / 2;
                            _indexes[currentSpellCircle].Text += string.Format("<a href='page={1}' color='#532' hovercolor='#800' activecolor='#611' style='font-family=uni0; text-decoration=none;'>{0}</a><br/>",
                                SpellsMagery.GetSpell(currentSpellCircle * 8 + spell.Value).Name,
                                5 + currentSpellInfoPage);
                        }
                    currentSpellCircle++;
                }
                else
                {
                    // Create Spell Info Page
                    if (currentSpellInfoIndex < _spellList.Count)
                        CreateSpellPage(page, isRightPage, _spellList[currentSpellInfoIndex].Key, SpellsMagery.GetSpell(_spellList[currentSpellInfoIndex].Key * 8 + _spellList[currentSpellInfoIndex].Value));
                }
            }

            ActivePage = page;
            // hide the page corners if we're at the first or final page.
            _pageCornerLeft.Page = ActivePage != 1 ? 0 : int.MaxValue;
            _pageCornerRight.Page = ActivePage != _maxPage ? 0 : int.MaxValue;
        }
    }
}
