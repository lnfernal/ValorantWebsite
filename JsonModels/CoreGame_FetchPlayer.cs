using System.Text.Json.Serialization;

namespace ValorantManager.JsonModels
{
    public class CoreGame_FetchPlayer
    {
        public class Root
        {
            [JsonPropertyName("Subject")]
            public string Subject { get; set; }

            [JsonPropertyName("MatchID")]
            public string MatchID { get; set; }

            [JsonPropertyName("Version")]
            public long GameStartEpochms { get; set; }
        }
    }
}
