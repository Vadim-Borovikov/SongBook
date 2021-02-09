using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    internal sealed class HalfBarData : ILoadable
    {
        public string Part { get; private set; }
        public string Chord { get; private set; }
        public string Text { get; private set; }

        public void Load(IList<object> values)
        {
            Part = values.ToString(0);
            Chord = values.ToString(1);
            Text = values.ToString(2);
        }
    }
}
