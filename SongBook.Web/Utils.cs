using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GoogleSheetsManager;

namespace SongBook.Web
{
    internal static class Utils
    {
        #region Google

        public static List<Uri> ToUris(this IList<object> values, int index) => values.To(index, ToUris);
        public static byte? ToByte(this IList<object> values, int index) => values.To(index, ToByte);

        private static List<Uri> ToUris(object o) => o?.ToString().Split("\n").Select(s => new Uri(s)).ToList();
        private static byte? ToByte(object o) => byte.TryParse(o?.ToString(), out byte i) ? (byte?)i : null;

        #endregion // Google

        public static T GetRandomElement<T>(IList<T> list) => list[Random.Next(list.Count)];

        public static void LogException(Exception ex)
        {
            File.AppendAllText(ExceptionsLogPath, $"{ex}{Environment.NewLine}");
        }

        private const string ExceptionsLogPath = "errors.txt";

        private static readonly Random Random = new Random();
    }
}
