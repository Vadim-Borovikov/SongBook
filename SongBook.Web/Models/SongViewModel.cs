using System.Collections.Generic;
using System.Linq;

namespace SongBook.Web.Models;

public sealed class SongViewModel
{
    public SongViewModel(Song song, int id, bool? showRepeats)
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

    public byte GetCurrentCapo() => Song.CurrentTune.Invert().Value;

    public float GetCurrentBarresPercent()
    {
        uint current = Song.CountBarres();
        int max = Song.Parts.SelectMany(p => p.HalfBars).Count();
        return current * 1.0f / max;
    }

    public readonly Song Song;
    public readonly int Id;
    public readonly bool ShowRepeats;
    public readonly List<PartViewModel> Parts;
}