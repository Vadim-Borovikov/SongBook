using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Sheets.v4;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    public sealed class Song : ILoadable
    {
        public bool Ready { get; private set; }
        public bool Learned { get; private set; }
        public string Name { get; private set; }
        public string Author { get; private set; }
        public Uri Music { get; private set; }
        public List<Uri> Tutorials { get; private set; }

        public byte CurrentTune { get; private set; }
        public byte GetCurrentCapo() => Invert(CurrentTune);

        internal IReadOnlyList<Part> Parts;

        public void Load(IList<object> values)
        {
            Ready = values.ToBool(0) ?? false;
            Learned = values.ToBool(1) ?? false;
            Name = values.ToString(2);
            Author = values.ToString(3);
            _defaultCapo = (byte)(values.ToInt(4) ?? 0);
            Music = values.ToUri(5);
            Tutorials = values.ToUris(6);
        }

        internal void Load(Provider provider, string sheetPostfix, Dictionary<string, Chord> chords)
        {
            _chords = chords;

            IList<HalfBarData> halfBars = DataManager.GetValues<HalfBarData>(provider, $"{Name}{sheetPostfix}",
                SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.FORMULA);
            var parts = new List<Part>();
            Part currentPart = null;
            foreach (HalfBarData halfBarData in halfBars)
            {
                halfBarData.InitMusic(_chords);
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

        internal byte GetDefaultTune() => Invert(_defaultCapo);

        internal void TransposeTo(sbyte semitones) => Transpose((sbyte)(semitones - CurrentTune));

        private void Transpose(sbyte semitones)
        {
            if (semitones == 0)
            {
                return;
            }

            CurrentTune = (byte)((Chord.Semitones.Length + CurrentTune + semitones) % Chord.Semitones.Length);
            foreach (HalfBarData halfBarData in Parts.SelectMany(p => p.HalfBars).Where(h => h.Chord != null))
            {
                string chordKey = halfBarData.Chord.Transpose(semitones);
                halfBarData.SetChord(chordKey, _chords);
            }
        }

        private static byte Invert(byte tune) => (byte)((Chord.Semitones.Length - tune) % Chord.Semitones.Length);

        private byte _defaultCapo;
        private Dictionary<string, Chord> _chords;
    }
}
