using System.Collections.Generic;
using System.Linq;

namespace SongBook.Web.Models;

public sealed class SongsViewModel
{
    internal SongsViewModel(IList<Song> songs, SaveData data)
    {
        Songs = songs;
        Data = data;
        FirstSongToLearn = songs.FirstOrDefault(s => s.Ready && !s.Learned);
    }

    internal readonly IList<Song> Songs;
    internal readonly SaveData Data;
    internal readonly Song? FirstSongToLearn;
}