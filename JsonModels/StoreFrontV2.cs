using System.Text.Json.Serialization;

namespace ValorantManager.JsonModels
{
    public class StoreFrontV2
    {
        // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
        public class SkinsPanelLayout
        {
            public string[] SingleItemOffers { get; set; }

            [JsonPropertyName("SingleItemOffersRemainingDurationInSeconds")]
            public int SingleItemOffersRemainingDurationInSeconds { get; set; }
        }


        public class Root
        {

            [JsonPropertyName("SkinsPanelLayout")]
            public SkinsPanelLayout SkinsPanelLayout { get; set; }

        }




    }
}
