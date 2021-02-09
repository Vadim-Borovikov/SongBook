using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

namespace SongBook.Web.Models
{
    public sealed class SongViewModel
    {
        public SongViewModel(Song song)
        {
            Song = song;

            FirstBarChordsLines = new List<string>();
            SecondBarChordsLines = new List<string>();
            TextLines = new List<string>();

            for (int i = 0; i < song.HalfBars.Count; i += LineSize)
            {
                List<HalfBarData> line = song.HalfBars.Skip(i).Take(LineSize).ToList();

                List<string> chords = line.Select(l => l.Chord).ToList();
                FirstBarChordsLines.Add(GetChordsLine(chords.Take(2).ToList()));
                SecondBarChordsLines.Add(GetChordsLine(chords.Skip(2).ToList()));

                string words = line.Select(l => l.Text).Join("");
                if (words.Length > 0)
                {
                    words = char.ToUpper(words[0]) + words.Substring(1);
                }
                TextLines.Add(words);
            }
        }

        private static string GetChordsLine(IReadOnlyList<string> chords)
        {
            if (chords.Count != 2)
            {
                return chords.Join("?");
            }

            return chords[0] == chords[1] ? chords[0] : $"{chords[0]}-{chords[1]}";
        }

        public readonly IList<string> FirstBarChordsLines;
        public readonly IList<string> SecondBarChordsLines;
        public readonly IList<string> TextLines;
        public readonly Song Song;

        private const int LineSize = 4;
    }
}