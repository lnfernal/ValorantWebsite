using System.Text.Json.Serialization;

namespace ValorantManager.JsonModels
{
    public class UserInfo
    {
        public class Acct
        {

            [JsonPropertyName("game_name")]
            public string GameName { get; set; }

            [JsonPropertyName("tag_line")]
            public string TagLine { get; set; }

            [JsonPropertyName("created_at")]
            public long CreatedAt { get; set; }
        }

        public class Root
        {

            [JsonPropertyName("sub")]
            public string Sub { get; set; }

            [JsonPropertyName("acct")]
            public Acct Acct { get; set; }

        }
    }

}
