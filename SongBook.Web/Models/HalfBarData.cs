using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    public sealed class HalfBarData : ILoadable
    {
        public string Chord { get; private set; }
        public string Text { get; private set; }

        public void Load(IList<object> values)
        {
            Chord = values.ToString(0);
            Text = values.ToString(1);
        }
    }
}
