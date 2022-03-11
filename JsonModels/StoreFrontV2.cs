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

        public class Cost
        {
            [JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")]
            public int _85ad13f73d1b51289eb27cd8ee0b5741 { get; set; }
        }

        public class Offer
        {
            [JsonPropertyName("OfferID")]
            public string OfferID { get; set; }

            [JsonPropertyName("StartDate")]
            public string StartDate { get; set; }

            [JsonPropertyName("Cost")]
            public Cost Cost { get; set; }
        }


        public class BonusStoreOffer
        {
            [JsonPropertyName("BonusOfferID")]
            public string BonusOfferID { get; set; }

            [JsonPropertyName("Offer")]
            public Offer Offer { get; set; }

            [JsonPropertyName("DiscountPercent")]
            public int DiscountPercent { get; set; }

            [JsonPropertyName("DiscountCosts")]
            public Cost DiscountCosts { get; set; }

            
        }


        public class BonusStore
        {
            [JsonPropertyName("BonusStoreOffers")]
            public List<BonusStoreOffer> BonusStoreOffers { get; set; }

            [JsonPropertyName("BonusStoreRemainingDurationInSeconds")]
            public int BonusStoreRemainingDurationInSeconds { get; set; }
        }

        public class Root
        {

            [JsonPropertyName("SkinsPanelLayout")]
            public SkinsPanelLayout SkinsPanelLayout { get; set; }

            [JsonPropertyName("BonusStore")]
            public BonusStore BonusStore { get; set; }
        }




    }
}
