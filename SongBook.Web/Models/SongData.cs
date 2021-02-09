using System.Collections.Generic;
using GoogleSheetsManager;

namespace SongBook.Web.Models
{
    public class SongData : ILoadable
    {
        public string Name { get; protected set; }
        public string Author { get; protected set; }

        public void Load(IList<object> values)
        {
            Name = values.ToString(0);
            Author = values.ToString(1);
        }
    }
}
