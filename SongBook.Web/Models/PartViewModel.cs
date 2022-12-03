using System;
using System.Collections.Generic;
using System.Linq;
using GryphonUtilities;

namespace SongBook.Web.Models;

internal sealed class PartViewModel
{
    public PartViewModel(Part part, bool isRepeat, bool wasProviousCutted)
    {
        Part = part;
        IsRepeat = isRepeat;

        Tabs = new List<Uri>();
        FirstBarChords = new List<IList<ChordViewModel>>();
        SecondBarChords = new List<IList<ChordViewModel>>();
        FirstBarRythms = new List<string>();
        SecondBarRythms = new List<string>();
        TextLines = new List<string>();

        bool isCutted = false;
        for (int i = 0; i < Part.HalfBars.Count; i += LineSize)
        {
            List<HalfBarData> line = Part.HalfBars.Skip(i).Take(LineSize).ToList();

            Uri? tab = line[0].Tab;
            if (tab is null)
            {
                List<ChordViewModel> chords = line.Select(l => new ChordViewModel(l.Chord, l.ChordOption)).ToList();
                FirstBarChords.Add(GetChords(chords.Take(2).ToList()));
                SecondBarChords.Add(GetChords(chords.Skip(2).ToList()));
            }
            else
            {
                Tabs.Add(tab);
            }

            List<string?> rythms = line.Select(l => l.Rythm).ToList();
            FirstBarRythms.Add(string.Join("", rythms.Take(2)));
            SecondBarRythms.Add(string.Join("", rythms.Skip(2)));

            string words = string.Join("", line.Select(l => l.Text));
            isCutted = false;
            if (!string.IsNullOrWhiteSpace(words))
            {
                isCutted = !words.EndsWith(' ');
                if (isCutted && !words.EndsWith('-'))
                {
                    words = $"{words}-";
                }

                if (wasProviousCutted)
                {
                    words = $"-{words}";
                }
                else
                {
                    words = char.ToUpper(words[0]) + words[1..];
                }
            }
            TextLines.Add(words);
            wasProviousCutted = isCutted;
        }
        IsCutted = isCutted;
    }

    private static IList<ChordViewModel> GetChords(IList<ChordViewModel> chords)
    {
        if ((chords.Count == 2) && Equals(chords[0], chords[1]))
        {
            return chords[0].WrapWithList();
        }

        return chords;
    }

    public readonly Part Part;
    public readonly IList<IList<ChordViewModel>> FirstBarChords;
    public readonly IList<IList<ChordViewModel>> SecondBarChords;
    public readonly IList<Uri> Tabs;
    public readonly IList<string> FirstBarRythms;
    public readonly IList<string> SecondBarRythms;
    public readonly IList<string> TextLines;
    public readonly bool IsRepeat;
    public readonly bool IsCutted;

    private const int LineSize = 4;
}