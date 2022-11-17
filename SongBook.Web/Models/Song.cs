using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GoogleSheetsManager;
using GoogleSheetsManager.Providers;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace SongBook.Web.Models;

internal sealed class Song
{
    [Required]
    [UsedImplicitly]
    [SheetField("Готова")]
    public bool Ready { get; internal set; }

    [Required]
    [UsedImplicitly]
    [SheetField("Выучена")]
    public bool Learned { get; internal set; }

    [Required]
    [UsedImplicitly]
    [SheetField("Название")]
    public string Name { get; internal set; } = null!;

    [Required]
    [UsedImplicitly]
    [SheetField("Автор")]
    public string Author { get; internal set; } = null!;

    [UsedImplicitly]
    [SheetField("Каподастр")]
    public byte? DefaultCapo
    {
        set
        {
            Tune defaultCapo = new(value ?? 0);
            DefaultTune = defaultCapo.Invert();
        }
    }

    [UsedImplicitly]
    [SheetField("Музыка")]
    public Uri? Music { get; internal set; }

    [UsedImplicitly]
    [SheetField("Разбор")]
    public List<Uri>? Tutorials { get; internal set; }

    public Tune DefaultTune { get; private set; }

    public Tune CurrentTune { get; private set; } = new(0);

    public IReadOnlyList<Part> Parts = new List<Part>();

    public async Task LoadAsync(SheetsProvider provider, string sheetPostfix, Dictionary<string, Chord> chords)
    {
        _chords = chords;

        SheetData<HalfBarData> halfBars = await DataManager<HalfBarData>.LoadAsync(provider, $"{Name}{sheetPostfix}",
            formula: true, additionalConverters: AdditionalConverters);
        List<Part> parts = new();
        Part? currentPart = null;
        foreach (HalfBarData halfBarData in halfBars.Instances)
        {
            halfBarData.InitMusic(_chords);
            if (string.IsNullOrWhiteSpace(halfBarData.Part))
            {
                if (currentPart is null)
                {
                    throw new NullReferenceException("Empty part!");
                }
                currentPart.HalfBars.Add(halfBarData);
            }
            else
            {
                if (currentPart is not null)
                {
                    parts.Add(currentPart);
                }
                currentPart = new Part(halfBarData);
            }
        }
        if (currentPart is not null)
        {
            parts.Add(currentPart);
        }
        Parts = parts;
    }

    public void TransposeTo(Tune tune) => TransposeBy(tune - CurrentTune);

    public void TransposeBy(sbyte delta)
    {
        if (delta == 0)
        {
            return;
        }

        CurrentTune += delta;
        foreach (HalfBarData halfBarData in Parts.SelectMany(p => p.HalfBars).Where(h => h.Chord is not null))
        {
            string chordKey = halfBarData.Chord!.TransposeBy(delta);
            halfBarData.SetChord(chordKey, _chords);
        }
    }

    public Tune GetEasiestTune()
    {
        Tune current = CurrentTune;

        Tune? best = null;
        uint? minBarres = null;
        for (byte value = 0; value < Tune.Limit; ++value)
        {
            Tune tune = new(value);
            TransposeTo(tune);
            uint barres = CountBarres();
            if (!minBarres.HasValue || (barres < minBarres))
            {
                minBarres = barres;
                best = tune;
            }
        }

        TransposeTo(current);

        return best!.Value;
    }

    public uint CountBarres() => (uint) Parts.SelectMany(p => p.HalfBars).Count(h => h.HasBarre());

    private Dictionary<string, Chord> _chords = new();

    private static readonly Dictionary<Type, Func<object?, object?>> AdditionalConverters = new()
    {
        { typeof(Uri), Utils.ToUri },
    };
}