namespace ValorantManager.Classes
{
    public class RiotGeo
    {
            public class Rootobject
            {
                public string token { get; set; }
                public Affinities affinities { get; set; }
            }

            public class Affinities
            {
                public string pbe { get; set; }
                public string live { get; set; }
            }
        }
}
