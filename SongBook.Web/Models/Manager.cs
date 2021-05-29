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
            var yesterdaySongs = new HashSet<byte>
            {
                _saveManager.Data.LastOrderedSongId,
                _saveManager.Data.RandomSongId
            };
            yesterdaySongs.UnionWith(_saveManager.Data.AlsoPlayedYesterday);
            _saveManager.Data.AlsoPlayedYesterday = new HashSet<byte>();

            _saveManager.Data.LastOrderedSongId = GetNextOrderedSong();
            var todaySongs = new HashSet<byte>
            {
                _saveManager.Data.LastOrderedSongId
            };

            byte? firstSongToLearn = GetFirstSongToLearn();
            if (firstSongToLearn.HasValue)
            {
                todaySongs.Add(firstSongToLearn.Value);
                _saveManager.Data.AlsoPlayedYesterday.Add(firstSongToLearn.Value);
            }

            byte tomorrowSong = GetNextOrderedSong();

            var excluded = new HashSet<byte>(yesterdaySongs);
            excluded.UnionWith(todaySongs);
            excluded.Add(tomorrowSong);

            _saveManager.Data.RandomSongId = GetNextRandomSong(excluded);

            todaySongs.Add(_saveManager.Data.RandomSongId);

            _saveManager.Save();
        }

        private byte GetNextOrderedSong() => GetNextOrderedSong(_saveManager.Data.LastOrderedSongId);
        private byte GetNextOrderedSong(byte current)
        {
            byte next = current;
            do
            {
                next = (byte)((next + 1) % Songs.Count);
            }
            while (!Songs[next].Learned);
            return next;
        }

        private byte? GetFirstSongToLearn()
        {
            Song first = Songs.FirstOrDefault(s => s.Ready && !s.Learned);
            return first == null ? null : (byte?)Songs.IndexOf(first);
        }

        private byte GetNextRandomSong(IEnumerable<byte> excluded)
        {
            List<byte> songs = Enumerable.Range(0, Songs.Count)
                                         .Where(i => Songs[i].Learned)
                                         .Select(i => (byte)i)
                                         .Except(excluded)
                                         .ToList();
            return Utils.GetRandomElement(songs);
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
