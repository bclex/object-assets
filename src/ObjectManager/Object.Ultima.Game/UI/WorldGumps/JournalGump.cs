using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Core.UI;
using OA.Ultima.Player;
using OA.Ultima.UI.Controls;
using UnityEngine;

namespace OA.Ultima.UI.WorldGumps
{
    class JournalGump : Gump
    {
        ExpandableScroll _background;
        RenderedTextList _journalEntries;
        readonly IScrollBar _scrollBar;

        public JournalGump()
            : base(0, 0)
        {
            IsMoveable = true;

            AddControl(_background = new ExpandableScroll(this, 0, 0, 300));
            _background.TitleGumpID = 0x82A;

            _scrollBar = (IScrollBar)AddControl(new ScrollFlag(this));
            AddControl(_journalEntries = new RenderedTextList(this, 30, 36, 242, 200, _scrollBar));
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("journal");

            InitializeJournalEntries();
            PlayerState.Journaling.OnJournalEntryAdded += AddJournalEntry;
        }

        public override void Dispose()
        {
            PlayerState.Journaling.OnJournalEntryAdded -= AddJournalEntry;
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            _journalEntries.Height = Height - 98;
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
        }

        private void AddJournalEntry(JournalEntry entry)
        {
            string text = string.Format("{0}{1}", entry.SpeakerName != string.Empty ? entry.SpeakerName + ": " : string.Empty, entry.Text);
            int font = entry.Font;
            bool asUnicode = entry.AsUnicode;
            TransformFont(ref font, ref asUnicode);

            _journalEntries.AddEntry(string.Format(
                "<span color='#{3}' style='font-family:{1}{2};'>{0}</span>", text, asUnicode ? "uni" : "ascii", font,
                // "<span color='#{1}' style='font-family:ascii9;'>{0}</span>", text, 
                Utility.GetColorFromUshort(Resources.HueData.GetHue(entry.Hue, 0))));
        }

        private void TransformFont(ref int font, ref bool asUnicode)
        {
            if (asUnicode)
                return;
            else
            {
                switch (font)
                {
                    case 3:
                        {
                            font = 1;
                            asUnicode = true;
                            break;
                        }
                }
            }
        }

        private void InitializeJournalEntries()
        {
            for (var i = 0; i < PlayerState.Journaling.JournalEntries.Count; i++)
                AddJournalEntry(PlayerState.Journaling.JournalEntries[i]);
            _scrollBar.MinValue = 0;
        }
    }
}
