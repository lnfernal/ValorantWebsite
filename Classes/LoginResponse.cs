namespace ValorantManager.Classes
{
    public class LoginResponse//Sent after PUT request on api/v1/authorization
    {
        public class Rootobject
        {
            public string type { get; set; }
            public Response response { get; set; }
            public string country { get; set; }
        }

        public class Response
        {
            public string mode { get; set; }
            public Parameters parameters { get; set; }
        }

        public class Parameters
        {
            public string uri { get; set; }
        }
    }
}
