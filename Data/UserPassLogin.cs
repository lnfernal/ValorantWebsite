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
        public static string Login(string username, string password)
        {
            CookieContainer cookie = new CookieContainer();
            GetAuthorization(cookie);
            var authJson = JsonSerializer.Deserialize<JsonElement>(Authenticate(cookie, username, password));
            string authURL = authJson.GetProperty("response").GetProperty("parameters").GetProperty("uri").GetString();
            return Regex.Match(authURL, @"access_token=(.+?)&scope=").Groups[1].Value;
        }



        public static void GetAuthorization(CookieContainer jar)
        {
            string url = "https://auth.riotgames.com/api/v1/authorization";
            RestClient client = new RestClient(url);

            client.CookieContainer = jar;

            RestRequest request = new RestRequest(Method.POST);
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
