using System.Text.Json.Serialization;

namespace ValorantManager.JsonModels
{
    public class CompetitiveTiers
    {
        // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
        public class Tier
        {
            [JsonPropertyName("tier")]
            public int TierNum { get; set; }

            [JsonPropertyName("tierName")]
            public string TierName { get; set; }

            [JsonPropertyName("division")]
            public string Division { get; set; }

            [JsonPropertyName("divisionName")]
            public string DivisionName { get; set; }

            [JsonPropertyName("color")]
            public string Color { get; set; }

            [JsonPropertyName("backgroundColor")]
            public string BackgroundColor { get; set; }

            [JsonPropertyName("smallIcon")]
            public string SmallIcon { get; set; }

            [JsonPropertyName("largeIcon")]
            public string LargeIcon { get; set; }

            [JsonPropertyName("rankTriangleDownIcon")]
            public string RankTriangleDownIcon { get; set; }

            [JsonPropertyName("rankTriangleUpIcon")]
            public string RankTriangleUpIcon { get; set; }
        }

        public class Datum
        {
            [JsonPropertyName("uuid")]
            public string Uuid { get; set; }

            [JsonPropertyName("assetObjectName")]
            public string AssetObjectName { get; set; }

            [JsonPropertyName("tiers")]
            public List<Tier> Tiers { get; set; }

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
