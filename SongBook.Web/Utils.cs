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

        public static bool? ToBool(this IList<object> values, int index) => values.To(index, ToBool);

        private static bool? ToBool(object o) => bool.TryParse(o?.ToString(), out bool i) ? (bool?)i : null;

        public static List<Uri> ToUris(this IList<object> values, int index) => values.To(index, ToUris);

        private static List<Uri> ToUris(object o) => o?.ToString().Split("\n").Select(s => new Uri(s)).ToList();

        #endregion // Google

        public static int GetRandomElementWeighted(List<int> weights)
        {
            int totalWeight = weights.Sum();
            int roll = Random.Next(totalWeight);
            int currentWeight = 0;
            for (int i = 0; i < weights.Count; ++i)
            {
                currentWeight += weights[i];
                if (roll < currentWeight)
                {
                    return i;
                }
            }
            throw new Exception("Failed to get random element");
        }

        public static void LogException(Exception ex)
        {
            File.AppendAllText(ExceptionsLogPath, $"{ex}{Environment.NewLine}");
        }

        private const string ExceptionsLogPath = "errors.txt";

        private static readonly Random Random = new Random();
    }
}
