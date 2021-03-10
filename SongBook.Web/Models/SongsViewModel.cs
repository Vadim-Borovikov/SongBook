using System.Collections.Generic;

namespace SongBook.Web.Models
{
    public sealed class SongsViewModel
    {
        public SongsViewModel(IList<Song> songs, SaveData data)
        {
            Songs = songs;
            Data = data;
        }

        public readonly IList<Song> Songs;
        public readonly SaveData Data;
    }
}