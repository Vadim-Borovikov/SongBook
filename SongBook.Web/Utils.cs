using System;
using System.Collections.Generic;
using System.IO;
using GoogleSheetsManager;

namespace SongBook.Web
{
    internal static class Utils
    {
        #region Google

        public static bool? ToBool(this IList<object> values, int index) => values.To(index, ToBool);

        private static bool? ToBool(object o) => bool.TryParse(o?.ToString(), out bool i) ? (bool?)i : null;

        #endregion // Google


        public static void LogException(Exception ex)
        {
            File.AppendAllText(ExceptionsLogPath, $"{ex}{Environment.NewLine}");
        }

        private const string ExceptionsLogPath = "errors.txt";
    }
}
