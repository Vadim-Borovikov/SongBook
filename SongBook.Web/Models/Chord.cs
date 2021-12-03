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

        internal List<Fingering> Fingerings;
        internal bool IsSimple;

        public Chord() { }

        public void Load(IDictionary<string, object> valueSet)
        {
            _semitone = valueSet[SemitoneTitle]?.ToString();
            _postfix = valueSet[PostfixTitle]?.ToString();
            _bass = valueSet[BassTitle]?.ToString();
            IsSimple = valueSet[IsSimpleTitle]?.ToBool() ?? false;
            Fingerings = new List<Fingering>
            {
                new Fingering(valueSet[Fingerings1Title]?.ToString()),
                new Fingering(valueSet[Fingerings2Title]?.ToString()),
                new Fingering(valueSet[Fingerings3Title]?.ToString())
            };
        }

        public override string ToString()
        {
            return _bass == _semitone ? $"{_semitone}{_postfix}" : $"{_semitone}{_postfix}/{_bass}";
        }

        internal string TransposeBy(sbyte delta)
        {
            string semitone = TransposeBy(_semitone, delta);
            string bass = TransposeBy(_bass, delta);
            var transposed = new Chord(semitone, _postfix, bass, Fingerings, IsSimple);
            return transposed.ToString();
        }

        private Chord(string semitone, string postfix, string bass, List<Fingering> fingerings, bool isSimple)
        {
            _semitone = semitone;
            _postfix = postfix;
            _bass = bass;
            Fingerings = fingerings;
            IsSimple = isSimple;
        }

        private static string TransposeBy(string semitone, sbyte delta)
        {
            if (!Semitones.Contains(semitone))
            {
                return semitone;
            }
            var tune = new Tune((byte) Semitones.IndexOf(semitone));
            tune += delta;
            return Semitones[tune.Value];
        }

        private const string SemitoneTitle = "Полутон";
        private const string PostfixTitle = "Вариация";
        private const string BassTitle = "Бас";
        private const string IsSimpleTitle = "Простой";
        private const string Fingerings1Title = "Аппликатура 1";
        private const string Fingerings2Title = "Аппликатура 2";
        private const string Fingerings3Title = "Аппликатура 3";

        private string _semitone;
        private string _postfix;
        private string _bass;
    }
}