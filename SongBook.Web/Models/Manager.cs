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
            _saveManager = new SaveManager<SaveData>(_config.SavePath);
        }

        public void Dispose() => _googleSheetProvider?.Dispose();

        internal void LoadIndex()
        {
            IList<Chord> chordsList = DataManager.GetValues<Chord>(_googleSheetProvider, _config.GoogleRangeChords);
            _chords = chordsList.ToDictionary(c => c.ToString(), c => c);

            Songs = DataManager.GetValues<Song>(_googleSheetProvider, _config.GoogleRangeIndex);
            _saveManager.Load();
        }

        internal void Roll()
        {
            ++_saveManager.Data.LastOrderedSongId;
            _saveManager.Data.RandomSongId =
                Utils.GetRandomBytye(1, (byte)(Songs.Count + 1), _saveManager.Data.LastOrderedSongId);
            _saveManager.Save();
        }

        internal void LoadSong(Song song) => song.Load(_googleSheetProvider, _config.GoogleRangePostfix, _chords);

        internal IList<Song> Songs;
        internal SaveData SaveData => _saveManager.Data;

        private Dictionary<string, Chord> _chords;

        private readonly Config _config;
        private readonly Provider _googleSheetProvider;
        private readonly SaveManager<SaveData> _saveManager;
    }
}
