using System.Text.Json.Serialization;

namespace ValorantManager.JsonModels
{
    public class Cards
    {
        // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
        public class Datum
        {
            [JsonPropertyName("uuid")]
            public string Uuid { get; set; }

            [JsonPropertyName("displayName")]
            public string DisplayName { get; set; }

            [JsonPropertyName("isHiddenIfNotOwned")]
            public bool IsHiddenIfNotOwned { get; set; }

            [JsonPropertyName("themeUuid")]
            public object ThemeUuid { get; set; }

            [JsonPropertyName("displayIcon")]
            public string DisplayIcon { get; set; }

            [JsonPropertyName("smallArt")]
            public string SmallArt { get; set; }

            [JsonPropertyName("wideArt")]
            public string WideArt { get; set; }

            [JsonPropertyName("largeArt")]
            public string LargeArt { get; set; }

            [JsonPropertyName("assetPath")]
            public string AssetPath { get; set; }
        }

        public class Root
        {
            [JsonPropertyName("status")]
            public int Status { get; set; }

            [JsonPropertyName("data")]
            public List<Datum> Data { get; set; }
        }


    }
}
