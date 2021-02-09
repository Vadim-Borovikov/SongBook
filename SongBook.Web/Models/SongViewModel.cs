using System.Collections.Generic;
using System.Linq;

namespace SongBook.Web.Models
{
    public sealed class SongViewModel
    {
        public SongViewModel(Song song)
        {
            Song = song;
            Parts = Song.Parts.Select(p => new PartViewModel(p)).ToList();
        }

        public readonly Song Song;
        public readonly IReadOnlyList<PartViewModel> Parts;
    }
}