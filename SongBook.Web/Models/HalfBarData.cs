using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    public sealed class HalfBarData : ILoadable
    {
        internal string Part { get; private set; }
        internal string Text { get; private set; }

        public Chord Chord { get; private set; }

        public void Load(IList<object> values)
        {
            Part = values.ToString(0);
            _chordKey = values.ToString(1);
            Text = values.ToString(2);
        }

        internal void SetChord(string chordKey, Dictionary<string, Chord> chords)
        {
            _chordKey = chordKey;
            SetChord(chords);
        }

        internal void SetChord(Dictionary<string, Chord> chords) { Chord = chords[_chordKey]; }

        private string _chordKey;
    }
}
