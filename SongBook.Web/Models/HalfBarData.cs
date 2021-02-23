using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    public sealed class HalfBarData : ILoadable
    {
        internal string Part { get; private set; }
        internal string Text { get; private set; }

        public Chord Chord { get; private set; }
        public int ChordOption { get; private set; }

        public void Load(IList<object> values)
        {
            Part = values.ToString(0);
            _chordKey = values.ToString(1);
            _initialChordOption = (values.ToInt(2) ?? 1) - 1;
            Text = values.ToString(3);
        }

        internal void SetChord(string chordKey, Dictionary<string, Chord> chords)
        {
            _chordKey = chordKey;
            InitChord(chords);
        }

        internal void InitChord(Dictionary<string, Chord> chords)
        {
            Chord = chords[_chordKey];
            ChordOption = string.IsNullOrWhiteSpace(Chord.Fingerings[_initialChordOption]) ? 0 : _initialChordOption;
        }

        private string _chordKey;
        private int _initialChordOption;
    }
}
