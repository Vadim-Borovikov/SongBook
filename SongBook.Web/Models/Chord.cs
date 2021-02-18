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
        private readonly byte _bass;

        private Chord(byte semitone, string postfix, byte bass, string fingering, bool isSimple)
        {
            _semitone = semitone;
            _postfix = postfix;
            _bass = bass;
            Fingering = fingering;
            IsSimple = isSimple;
        }

        internal Chord(ChordData data)
            : this((byte)Semitones.IndexOf(data.Semitone), data.Postfix, (byte)Semitones.IndexOf(data.Bass),
                data.Fingering, data.IsSimple)
        {
        }

        internal string Transpose(sbyte semitones)
        {
            byte semitone = (byte)((Semitones.Length + _semitone + semitones) % Semitones.Length);
            byte bass = (byte)((Semitones.Length + _bass + semitones) % Semitones.Length);
            var transposed = new Chord(semitone, _postfix, bass, Fingering, IsSimple);
            return transposed.ToString();
        }

        public override string ToString()
        {
            string semitone = Semitones[_semitone];
            string bassSemitone = Semitones[_bass];
            string result = $"{semitone}{_postfix}";
            if (bassSemitone != semitone)
            {
                result += $"/{bassSemitone}";
            }
            return result;
        }
    }
}