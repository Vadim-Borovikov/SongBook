using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleSheetsManager;
using GoogleSheetsManager.Providers;

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

        public void Load(IDictionary<string, object> valueSet)
        {
            Ready = valueSet[ReadyTitle]?.ToBool() ?? false;
            Learned = valueSet[LearnedTitle]?.ToBool() ?? false;
            Name = valueSet[NameTitle]?.ToString();
            Author = valueSet[AuthorTitle]?.ToString();
            Music = valueSet[MusicTitle]?.ToUri();
            Tutorials = valueSet[TutorialsTitle]?.ToUris();

            var defaultCapo = new Tune(valueSet[DefaultCapoTitle]?.ToByte() ?? 0);
            DefaultTune = defaultCapo.Invert();
            CurrentTune = new Tune(0);
        }

        internal async Task LoadAsync(SheetsProvider provider, string sheetPostfix, Dictionary<string, Chord> chords)
        {
            _chords = chords;

            IList<HalfBarData> halfBars =
                await DataManager.GetValuesAsync<HalfBarData>(provider, $"{Name}{sheetPostfix}", true);
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

        private const string ReadyTitle = "Готова";
        private const string LearnedTitle = "Выучена";
        private const string NameTitle = "Название";
        private const string AuthorTitle = "Автор";
        private const string DefaultCapoTitle = "Каподастр";
        private const string MusicTitle = "Музыка";
        private const string TutorialsTitle = "Разбор";

        private Dictionary<string, Chord> _chords;
    }
}
