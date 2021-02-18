using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    internal sealed class HalfBarData : ILoadable
    {
        public string Part { get; private set; }
        public string Text { get; private set; }

        public Chord Chord { get; private set; }

        public void SetChord(string chordKey, Dictionary<string, Chord> chords)
        {
            _chordKey = chordKey;
            SetChord(chords);
        }

        public void SetChord(Dictionary<string, Chord> chords) { Chord = chords[_chordKey]; }

        public void Load(IList<object> values)
        {
            Part = values.ToString(0);
            _chordKey = values.ToString(1);
            Text = values.ToString(2);
        }

        private string _chordKey;
    }
}
