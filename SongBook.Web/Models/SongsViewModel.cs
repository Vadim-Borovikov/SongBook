using System.Collections.Generic;
using System.Linq;

namespace SongBook.Web.Models
{
    public sealed class SongsViewModel
    {
        public SongsViewModel(IList<Song> songs, SaveData data)
        {
            Songs = songs;
            Data = data;
            FirstSongToLearn = songs.FirstOrDefault(s => s.Ready && !s.Learned);
        }

        public readonly IList<Song> Songs;
        public readonly SaveData Data;
        public readonly Song FirstSongToLearn;
    }
}