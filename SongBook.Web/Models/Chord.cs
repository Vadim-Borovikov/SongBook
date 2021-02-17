using Microsoft.EntityFrameworkCore.Internal;

namespace SongBook.Web.Models
{
    public sealed class Chord
    {
        private static readonly string[] Notes =
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

        private readonly byte _note;
        private readonly string _postfix;
        private readonly bool _isSimple;

        private Chord(byte note, string postfix, string fingering, bool isSimple)
        {
            _note = note;
            _postfix = postfix;
            Fingering = fingering;
            _isSimple = isSimple;
        }

        internal Chord(ChordData data)
            : this((byte)Notes.IndexOf(data.Note), data.Postfix, data.Fingering, data.IsSimple)
        {
        }

        public Chord Transpose(int tones)
        {
            byte note = (byte)((_note + tones) % Notes.Length);
            return new Chord(note, _postfix, Fingering, _isSimple);
        }

        public override string ToString() => $"{Notes[_note]}{_postfix}";
    }
}