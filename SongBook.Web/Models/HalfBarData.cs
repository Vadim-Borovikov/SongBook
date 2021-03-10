using System;
using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    public sealed class HalfBarData : ILoadable
    {
        internal string Part { get; private set; }
        internal string Rythm { get; private set; }
        internal string Text { get; private set; }

        internal Uri Tab { get; private set; }

        public Chord Chord { get; private set; }
        public int ChordOption { get; private set; }

        public void Load(IList<object> values)
        {
            Part = values.ToString(0);

            _music = values.ToString(1);

            _initialChordOption = (values.ToInt(2) ?? 1) - 1;

            Rythm = values.ToString(3);

            Text = values.ToString(4);
        }

        internal void SetChord(string chordKey, Dictionary<string, Chord> chords)
        {
            _music = chordKey;
            InitMusic(chords);
        }

        internal void InitMusic(Dictionary<string, Chord> chords)
        {
            if (_music == null)
            {
                return;
            }

            if (chords.ContainsKey(_music))
            {
                Chord = chords[_music];
                ChordOption = string.IsNullOrWhiteSpace(Chord.Fingerings[_initialChordOption])
                    ? 0
                    : _initialChordOption;
            }
            else
            {
                string tabUrl =
                    _music.Substring(TabPrefix.Length, _music.Length - TabPrefix.Length - TabPostfix.Length);
                Tab = new Uri(tabUrl);
            }
        }

        private const string TabPrefix = "=IMAGE(\"";
        private const string TabPostfix = "\")";

        private string _music;
        private int _initialChordOption;
    }
}
