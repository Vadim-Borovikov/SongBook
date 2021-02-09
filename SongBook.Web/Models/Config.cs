using System.Collections.Generic;
using Newtonsoft.Json;

namespace SongBook.Web.Models
{
    public sealed class Config
    {
        [JsonProperty]
        public string ApplicationName { get; set; }

        [JsonProperty]
        public string GoogleSheetId { get; set; }

        [JsonProperty]
        public string GoogleRangeIndex { get; set; }

        [JsonProperty]
        public string GoogleRangePostfix { get; set; }

        [JsonProperty]
        public Dictionary<string, string> GoogleCredential { get; set; }

        [JsonProperty]
        public string GoogleCredentialJson { get; set; }
    }
}
