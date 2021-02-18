using System;
using System.Collections.Generic;
using System.Linq;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    public sealed class Song : SongData
    {
        public byte CurrentTune { get; private set; }
        public byte GetCurrentCapo() => Invert(CurrentTune);

        internal readonly IReadOnlyList<Part> Parts;

        internal Song(string name, string author, byte defaultCapo, Provider provider, string sheetPostfix,
            Dictionary<string, Chord> chords)
        {
            Name = name;
            Author = author;

            DefaultCapo = defaultCapo;
            CurrentTune = 0;

            _chords = chords;

            IList<HalfBarData> halfBars = DataManager.GetValues<HalfBarData>(provider, $"{Name}{sheetPostfix}");
            var parts = new List<Part>();
            Part currentPart = null;
            foreach (HalfBarData halfBarData in halfBars)
            {
                halfBarData.InitChord(_chords);
                if (string.IsNullOrWhiteSpace(halfBarData.Part))
                {
                    if (currentPart == null)
                    {
                        throw new NullReferenceException("Empty part!");
                    }
                    currentPart.HalfBars.Add(halfBarData);
                }
                else
                {
                    if (currentPart != null)
                    {
                        parts.Add(currentPart);
                    }
                    currentPart = new Part(halfBarData);
                }
            }
            if (currentPart != null)
            {
                parts.Add(currentPart);
            }
            Parts = parts;
        }

        internal void Reset() => TransposeTo((sbyte)Invert(DefaultCapo));

        internal void TransposeTo(sbyte semitones) => Transpose((sbyte)(semitones - CurrentTune));

        private void Transpose(sbyte semitones)
        {
            if (semitones == 0)
            {
                return;
            }

            CurrentTune = (byte)((Chord.Semitones.Length + CurrentTune + semitones) % Chord.Semitones.Length);
            foreach (HalfBarData halfBarData in Parts.SelectMany(p => p.HalfBars))
            {
                string chordKey = halfBarData.Chord.Transpose(semitones);
                halfBarData.SetChord(chordKey, _chords);
            }
        }

        private static byte Invert(byte tune) => (byte)((Chord.Semitones.Length - tune) % Chord.Semitones.Length);

        private readonly Dictionary<string, Chord> _chords;
    }
}
