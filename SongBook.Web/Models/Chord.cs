using Microsoft.EntityFrameworkCore.Internal;

namespace SongBook.Web.Models
{
    public sealed class Chord
    {
        private static readonly string[] Semitones =
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

        private readonly byte _semitone;
        private readonly string _postfix;
        private readonly bool _isSimple;

        private Chord(byte semitone, string postfix, string fingering, bool isSimple)
        {
            _semitone = semitone;
            _postfix = postfix;
            Fingering = fingering;
            _isSimple = isSimple;
        }

        internal Chord(ChordData data)
            : this((byte)Semitones.IndexOf(data.Semitone), data.Postfix, data.Fingering, data.IsSimple)
        {
        }

        public Chord Transpose(int semitones)
        {
            byte semitone = (byte)((Semitones.Length + _semitone + semitones) % Semitones.Length);
            return new Chord(semitone, _postfix, Fingering, _isSimple);
        }

        public override string ToString() => $"{Semitones[_semitone]}{_postfix}";
    }
}