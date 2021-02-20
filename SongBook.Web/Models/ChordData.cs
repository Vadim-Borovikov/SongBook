using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    internal sealed class ChordData : ILoadable
    {
        public string Semitone;
        public string Postfix;
        public string Bass;
        public string Fingering;
        public bool IsSimple;

        public void Load(IList<object> values)
        {
            Semitone = values.ToString(0);
            Postfix = values.ToString(1);
            Bass = values.ToString(2);
            Fingering = values.ToString(3);
            IsSimple = values.ToBool(4) ?? false;
        }

        public override string ToString() => Bass == Semitone ? $"{Semitone}{Postfix}" : $"{Semitone}{Postfix}/{Bass}";
    }
}
