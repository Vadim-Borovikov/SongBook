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
            IList<Chord> chordsList = DataManager.GetValues<Chord>(_googleSheetProvider, _config.GoogleRangeChords);
            Dictionary<string, Chord> chords = chordsList.ToDictionary(c => c.ToString(), c => c);

            Songs = DataManager.GetValues<Song>(_googleSheetProvider, _config.GoogleRangeIndex);
            foreach (Song song in Songs)
            {
                song.Load(_googleSheetProvider, _config.GoogleRangePostfix, chords);
            }
        }

        internal IList<Song> Songs;

        private readonly Config _config;
        private readonly Provider _googleSheetProvider;
    }
}
