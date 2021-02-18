using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    internal sealed class ChordData : ILoadable
    {
        public string Id;
        public string Semitone;
        public string Postfix;
        public string Bass;
        public string Fingering;
        public bool IsSimple;

        public void Load(IList<object> values)
        {
            Id = values.ToString(0);
            Semitone = values.ToString(1);
            Postfix = values.ToString(2);
            Bass = values.ToString(3);
            Fingering = values.ToString(4);
            IsSimple = values.ToBool(5) ?? false;
        }
    }
}
