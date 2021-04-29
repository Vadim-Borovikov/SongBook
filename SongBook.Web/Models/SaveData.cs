using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SongBook.Web.Models
{
    public sealed class SaveData
    {
        [JsonProperty]
        public byte LastOrderedSongId { get; set; }
        [JsonProperty]
        public byte RandomSongId { get; set; }

        [JsonProperty]
        public Dictionary<byte, DateTime> LastPlayed { get; set; }

        public SaveData() => LastPlayed = new Dictionary<byte, DateTime>();
    }
}