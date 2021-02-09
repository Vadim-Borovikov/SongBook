using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    public sealed class Song : SongData
    {
        internal readonly IReadOnlyList<HalfBarData> HalfBars;

        internal Song(string name, string author, Provider provider, string sheetPostfix)
        {
            Name = name;
            Author = author;
            HalfBars = new List<HalfBarData>(DataManager.GetValues<HalfBarData>(provider, $"{Name}{sheetPostfix}"));
        }
    }
}
