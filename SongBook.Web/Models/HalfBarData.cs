using System;
using System.Collections.Generic;
using GoogleSheetsManager;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace SongBook.Web.Models;

public sealed class HalfBarData
{
    [UsedImplicitly]
    [SheetField("Часть")]
    public string? Part;

    [UsedImplicitly]
    [SheetField("Аккорд")]
    public string? Music;

    [UsedImplicitly]
    [SheetField("Вариант")]
    public int? InitialChordOptionTitle
    {
        set => _initialChordOption = (value ?? 1) - 1;
    }

    [UsedImplicitly]
    [SheetField("Ритм")]
    public string? Rythm;

    [UsedImplicitly]
    [SheetField("Текст")]
    public string? Text;

    internal Uri? Tab { get; private set; }

    public Chord? Chord { get; private set; }
    public int ChordOption { get; private set; }

    internal bool HasBarre() => Chord is not null && Chord.Fingerings[ChordOption].HasBarre();

    internal void SetChord(string chordKey, Dictionary<string, Chord> chords)
    {
        Music = chordKey;
        InitMusic(chords);
    }

    internal void InitMusic(Dictionary<string, Chord> chords)
    {
        if (string.IsNullOrWhiteSpace(Music))
        {
            return;
        }

        if (chords.ContainsKey(Music))
        {
            Chord = chords[Music];
            ChordOption = Chord.Fingerings[_initialChordOption].IsPresent ? 0 : _initialChordOption;
        }
        else
        {
            string tabUrl = Music.Substring(TabPrefix.Length, Music.Length - TabPrefix.Length - TabPostfix.Length);
            Tab = new Uri(tabUrl);
        }
    }

    private const string TabPrefix = "=IMAGE(\"";
    private const string TabPostfix = "\")";

    private int _initialChordOption;
}