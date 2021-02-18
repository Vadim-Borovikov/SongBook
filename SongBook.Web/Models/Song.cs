using System;
using System.Collections.Generic;
using System.Linq;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    public sealed class Song : SongData
    {
        public byte CurrentTune { get; private set; }

        internal readonly IReadOnlyList<Part> Parts;

        internal Song(string name, string author, byte defaultTune, Provider provider, string sheetPostfix,
            Dictionary<string, Chord> chords)
        {
            Name = name;
            Author = author;

            DefaultTune = defaultTune;
            CurrentTune = DefaultTune;

            _chords = chords;

            IList<HalfBarData> halfBars = DataManager.GetValues<HalfBarData>(provider, $"{Name}{sheetPostfix}");
            var parts = new List<Part>();
            Part currentPart = null;
            foreach (HalfBarData halfBar in halfBars)
            {
                halfBar.SetChord(_chords);
                if (string.IsNullOrWhiteSpace(halfBar.Part))
                {
                    if (currentPart == null)
                    {
                        throw new NullReferenceException("Empty part!");
                    }
                    currentPart.HalfBars.Add(halfBar);
                }
                else
                {
                    if (currentPart != null)
                    {
                        parts.Add(currentPart);
                    }
                    currentPart = new Part(halfBar);
                }
            }
            if (currentPart != null)
            {
                parts.Add(currentPart);
            }
            Parts = parts;
        }

        internal void Reset() => TransposeTo(DefaultTune);

        internal void TransposeTo(byte semitones) => Transpose((sbyte)(semitones - CurrentTune));

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

        private readonly Dictionary<string, Chord> _chords;
    }
}
