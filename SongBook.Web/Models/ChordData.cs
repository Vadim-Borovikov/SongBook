using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    public class ChordData : ILoadable
    {
        public string Id;
        public string Semitone;
        public string Postfix;
        public string Fingering;
        public bool IsSimple;

        public void Load(IList<object> values)
        {
            Id = values.ToString(0);
            Semitone = values.ToString(1);
            Postfix = values.ToString(2);
            Fingering = values.ToString(3);
            IsSimple = values.ToBool(4) ?? false;
        }
    }
}
