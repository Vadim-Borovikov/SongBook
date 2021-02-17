using System;
using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    public sealed class Song : SongData
    {
        internal readonly IReadOnlyList<Part> Parts;

        internal Song(string name, string author, Provider provider, string sheetPostfix,
            Dictionary<string, Chord> chords)
        {
            Name = name;
            Author = author;

            IList<HalfBarData> halfBars = DataManager.GetValues<HalfBarData>(provider, $"{Name}{sheetPostfix}");
            var parts = new List<Part>();
            Part currentPart = null;
            foreach (HalfBarData halfBar in halfBars)
            {
                halfBar.SetChord(chords);
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
    }
}
