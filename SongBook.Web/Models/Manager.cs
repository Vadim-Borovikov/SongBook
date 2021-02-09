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
            Config config = options.Value;

            if (string.IsNullOrWhiteSpace(config.GoogleCredentialJson))
            {
                config.GoogleCredentialJson = JsonConvert.SerializeObject(config.GoogleCredential);
            }
            _googleSheetProvider =
                new Provider(config.GoogleCredentialJson, config.ApplicationName, config.GoogleSheetId);
            IList<SongData> songDatas = DataManager.GetValues<SongData>(_googleSheetProvider, config.GoogleRangeIndex);
            IEnumerable<Song> songs =
                songDatas.Select(sd => new Song(sd.Name, sd.Author, _googleSheetProvider, config.GoogleRangePostfix));
            Songs = songs.ToList();
        }

        public void Dispose() => _googleSheetProvider?.Dispose();

        internal readonly IList<Song> Songs;
        private readonly Provider _googleSheetProvider;
    }
}
