using System;
using System.Collections.Generic;

namespace OA.Ultima.Player
{
    public class JournalData
    {
        readonly List<JournalEntry> _journalEntries = new List<JournalEntry>();
        public List<JournalEntry> JournalEntries
        {
            get { return _journalEntries; }
        }

        public event Action<JournalEntry> OnJournalEntryAdded;

        public void AddEntry(string text, int font, ushort hue, string speakerName, bool asUnicode)
        {
            while (_journalEntries.Count > 99)
                _journalEntries.RemoveAt(0);
            _journalEntries.Add(new JournalEntry(text, font, hue, speakerName, asUnicode));
            OnJournalEntryAdded?.Invoke(_journalEntries[_journalEntries.Count - 1]);
        }
    }

    public class JournalEntry
    {
        public readonly string Text;
        public readonly int Font;
        public readonly ushort Hue;
        public readonly string SpeakerName;
        public readonly bool AsUnicode;

        public JournalEntry(string text, int font, ushort hue, string speakerName, bool asUnicode)
        {
            Text = text;
            Font = font;
            Hue = hue;
            SpeakerName = speakerName;
            AsUnicode = asUnicode;
        }
    }
}
