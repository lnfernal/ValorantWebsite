using System.Text.Json.Serialization;

namespace ValorantManager.JsonModels
{

    public class Wallet
    {
        public class Rootobject
        {
            public Balances Balances { get; set; }
        }

        public class Balances
        {
            [JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")]
            public int ValorantPoints { get; set; }
            [JsonPropertyName("e59aa87c-4cbf-517a-5983-6e81511be9b7")]
            public int Radianite { get; set; }
        }
    }
}
