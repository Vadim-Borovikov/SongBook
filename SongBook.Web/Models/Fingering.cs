using System;
using System.Linq;

namespace SongBook.Web.Models;

public sealed class Fingering
{
    internal Fingering(string? fingering = null) => _parts = fingering?.Split(' ') ?? Array.Empty<string>();

    public override string ToString() => string.Join(' ', _parts);

    internal bool IsPresent => _parts.Length > 0;

    internal bool HasBarre()
    {
        bool hasFret = false;
        foreach (string part in _parts)
        {
            if (part == "O")
            {
                return false;
            }
            hasFret |= part.Any(char.IsDigit);
        }
        return hasFret;
    }

    private readonly string[] _parts;
}