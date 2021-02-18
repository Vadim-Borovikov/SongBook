using System;
using System.Collections.Generic;
using System.Linq;
using GoogleSheetsManager;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace SongBook.Web.Models
{
    public sealed class Manager : IDisposable
    {
        public Manager(IOptions<Config> options)
        {
            _config = options.Value;

            if (string.IsNullOrWhiteSpace(_config.GoogleCredentialJson))
            {
                _config.GoogleCredentialJson = JsonConvert.SerializeObject(_config.GoogleCredential);
            }
            _googleSheetProvider =
                new Provider(_config.GoogleCredentialJson, _config.ApplicationName, _config.GoogleSheetId);
        }

        public void Dispose() => _googleSheetProvider?.Dispose();

        internal void LoadSongs()
        {
            IList<ChordData> chordDatas =
                DataManager.GetValues<ChordData>(_googleSheetProvider, _config.GoogleRangeChords);
            Dictionary<string, Chord> chords = chordDatas.ToDictionary(c => c.Id, c => new Chord(c));
            IList<SongData> songDatas =
                DataManager.GetValues<SongData>(_googleSheetProvider, _config.GoogleRangeIndex);
            IEnumerable<Song> songs = songDatas.Select(sd => new Song(sd.Name, sd.Author, sd.DefaultCapo,
                _googleSheetProvider, _config.GoogleRangePostfix, chords));
            Songs = songs.ToList();
        }

        internal IList<Song> Songs;

        private readonly Config _config;
        private readonly Provider _googleSheetProvider;
    }
}
