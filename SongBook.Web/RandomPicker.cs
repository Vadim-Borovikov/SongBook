using System;
using System.Collections.Generic;

namespace SongBook.Web;

internal sealed class RandomPicker
{
    public T GetRandomElement<T>(IList<T> list) => list[_random.Next(list.Count)];

    private readonly Random _random = new();
}