namespace ValorantManager.JsonModels
{
    public class FetchCompetitiveUpdates
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Match
        {
            public string MatchID { get; set; }
            public string MapID { get; set; }
            public string SeasonID { get; set; }
            public long MatchStartTime { get; set; }
            public int TierAfterUpdate { get; set; }
            public int TierBeforeUpdate { get; set; }
            public int RankedRatingAfterUpdate { get; set; }
            public int RankedRatingBeforeUpdate { get; set; }
            public int RankedRatingEarned { get; set; }
            public int RankedRatingPerformanceBonus { get; set; }
            public string CompetitiveMovement { get; set; }
            public int AFKPenalty { get; set; }
        }

        public class Root
        {
            public int Version { get; set; }
            public string Subject { get; set; }
            public List<Match> Matches { get; set; }
        }
    }
}
