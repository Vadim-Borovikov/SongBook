using System.Collections.Generic;
using System.Linq;

namespace SongBook.Web.Models;

public sealed class SongViewModel
{
    internal SongViewModel(Song song, int id, bool? showRepeats)
    {
        Song = song;
        Id = id;
        ShowRepeats = showRepeats.HasValue && showRepeats.Value;
        Parts = new List<PartViewModel>(Song.Parts.Count);
        PartViewModel? viewModel = null;
        for (int i = 0; i < Song.Parts.Count; ++i)
        {
            Part part = Song.Parts[i];
            bool isRepeat = Song.Parts.Take(i).Any(p => p.Name == part.Name);
            bool wasProviousCutted = viewModel?.IsCutted ?? false;
            viewModel = new PartViewModel(part, isRepeat, wasProviousCutted);
            Parts.Add(viewModel);
        }
    }

    internal byte GetCurrentCapo() => Song.CurrentTune.Invert().Value;

    internal float GetCurrentBarresPercent()
    {
        uint current = Song.CountBarres();
        int max = Song.Parts.SelectMany(p => p.HalfBars).Count();
        return current * 1.0f / max;
    }

    internal readonly Song Song;
    internal readonly int Id;
    internal readonly bool ShowRepeats;
    internal readonly List<PartViewModel> Parts;
}