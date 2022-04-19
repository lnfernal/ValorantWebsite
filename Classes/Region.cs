namespace ValorantManager.Classes
{
    public static class Region
    {
        public class RiotServer
        {
            public string region { get; set; }
            public string shard { get; set; }
        }
        public static class RegionList
        {
            public static RiotServer AsiaPacific = new RiotServer() { region = "ap", shard = "ap" };
            public static RiotServer Brazil = new RiotServer() { region = "br", shard = "na" };
            public static RiotServer Europe = new RiotServer() { region = "eu", shard = "eu" };
            public static RiotServer Korea = new RiotServer() { region = "kr", shard = "kr" };
            public static RiotServer LatinAmerica = new RiotServer() { region = "latam", shard = "na" };
            public static RiotServer NorthAmerica = new RiotServer() { region = "na", shard = "na" };
        }
    }
}
