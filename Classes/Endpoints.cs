namespace ValorantManager
{
    public static class Endpoints
    {
            public const string BaseAuth = "https://auth.riotgames.com/";

            public const string Auth = "/api/v1/authorization";

            public const string Region = "https://riot-geo.pas.si.riotgames.com/pas/v1/product/valorant";
            public const string Entitlement = "https://entitlements.auth.riotgames.com/api/token/v1";



            public static string PlayerData(Classes.Region.RiotServer s) => $"https://pd.{s.shard}.a.pvp.net";
            public static string CoreGame(Classes.Region.RiotServer s) => $"https://glz-{s.region}-1.{s.shard}.a.pvp.net";
            public static string Shared(Classes.Region.RiotServer s) => $"https://shared.{s.shard}.a.pvp.net";

    }
}
