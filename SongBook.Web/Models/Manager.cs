using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleSheetsManager;
using GoogleSheetsManager.Providers;
using GryphonUtilities;

namespace SongBook.Web.Models;

public sealed class Manager : IDisposable
{
    public Manager(Config config)
    {
        _config = config;
        _googleSheetProvider = new SheetsProvider(_config, _config.GoogleSheetId);
        _saveManager = new SaveManager<SaveData>(_config.SavePath);
    }

    public void Dispose() => _googleSheetProvider.Dispose();

    internal async Task LoadIndexAsync()
    {
        SheetData<Chord> chords = await DataManager<Chord>.LoadAsync(_googleSheetProvider, _config.GoogleRangeChords,
            additionalConverters: AdditionalConverters);
        _chords = chords.Instances.ToDictionary(c => c.ToString(), c => c);

        SheetData<Song> songs = await DataManager<Song>.LoadAsync(_googleSheetProvider, _config.GoogleRangeIndex,
            additionalConverters: AdditionalConverters);
        Songs = songs.Instances;
        _saveManager.Load();
    }

    internal void Roll()
    {
        HashSet<byte> yesterdaySongs = new()
        {
            _saveManager.Data.LastOrderedSongId,
            _saveManager.Data.RandomSongId
        };
        yesterdaySongs.UnionWith(_saveManager.Data.AlsoPlayedYesterday);
        _saveManager.Data.AlsoPlayedYesterday = new HashSet<byte>();

        _saveManager.Data.LastOrderedSongId = GetNextOrderedSong();
        HashSet<byte> todaySongs = new()
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

        HashSet<byte> excluded = new(yesterdaySongs);
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
        Song? first = Songs.FirstOrDefault(s => s.Ready && !s.Learned);
        return first is null ? null : (byte?) Songs.IndexOf(first);
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

    internal Task LoadSongAsync(Song song)
    {
        return song.LoadAsync(_googleSheetProvider, _config.GoogleRangePostfix, _chords);
    }

    internal IList<Song> Songs = new List<Song>();
    internal SaveData SaveData => _saveManager.Data;

    private Dictionary<string, Chord> _chords = new();

    private readonly Config _config;
    private readonly SheetsProvider _googleSheetProvider;
    private readonly SaveManager<SaveData> _saveManager;

    private static readonly Dictionary<Type, Func<object?, object?>> AdditionalConverters = new()
    {
        { typeof(byte), o => o.ToByte() },
        { typeof(byte?), o => o.ToByte() },
        { typeof(Uri), Utils.ToUri },
        { typeof(List<Uri>), Utils.ToUris }
    };
}