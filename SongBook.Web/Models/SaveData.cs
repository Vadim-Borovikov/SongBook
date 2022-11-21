using System.Collections.Generic;

namespace SongBook.Web.Models;

internal sealed class SaveData
{
    public byte LastOrderedSongId;
    public byte RandomSongId;
    public HashSet<byte> AlsoPlayedYesterday = new();
}