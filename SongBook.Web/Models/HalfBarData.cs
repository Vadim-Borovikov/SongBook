using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    internal sealed class HalfBarData : ILoadable
    {
        public string Part { get; private set; }
        public string ChordKey { get; private set; }
        public string Text { get; private set; }

        public Chord Chord { get; private set; }

        public void SetChord(Dictionary<string, Chord> chords) { Chord = chords[ChordKey]; }

        public void Load(IList<object> values)
        {
            Part = values.ToString(0);
            ChordKey = values.ToString(1);
            Text = values.ToString(2);
        }
    }
}
