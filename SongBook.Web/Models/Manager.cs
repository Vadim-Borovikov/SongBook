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
            MarkFirstSongToLearn();
            MarkNextRandomSong();

            _saveManager.Save();
        }

        private void MarkNextOrderedSong()
        {
            do
            {
                _saveManager.Data.LastOrderedSongId = (byte)((_saveManager.Data.LastOrderedSongId  + 1) % Songs.Count);
            }
            while (!Songs[_saveManager.Data.LastOrderedSongId].Learned);
            _saveManager.Data.LastPlayed[_saveManager.Data.LastOrderedSongId] = DateTime.Today;
        }

        private void MarkFirstSongToLearn()
        {
            Song first = Songs.FirstOrDefault(s => s.Ready && !s.Learned);
            if (first != null)
            {
                _saveManager.Data.LastPlayed[(byte)Songs.IndexOf(first)] = DateTime.Today;
            }
        }

        private void MarkNextRandomSong()
        {
            var weights = new Dictionary<byte, int>();

            foreach (byte id in _saveManager.Data.LastPlayed.Keys)
            {
                int daysSincePlayed = (int)(DateTime.Today - _saveManager.Data.LastPlayed[id]).TotalDays;
                weights[id] = Math.Max(0, daysSincePlayed - 1);
            }

            int max = weights.Values.Max();

            for (byte id = 0; id < Songs.Count; ++id)
            {
                if (!Songs[id].Ready || !Songs[id].Learned)
                {
                    weights[id] = 0;
                }
                else if (!weights.ContainsKey(id))
                {
                    weights[id] = max + 1;
                }
            }

            _saveManager.Data.RandomSongId =
                (byte) Utils.GetRandomElementWeighted(weights.OrderBy(p => p.Key).Select(p => p.Value).ToList());
            _saveManager.Data.LastPlayed[_saveManager.Data.RandomSongId] = DateTime.Today;
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
