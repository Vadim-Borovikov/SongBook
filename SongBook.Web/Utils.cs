using System;
using System.Collections.Generic;
using System.IO;

namespace SongBook.Web
{
    internal static class Utils
    {
        public static T GetRandomElement<T>(IList<T> list) => list[Random.Next(list.Count)];

        public static void LogException(Exception ex)
        {
            File.AppendAllText(ExceptionsLogPath, $"{ex}{Environment.NewLine}");
        }

        private const string ExceptionsLogPath = "errors.txt";

        private static readonly Random Random = new Random();
    }
}
