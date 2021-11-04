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

        internal Tune DefaultTune { get; private set; }
        public Tune CurrentTune { get; private set; }

        internal IReadOnlyList<Part> Parts;

        public void Load(IList<object> values)
        {
            Ready = values.ToBool(0) ?? false;
            Learned = values.ToBool(1) ?? false;
            Name = values.ToString(2);
            Author = values.ToString(3);
            Music = values.ToUri(5);
            Tutorials = values.ToUris(6);

            var defaultCapo = new Tune(values.ToByte(4) ?? 0);
            DefaultTune = defaultCapo.Invert();
            CurrentTune = new Tune(0);
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

        internal void TransposeTo(Tune tune) => TransposeBy(tune - CurrentTune);

        internal void TransposeBy(sbyte delta)
        {
            if (delta == 0)
            {
                return;
            }

            CurrentTune += delta;
            foreach (HalfBarData halfBarData in Parts.SelectMany(p => p.HalfBars).Where(h => h.Chord != null))
            {
                string chordKey = halfBarData.Chord.TransposeBy(delta);
                halfBarData.SetChord(chordKey, _chords);
            }
        }

        internal Tune GetEasiestTune()
        {
            Tune current = CurrentTune;

            Tune? best = null;
            uint? minBarres = null;
            for (byte value = 0; value < Tune.Limit; ++value)
            {
                var tune = new Tune(value);
                TransposeTo(tune);
                uint barres = CountBarres();
                if (!minBarres.HasValue || (barres < minBarres))
                {
                    minBarres = barres;
                    best = tune;
                }
            }

            TransposeTo(current);

            // ReSharper disable once PossibleInvalidOperationException
            return best.Value;
        }

        internal uint CountBarres() => (uint)Parts.SelectMany(p => p.HalfBars).Count(h => h.HasBarre());

        private Dictionary<string, Chord> _chords;
    }
}
