using System;
using System.Collections.Generic;
using System.Linq;
using GryphonUtilities;

namespace SongBook.Web;

internal static class Utils
{
    public static readonly LogManager LogManager = new();

    public static void StartLogWith(string systemTimeZoneId)
    {
        LogManager.SetTimeZone(systemTimeZoneId);
        LogManager.LogMessage();
        LogManager.LogTimedMessage("Startup");
    }

    public static T GetRandomElement<T>(IList<T> list) => list[Random.Next(list.Count)];

    public static byte? ToByte(this object? o)
    {
        if (o is byte b)
        {
            return b;
        }
        return byte.TryParse(o?.ToString(), out b) ? b : null;
    }

    public static Uri? ToUri(this object? o)
    {
        if (o is Uri uri)
        {
            return uri;
        }
        string? uriString = o?.ToString();
        return string.IsNullOrWhiteSpace(uriString) ? null : new Uri(uriString);
    }

    public static List<Uri>? ToUris(this object? o)
    {
        if (o is IEnumerable<Uri> l)
        {
            return l.ToList();
        }
        return o?.ToString()?.Split("\n").Select(ToUri).RemoveNulls().ToList();
    }

    private static readonly Random Random = new();
}