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

        internal bool HasBarre() => (Chord != null) && Chord.Fingerings[ChordOption].HasBarre();

        public void Load(IDictionary<string, object> valueSet)
        {
            Part = valueSet[PartTitle]?.ToString();

            _music = valueSet[MusicTitle]?.ToString();

            _initialChordOption = (valueSet[InitialChordOptionTitle]?.ToInt() ?? 1) - 1;

            Rythm = valueSet[RythmTitle]?.ToString();

            Text = valueSet.ContainsKey(TextTitle) ? valueSet[TextTitle]?.ToString() : null;
        }

        internal void SetChord(string chordKey, Dictionary<string, Chord> chords)
        {
            _music = chordKey;
            InitMusic(chords);
        }

        internal void InitMusic(Dictionary<string, Chord> chords)
        {
            if (string.IsNullOrWhiteSpace(_music))
            {
                return;
            }

            if (chords.ContainsKey(_music))
            {
                Chord = chords[_music];
                ChordOption = Chord.Fingerings[_initialChordOption].IsPresent ? 0 : _initialChordOption;
            }
            else
            {
                string tabUrl =
                    _music.Substring(TabPrefix.Length, _music.Length - TabPrefix.Length - TabPostfix.Length);
                Tab = new Uri(tabUrl);
            }
        }

        private const string PartTitle = "Часть";
        private const string MusicTitle = "Аккорд";
        private const string InitialChordOptionTitle = "Вариант";
        private const string RythmTitle = "Ритм";
        private const string TextTitle = "Текст";

        private const string TabPrefix = "=IMAGE(\"";
        private const string TabPostfix = "\")";

        private string _music;
        private int _initialChordOption;
    }
}
