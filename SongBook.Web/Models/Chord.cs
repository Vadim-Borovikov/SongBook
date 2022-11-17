using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GoogleSheetsManager;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace SongBook.Web.Models;

internal sealed class Chord
{
    public static readonly string[] Semitones =
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

    [Required]
    [UsedImplicitly]
    [SheetField("Полутон")]
    public string Semitone = null!;

    [UsedImplicitly]
    [SheetField("Вариация")]
    public string? Postfix;

    [Required]
    [UsedImplicitly]
    [SheetField("Бас")]
    public string Bass = null!;

    [Required]
    [UsedImplicitly]
    [SheetField("Простой")]
    public bool IsSimple;

    [UsedImplicitly]
    [SheetField("Аппликатура 1")]
    public string? Fingerings1
    {
        set => Fingerings[0] = new Fingering(value);
    }

    [UsedImplicitly]
    [SheetField("Аппликатура 2")]
    public string? Fingerings2
    {
        set => Fingerings[1] = new Fingering(value);
    }

    [UsedImplicitly]
    [SheetField("Аппликатура 3")]
    public string? Fingerings3
    {
        set => Fingerings[2] = new Fingering(value);
    }

    public readonly IList<Fingering> Fingerings = new Fingering[3];

    public Chord() { }

    public override string ToString()
    {
        return Bass == Semitone ? $"{Semitone}{Postfix}" : $"{Semitone}{Postfix}/{Bass}";
    }

    public string TransposeBy(sbyte delta)
    {
        string semitone = TransposeBy(Semitone, delta);
        string bass = TransposeBy(Bass, delta);
        Chord transposed = new(semitone, Postfix, bass, Fingerings, IsSimple);
        return transposed.ToString();
    }

    private Chord(string semitone, string? postfix, string bass, IList<Fingering> fingerings, bool isSimple)
    {
        Semitone = semitone;
        Postfix = postfix;
        Bass = bass;
        Fingerings = fingerings;
        IsSimple = isSimple;
    }

    private static string TransposeBy(string semitone, sbyte delta)
    {
        if (!Semitones.Contains(semitone))
        {
            return semitone;
        }
        Tune tune = new((byte) Array.IndexOf(Semitones, semitone));
        tune += delta;
        return Semitones[tune.Value];
    }
}