using RestSharp;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ValorantManager.Data
{

    //https://github.com/RumbleMike/ValorantAuth/blob/master/Program.cs
    public static class UserPassLogin
    {
        public static JsonElement MultifactorCode(string code, CookieContainer cookie)
        {
            var client = new RestClient("https://auth.riotgames.com/api/v1/authorization");
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            client.CookieContainer = cookie;
            request.AddHeader("Content-Type", "application/json");
            var body = "{\"type\":\"multifactor\",\"code\":\"" + code + "\",\"rememberDevice\":false}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return JsonSerializer.Deserialize<JsonElement>(response.Content);
        }


        public static void GetAuthorization(CookieContainer jar)
        {
            string url = "https://auth.riotgames.com/api/v1/authorization";
            RestClient client = new RestClient(url);

            client.CookieContainer = jar;

            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.99 Safari/537.36");
            string body = "{\"client_id\":\"play-valorant-web-prod\",\"nonce\":\"1\",\"redirect_uri\":\"https://playvalorant.com/opt_in" + "\",\"response_type\":\"token id_token\",\"scope\":\"account openid\"}";
            request.AddJsonBody(body);
            client.Execute(request);
        }

        public static string Authenticate(CookieContainer cookie, string user, string pass)
        {
            string url = "https://auth.riotgames.com/api/v1/authorization";
            RestClient client = new RestClient(url);

            client.CookieContainer = cookie;

            RestRequest request = new RestRequest(Method.PUT);
            string body = "{\"type\":\"auth\",\"username\":\"" + user + "\",\"password\":\"" + pass + "\"}";
            request.AddJsonBody(body);

            return client.Execute(request).Content;
        }
    }
}
