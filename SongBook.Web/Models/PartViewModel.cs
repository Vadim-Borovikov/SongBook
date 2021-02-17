using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

namespace SongBook.Web.Models
{
    public sealed class PartViewModel
    {
        internal PartViewModel(Part part)
        {
            Part = part;

            FirstBarChords = new List<IList<Chord>>();
            SecondBarChords = new List<IList<Chord>>();
            TextLines = new List<string>();

            for (int i = 0; i < Part.HalfBars.Count; i += LineSize)
            {
                List<HalfBarData> line = Part.HalfBars.Skip(i).Take(LineSize).ToList();

                List<Chord> chords = line.Select(l => l.Chord).ToList();
                FirstBarChords.Add(GetChords(chords.Take(2).ToList()));
                SecondBarChords.Add(GetChords(chords.Skip(2).ToList()));

                string words = line.Select(l => l.Text).Join("");
                string previous = TextLines.LastOrDefault();
                if (words.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(previous) && !previous.EndsWith(' '))
                    {
                        TextLines[TextLines.Count - 1] = $"{previous}-";
                        words = $"-{words}";
                    }
                    else
                    {
                        words = char.ToUpper(words[0]) + words.Substring(1);
                    }
                }
                TextLines.Add(words);
            }
        }

        private static IList<Chord> GetChords(IList<Chord> chords)
        {
            if ((chords.Count == 2) && (chords[0] == chords[1]))
            {
                return new[] { chords[0] };
            }

            return chords;
        }

        public readonly Part Part;
        public readonly IList<IList<Chord>> FirstBarChords;
        public readonly IList<IList<Chord>> SecondBarChords;
        public readonly IList<string> TextLines;

        private const int LineSize = 4;
    }
}