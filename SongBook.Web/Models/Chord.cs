using System.Collections.Generic;
using GoogleSheetsManager;
using Microsoft.EntityFrameworkCore.Internal;

namespace SongBook.Web.Models
{
    public sealed class Chord : ILoadable
    {
        internal static readonly string[] Semitones =
        {
            "A",
            "A#",
            "B",
            "C",
            "C#",
            "D",
            "D#",
            "E",
            "F",
            "F#",
            "G",
            "G#"
        };

        public string Fingering;
        public bool IsSimple;

        public Chord() { }

        public void Load(IList<object> values)
        {
            _semitone = values.ToString(0);
            _postfix = values.ToString(1);
            _bass = values.ToString(2);
            Fingering = values.ToString(3);
            IsSimple = values.ToBool(4) ?? false;
        }

        public override string ToString()
        {
            return _bass == _semitone ? $"{_semitone}{_postfix}" : $"{_semitone}{_postfix}/{_bass}";
        }

        internal string Transpose(sbyte semitones)
        {
            string semitone = Transpose(_semitone, semitones);
            string bass = Transpose(_bass, semitones);
            var transposed = new Chord(semitone, _postfix, bass, Fingering, IsSimple);
            return transposed.ToString();
        }

        private Chord(string semitone, string postfix, string bass, string fingering, bool isSimple)
        {
            _semitone = semitone;
            _postfix = postfix;
            _bass = bass;
            Fingering = fingering;
            IsSimple = isSimple;
        }

        private static string Transpose(string semitone, sbyte semitones)
        {
            int index = (Semitones.Length + Semitones.IndexOf(semitone) + semitones) % Semitones.Length;
            return Semitones[index];
        }

        private string _semitone;
        private string _postfix;
        private string _bass;
    }
}