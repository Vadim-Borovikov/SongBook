using System.Collections.Generic;
using System.Linq;
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

        internal List<string> Fingerings;
        internal bool IsSimple;

        public Chord() { }

        public void Load(IList<object> values)
        {
            _semitone = values.ToString(0);
            _postfix = values.ToString(1);
            _bass = values.ToString(2);
            IsSimple = values.ToBool(3) ?? false;
            Fingerings = new List<string>
            {
                values.ToString(4),
                values.ToString(5),
                values.ToString(6)
            };
        }

        public override string ToString()
        {
            return _bass == _semitone ? $"{_semitone}{_postfix}" : $"{_semitone}{_postfix}/{_bass}";
        }

        internal string Transpose(sbyte semitones)
        {
            string semitone = Transpose(_semitone, semitones);
            string bass = Transpose(_bass, semitones);
            var transposed = new Chord(semitone, _postfix, bass, Fingerings, IsSimple);
            return transposed.ToString();
        }

        private Chord(string semitone, string postfix, string bass, List<string> fingerings, bool isSimple)
        {
            _semitone = semitone;
            _postfix = postfix;
            _bass = bass;
            Fingerings = fingerings;
            IsSimple = isSimple;
        }

        private static string Transpose(string semitone, sbyte semitones)
        {
            if (!Semitones.Contains(semitone))
            {
                return semitone;
            }
            int index = (Semitones.Length + Semitones.IndexOf(semitone) + semitones) % Semitones.Length;
            return Semitones[index];
        }

        private string _semitone;
        private string _postfix;
        private string _bass;
    }
}