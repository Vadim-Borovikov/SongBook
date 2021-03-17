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
            MarkNextOrderedSong();
            MarkNextRandomSong();
            _saveManager.Save();
        }

        internal void Learn(byte id)
        {
            _saveManager.Data.LearningSongId = id;
            _saveManager.Save();
        }

        private void MarkNextRandomSong()
        {
            List<byte> songs = Enumerable.Range(1, Songs.Count).Select(i => (byte)i).ToList();
            songs.Remove(_saveManager.Data.LearningSongId);
            songs.Remove(_saveManager.Data.LastOrderedSongId);
            songs.Remove(_saveManager.Data.RandomSongId);
            _saveManager.Data.RandomSongId = Utils.GetRandomElement(songs);
        }

        private void MarkNextOrderedSong()
        {
            do
            {
                _saveManager.Data.LastOrderedSongId = (byte)(_saveManager.Data.LastOrderedSongId % Songs.Count + 1);
            }
            while (_saveManager.Data.LastOrderedSongId == _saveManager.Data.LearningSongId);
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
