using System.Collections.Generic;
using System.Linq;

namespace SongBook.Web.Models
{
    public sealed class SongViewModel
    {
        public SongViewModel(Song song, int id)
        {
            Song = song;
            Id = id;
            Parts = new List<PartViewModel>(Song.Parts.Count);
            PartViewModel viewModel = null;
            for (int i = 0; i < Song.Parts.Count; ++i)
            {
                Part part = Song.Parts[i];
                bool isRepeat = Song.Parts.Take(i).Any(p => p.Name == part.Name);
                bool wasProviousCutted = viewModel?.IsCutted ?? false;
                viewModel = new PartViewModel(part, isRepeat, wasProviousCutted);
                Parts.Add(viewModel);
            }
        }

        public readonly Song Song;
        public readonly int Id;
        public readonly List<PartViewModel> Parts;
    }
}