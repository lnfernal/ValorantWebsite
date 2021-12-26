namespace ValorantManager.JsonModels
{
    public class Titles
    {
        public class Rootobject
        {
            public int status { get; set; }
            public List<Datum> data { get; set; }
        }

        public class Datum
        {
            public string uuid { get; set; }
            public string displayName { get; set; }
            public string titleText { get; set; }
            public bool isHiddenIfNotOwned { get; set; }
            public string assetPath { get; set; }
        }
    }
}
