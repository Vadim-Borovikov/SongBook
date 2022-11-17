using System.Collections.Generic;
using Newtonsoft.Json;

namespace SongBook.Web.Models;

internal sealed class SaveData
{
    [JsonProperty]
    public byte LastOrderedSongId { get; set; }
    [JsonProperty]
    public byte RandomSongId { get; set; }

    [JsonProperty]
    public HashSet<byte> AlsoPlayedYesterday { get; set; }

    public SaveData() => AlsoPlayedYesterday = new HashSet<byte>();
}