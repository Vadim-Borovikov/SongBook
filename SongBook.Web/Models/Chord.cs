using Microsoft.EntityFrameworkCore.Internal;

namespace SongBook.Web.Models
{
    public sealed class Chord
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

        public readonly string Fingering;
        public readonly bool IsSimple;

        private readonly byte _semitone;
        private readonly string _postfix;

        private Chord(byte semitone, string postfix, string fingering, bool isSimple)
        {
            _semitone = semitone;
            _postfix = postfix;
            Fingering = fingering;
            IsSimple = isSimple;
        }

        internal Chord(ChordData data)
            : this((byte)Semitones.IndexOf(data.Semitone), data.Postfix, data.Fingering, data.IsSimple)
        {
        }

        internal string Transpose(sbyte semitones)
        {
            byte semitone = (byte)((Semitones.Length + _semitone + semitones) % Semitones.Length);
            return $"{Semitones[semitone]}{_postfix}";
        }

        public override string ToString() => $"{Semitones[_semitone]}{_postfix}";
    }
}