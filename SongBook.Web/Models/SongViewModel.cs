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
            for (int i = 0; i < Song.Parts.Count; ++i)
            {
                Part part = Song.Parts[i];
                bool isRepeat = Song.Parts.Take(i).Any(p => p.Name == part.Name);
                var viewModel = new PartViewModel(part, isRepeat);
                Parts.Add(viewModel);
            }
        }

        public readonly Song Song;
        public readonly int Id;
        public readonly List<PartViewModel> Parts;
    }
}