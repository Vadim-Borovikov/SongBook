using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleSheetsManager;
using GoogleSheetsManager.Documents;
using GryphonUtilities;

namespace SongBook.Web.Models;

public sealed class Manager : IDisposable
{
    public Manager(Config config)
    {
        _config = config;
        _documentsManager = new DocumentsManager(_config);
        _document = _documentsManager.GetOrAdd(_config.GoogleSheetId);
        _saveManager = new SaveManager<SaveData>(_config.SavePath);
    }

    private readonly Document _document;

    public void Dispose() => _documentsManager.Dispose();

    internal async Task LoadIndexAsync()
    {
        Dictionary<Type, Func<object?, object?>> additionalConverters = new()
        {
            { typeof(Uri), o => o.ToUri() },
            { typeof(List<Uri>), o => o.ToUris() }
        };
        additionalConverters[typeof(byte)] = additionalConverters[typeof(byte?)] = o => o.ToByte();

        Sheet chordsSheet = _document.GetOrAddSheet(_config.GoogleTitleChords, additionalConverters);
        SheetData<Chord> chords = await chordsSheet.LoadAsync<Chord>(_config.GoogleRangeChords);
        _chords = chords.Instances.ToDictionary(c => c.ToString(), c => c);

        Sheet indexSheet = _document.GetOrAddSheet(_config.GoogleTitleIndex, additionalConverters);
        SheetData<Song> songs = await indexSheet.LoadAsync<Song>(_config.GoogleRangeIndex);
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
        return _picker.GetRandomElement(songs);
    }

    internal Task LoadSongAsync(Song song) => song.LoadAsync(_document, _config.GoogleRangeSong, _chords);

    internal IList<Song> Songs = new List<Song>();
    internal SaveData SaveData => _saveManager.Data;

    private Dictionary<string, Chord> _chords = new();

    private readonly Config _config;
    private readonly DocumentsManager _documentsManager;
    private readonly SaveManager<SaveData> _saveManager;
    private readonly RandomPicker _picker = new();
}