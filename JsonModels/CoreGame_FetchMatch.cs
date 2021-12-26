using System.Text.Json.Serialization;

namespace ValorantManager.JsonModels
{
    public class CoreGame_FetchMatch
    {

        public class PlayerIdentity
        {
            [JsonPropertyName("Subject")]
            public string Subject { get; set; }

            [JsonPropertyName("PlayerCardID")]
            public string PlayerCardID { get; set; }

            [JsonPropertyName("PlayerTitleID")]
            public string PlayerTitleID { get; set; }

            [JsonPropertyName("AccountLevel")]
            public int AccountLevel { get; set; }

            [JsonPropertyName("PreferredLevelBorderID")]
            public string PreferredLevelBorderID { get; set; }

            [JsonPropertyName("Incognito")]
            public bool Incognito { get; set; }

            [JsonPropertyName("HideAccountLevel")]
            public bool HideAccountLevel { get; set; }
        }

        public class SeasonalBadgeInfo
        {
            [JsonPropertyName("SeasonID")]
            public string SeasonID { get; set; }

            [JsonPropertyName("NumberOfWins")]
            public int NumberOfWins { get; set; }

            [JsonPropertyName("WinsByTier")]
            public object WinsByTier { get; set; }

            [JsonPropertyName("Rank")]
            public int Rank { get; set; }

            [JsonPropertyName("LeaderboardRank")]
            public int LeaderboardRank { get; set; }
        }

        public class Player
        {
            [JsonPropertyName("Subject")]
            public string Subject { get; set; }

            [JsonPropertyName("TeamID")]
            public string TeamID { get; set; }

            [JsonPropertyName("CharacterID")]
            public string CharacterID { get; set; }

            [JsonPropertyName("PlayerIdentity")]
            public PlayerIdentity PlayerIdentity { get; set; }

            [JsonPropertyName("SeasonalBadgeInfo")]
            public SeasonalBadgeInfo SeasonalBadgeInfo { get; set; }

            [JsonPropertyName("IsCoach")]
            public bool IsCoach { get; set; }
        }

        public class MatchmakingData
        {
            [JsonPropertyName("QueueID")]
            public string QueueID { get; set; }

            [JsonPropertyName("IsRanked")]
            public bool IsRanked { get; set; }
        }

        public class Root
        {
            [JsonPropertyName("MatchID")]
            public string MatchID { get; set; }

            [JsonPropertyName("Version")]
            public long Version { get; set; }

            [JsonPropertyName("State")]
            public string State { get; set; }

            [JsonPropertyName("MapID")]
            public string MapID { get; set; }

            [JsonPropertyName("ModeID")]
            public string ModeID { get; set; }

            [JsonPropertyName("ProvisioningFlow")]
            public string ProvisioningFlow { get; set; }

            [JsonPropertyName("GamePodID")]
            public string GamePodID { get; set; }

            [JsonPropertyName("AllMUCName")]
            public string AllMUCName { get; set; }

            [JsonPropertyName("TeamMUCName")]
            public string TeamMUCName { get; set; }

            [JsonPropertyName("TeamVoiceID")]
            public string TeamVoiceID { get; set; }

            [JsonPropertyName("IsReconnectable")]
            public bool IsReconnectable { get; set; }

            [JsonPropertyName("PostGameDetails")]
            public object PostGameDetails { get; set; }

            [JsonPropertyName("Players")]
            public List<Player> Players { get; set; }

            [JsonPropertyName("MatchmakingData")]
            public MatchmakingData MatchmakingData { get; set; }
        }
    }
}
