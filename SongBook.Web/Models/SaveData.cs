using System;
using System.Collections.Generic;

namespace SongBook.Web.Models
{
    public sealed class SaveData
    {
        public byte LastOrderedSongId;
        public byte RandomSongId;

        internal readonly Dictionary<byte, DateTime> LastPlayed;

        public SaveData() => LastPlayed = new Dictionary<byte, DateTime>();
    }
}