using System.Net;
using ValorantManager.Data;
using ValorantManager.JsonModels;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;
using ValorantManager.Util;
namespace ValorantManager.Services
{
    public class ValorantService
    {
        private const string RiotEntitlementHeader = "X-Riot-Entitlements-JWT";
        private const string RiotPlatformHeader = "X-Riot-ClientPlatform";
        private const string RiotPlatformHeaderValue = "eyJwbGF0Zm9ybVR5cGUiOiJQQyIsInBsYXRmb3JtT1MiOiJXaW5kb3dzIiwicGxhdGZvcm1PU1ZlcnNpb24iOiIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwicGxhdGZvcm1DaGlwc2V0IjoiVW5rbm93biJ9";
        private const string AuthorizationHeader = "Authorization";

        public event Action OnChange;
        private void LoginStateChanged() => OnChange.Invoke();
        public User user { get; set; } = new User() { loginState = LoginState.LoggedOut };

        public void Login()
        {
            var LoginRequest = new HttpRequestMessage(HttpMethod.Get, "https://auth.riotgames.com/userinfo");
            LoginRequest.Headers.Add(AuthorizationHeader, $"Bearer {user.Token}");
            HttpClient client = new HttpClient();
            var LoginResponse = client.Send(LoginRequest);
            LoginRequest.Dispose();
            if (LoginResponse.IsSuccessStatusCode)
            {
                using var responseStream = LoginResponse.Content.ReadAsStream();
                UserInfo.Root userobj = JsonSerializer.Deserialize<UserInfo.Root>(responseStream);
                user.Name = $"{userobj.Acct.GameName}#{userobj.Acct.TagLine}";
                user.puuid = userobj.Sub;
                user.CreationDate = userobj.Acct.CreatedAt;

                var EntitlementRequest = new HttpRequestMessage(HttpMethod.Post, "https://entitlements.auth.riotgames.com/api/token/v1");
                EntitlementRequest.Headers.Add(AuthorizationHeader, $"Bearer {user.Token}");
                EntitlementRequest.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");//CONTENT-TYPE header
                var EntitlementResponse = client.Send(EntitlementRequest);

                if (EntitlementResponse.IsSuccessStatusCode)
                {
                    using var EnresponseStream = EntitlementResponse.Content.ReadAsStream();
                    user.Entitlement = JsonSerializer.Deserialize<JsonElement>(EnresponseStream).GetProperty("entitlements_token").GetString();

                    if (user.cookie_region == Regions.Auto && user.region == Regions.Auto)
                    {
                        //No region stored
                        user.region = GetUserRegion(user);
                        user.cookie_region = user.region;
                    }
                    else
                    {
                        //Region stored from cookies
                        user.region = user.cookie_region;
                    }

                    user.loginState = LoginState.LoggedIn;

                }
                else
                    user.loginState = LoginState.WrongLogin;
            }
            else
                user.loginState = LoginState.WrongLogin;

            LoginStateChanged();
        }

        public void Logout()
        {
            user = new();
            LoginStateChanged();
        }

        public Regions GetUserRegion(User user)
        {
            using (WebClient wc = new())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);

                foreach (Regions region in Enum.GetValues(typeof(Regions)))
                {
                    if (region == Regions.Auto)
                        continue;

                    string CheckRegion = wc.DownloadString($"https://pd.{region}.a.pvp.net/store/v1/entitlements/{user.puuid}");
                    if (CheckRegion.Length > 335)
                        return region;
                }
                return Regions.na;//Default
            }
        }


        public Loadout.Root GetLoadout()
        {
            //https://pd.{region}.a.pvp.net/personalization/v2/players/{puuid}/playerloadout

            using (WebClient wc = new())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);

                return JsonSerializer.Deserialize<Loadout.Root>(wc.DownloadString($"https://pd.{user.region}.a.pvp.net/personalization/v2/players/{user.puuid}/playerloadout"));
            }
        }

        public bool SetPlayerLoadout(Loadout.Root loadout)
        {
            try
            {
                using (WebClient wc = new())
                {
                    wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                    wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                    string url = $"https://pd.{user.region}.a.pvp.net/personalization/v2/players/{user.puuid}/playerloadout";
                    string response = wc.UploadString(url, "PUT", JsonSerializer.Serialize(loadout));
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static News.Root News_Instance { get; set; } = null;
        public News.Root News
        {
            get
            {
                if (News_Instance == null)
                    News_Instance = Get_News();


                return News_Instance;
            }
            set { News_Instance = value; }
        }

        public News.Root Get_News() => JsonSerializer.Deserialize<News.Root>(new WebClient().DownloadString("https://playvalorant.com/page-data/en-us/news/page-data.json"));


        public static CompetitiveTiers.Root Tiers_Instance { get; set; } = null;
        public CompetitiveTiers.Root CompTiers
        {
            get
            {
                if (Tiers_Instance == null)
                    Tiers_Instance = Get_CompTiers();


                return Tiers_Instance;
            }
            set { Tiers_Instance = value; }
        }

        public CompetitiveTiers.Root Get_CompTiers() => JsonSerializer.Deserialize<CompetitiveTiers.Root>("{\"status\":200,\"data\":[{\"uuid\":\"564d8e28-c226-3180-6285-e48a390db8b1\",\"assetObjectName\":\"Episode1_CompetitiveTierDataTable\",\"tiers\":[{\"tier\":0,\"tierName\":\"UNRANKED\",\"division\":\"ECompetitiveDivision::UNRANKED\",\"divisionName\":\"UNRANKED\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/0/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/0/largeicon.png\",\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":1,\"tierName\":\"Unused1\",\"division\":\"ECompetitiveDivision::INVALID\",\"divisionName\":\"Unused1\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":null,\"largeIcon\":null,\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":2,\"tierName\":\"Unused2\",\"division\":\"ECompetitiveDivision::INVALID\",\"divisionName\":\"Unused2\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":null,\"largeIcon\":null,\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":3,\"tierName\":\"IRON 1\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/3/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/3/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/3/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/3/ranktriangleupicon.png\"},{\"tier\":4,\"tierName\":\"IRON 2\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/4/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/4/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/4/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/4/ranktriangleupicon.png\"},{\"tier\":5,\"tierName\":\"IRON 3\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/5/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/5/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/5/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/5/ranktriangleupicon.png\"},{\"tier\":6,\"tierName\":\"BRONZE 1\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/6/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/6/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/6/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/6/ranktriangleupicon.png\"},{\"tier\":7,\"tierName\":\"BRONZE 2\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/7/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/7/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/7/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/7/ranktriangleupicon.png\"},{\"tier\":8,\"tierName\":\"BRONZE 3\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/8/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/8/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/8/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/8/ranktriangleupicon.png\"},{\"tier\":9,\"tierName\":\"SILVER 1\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/9/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/9/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/9/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/9/ranktriangleupicon.png\"},{\"tier\":10,\"tierName\":\"SILVER 2\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/10/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/10/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/10/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/10/ranktriangleupicon.png\"},{\"tier\":11,\"tierName\":\"SILVER 3\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/11/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/11/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/11/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/11/ranktriangleupicon.png\"},{\"tier\":12,\"tierName\":\"GOLD 1\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/12/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/12/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/12/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/12/ranktriangleupicon.png\"},{\"tier\":13,\"tierName\":\"GOLD 2\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/13/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/13/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/13/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/13/ranktriangleupicon.png\"},{\"tier\":14,\"tierName\":\"GOLD 3\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/14/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/14/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/14/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/14/ranktriangleupicon.png\"},{\"tier\":15,\"tierName\":\"PLATINUM 1\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/15/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/15/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/15/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/15/ranktriangleupicon.png\"},{\"tier\":16,\"tierName\":\"PLATINUM 2\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/16/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/16/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/16/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/16/ranktriangleupicon.png\"},{\"tier\":17,\"tierName\":\"PLATINUM 3\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/17/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/17/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/17/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/17/ranktriangleupicon.png\"},{\"tier\":18,\"tierName\":\"DIAMOND 1\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/18/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/18/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/18/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/18/ranktriangleupicon.png\"},{\"tier\":19,\"tierName\":\"DIAMOND 2\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/19/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/19/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/19/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/19/ranktriangleupicon.png\"},{\"tier\":20,\"tierName\":\"DIAMOND 3\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/20/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/20/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/20/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/20/ranktriangleupicon.png\"},{\"tier\":21,\"tierName\":\"IMMORTAL 1\",\"division\":\"ECompetitiveDivision::IMMORTAL\",\"divisionName\":\"IMMORTAL\",\"color\":\"bb3d65ff\",\"backgroundColor\":\"ff5551ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/21/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/21/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/21/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/21/ranktriangleupicon.png\"},{\"tier\":22,\"tierName\":\"IMMORTAL 2\",\"division\":\"ECompetitiveDivision::IMMORTAL\",\"divisionName\":\"IMMORTAL\",\"color\":\"bb3d65ff\",\"backgroundColor\":\"ff5551ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/22/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/22/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/22/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/22/ranktriangleupicon.png\"},{\"tier\":23,\"tierName\":\"IMMORTAL 3\",\"division\":\"ECompetitiveDivision::IMMORTAL\",\"divisionName\":\"IMMORTAL\",\"color\":\"bb3d65ff\",\"backgroundColor\":\"ff5551ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/23/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/23/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/23/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/23/ranktriangleupicon.png\"},{\"tier\":24,\"tierName\":\"RADIANT\",\"division\":\"ECompetitiveDivision::RADIANT\",\"divisionName\":\"RADIANT\",\"color\":\"ffffaaff\",\"backgroundColor\":\"ffedaaff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/24/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/24/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/24/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/24/ranktriangleupicon.png\"}],\"assetPath\":\"ShooterGame/Content/UI/Screens/Shared/Competitive/Episode1_CompetitiveTierDataTable\"},{\"uuid\":\"23eb970e-6408-bc0b-3f20-d8fb0e0354ea\",\"assetObjectName\":\"Episode2_CompetitiveTierDataTable\",\"tiers\":[{\"tier\":0,\"tierName\":\"UNRANKED\",\"division\":\"ECompetitiveDivision::UNRANKED\",\"divisionName\":\"UNRANKED\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/0/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/0/largeicon.png\",\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":1,\"tierName\":\"Unused1\",\"division\":\"ECompetitiveDivision::INVALID\",\"divisionName\":\"Unused1\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":null,\"largeIcon\":null,\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":2,\"tierName\":\"Unused2\",\"division\":\"ECompetitiveDivision::INVALID\",\"divisionName\":\"Unused2\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":null,\"largeIcon\":null,\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":3,\"tierName\":\"IRON 1\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/3/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/3/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/3/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/3/ranktriangleupicon.png\"},{\"tier\":4,\"tierName\":\"IRON 2\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/4/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/4/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/4/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/4/ranktriangleupicon.png\"},{\"tier\":5,\"tierName\":\"IRON 3\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/5/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/5/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/5/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/5/ranktriangleupicon.png\"},{\"tier\":6,\"tierName\":\"BRONZE 1\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/6/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/6/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/6/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/6/ranktriangleupicon.png\"},{\"tier\":7,\"tierName\":\"BRONZE 2\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/7/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/7/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/7/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/7/ranktriangleupicon.png\"},{\"tier\":8,\"tierName\":\"BRONZE 3\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/8/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/8/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/8/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/8/ranktriangleupicon.png\"},{\"tier\":9,\"tierName\":\"SILVER 1\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/9/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/9/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/9/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/9/ranktriangleupicon.png\"},{\"tier\":10,\"tierName\":\"SILVER 2\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/10/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/10/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/10/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/10/ranktriangleupicon.png\"},{\"tier\":11,\"tierName\":\"SILVER 3\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/11/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/11/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/11/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/11/ranktriangleupicon.png\"},{\"tier\":12,\"tierName\":\"GOLD 1\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/12/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/12/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/12/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/12/ranktriangleupicon.png\"},{\"tier\":13,\"tierName\":\"GOLD 2\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/13/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/13/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/13/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/13/ranktriangleupicon.png\"},{\"tier\":14,\"tierName\":\"GOLD 3\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/14/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/14/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/14/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/14/ranktriangleupicon.png\"},{\"tier\":15,\"tierName\":\"PLATINUM 1\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/15/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/15/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/15/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/15/ranktriangleupicon.png\"},{\"tier\":16,\"tierName\":\"PLATINUM 2\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/16/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/16/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/16/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/16/ranktriangleupicon.png\"},{\"tier\":17,\"tierName\":\"PLATINUM 3\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/17/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/17/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/17/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/17/ranktriangleupicon.png\"},{\"tier\":18,\"tierName\":\"DIAMOND 1\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/18/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/18/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/18/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/18/ranktriangleupicon.png\"},{\"tier\":19,\"tierName\":\"DIAMOND 2\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/19/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/19/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/19/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/19/ranktriangleupicon.png\"},{\"tier\":20,\"tierName\":\"DIAMOND 3\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/20/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/20/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/20/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/20/ranktriangleupicon.png\"},{\"tier\":21,\"tierName\":\"IMMORTAL\",\"division\":\"ECompetitiveDivision::IMMORTAL\",\"divisionName\":\"IMMORTAL\",\"color\":\"bb3d65ff\",\"backgroundColor\":\"ff5551ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/21/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/21/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/21/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/21/ranktriangleupicon.png\"},{\"tier\":24,\"tierName\":\"RADIANT\",\"division\":\"ECompetitiveDivision::RADIANT\",\"divisionName\":\"RADIANT\",\"color\":\"ffffaaff\",\"backgroundColor\":\"ffedaaff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/24/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/24/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/24/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/23eb970e-6408-bc0b-3f20-d8fb0e0354ea/24/ranktriangleupicon.png\"}],\"assetPath\":\"ShooterGame/Content/UI/Screens/Shared/Competitive/Episode2_CompetitiveTierDataTable\"},{\"uuid\":\"edb72a72-7e6d-6010-9591-7c053bbdbf48\",\"assetObjectName\":\"Episode3_CompetitiveTierDataTable\",\"tiers\":[{\"tier\":0,\"tierName\":\"UNRANKED\",\"division\":\"ECompetitiveDivision::UNRANKED\",\"divisionName\":\"UNRANKED\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/0/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/0/largeicon.png\",\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":1,\"tierName\":\"Unused1\",\"division\":\"ECompetitiveDivision::INVALID\",\"divisionName\":\"Unused1\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":null,\"largeIcon\":null,\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":2,\"tierName\":\"Unused2\",\"division\":\"ECompetitiveDivision::INVALID\",\"divisionName\":\"Unused2\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":null,\"largeIcon\":null,\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":3,\"tierName\":\"IRON 1\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/3/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/3/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/3/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/3/ranktriangleupicon.png\"},{\"tier\":4,\"tierName\":\"IRON 2\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/4/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/4/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/4/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/4/ranktriangleupicon.png\"},{\"tier\":5,\"tierName\":\"IRON 3\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/5/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/5/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/5/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/5/ranktriangleupicon.png\"},{\"tier\":6,\"tierName\":\"BRONZE 1\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/6/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/6/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/6/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/6/ranktriangleupicon.png\"},{\"tier\":7,\"tierName\":\"BRONZE 2\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/7/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/7/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/7/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/7/ranktriangleupicon.png\"},{\"tier\":8,\"tierName\":\"BRONZE 3\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/8/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/8/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/8/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/8/ranktriangleupicon.png\"},{\"tier\":9,\"tierName\":\"SILVER 1\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/9/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/9/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/9/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/9/ranktriangleupicon.png\"},{\"tier\":10,\"tierName\":\"SILVER 2\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/10/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/10/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/10/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/10/ranktriangleupicon.png\"},{\"tier\":11,\"tierName\":\"SILVER 3\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/11/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/11/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/11/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/11/ranktriangleupicon.png\"},{\"tier\":12,\"tierName\":\"GOLD 1\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/12/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/12/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/12/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/12/ranktriangleupicon.png\"},{\"tier\":13,\"tierName\":\"GOLD 2\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/13/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/13/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/13/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/13/ranktriangleupicon.png\"},{\"tier\":14,\"tierName\":\"GOLD 3\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/14/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/14/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/14/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/14/ranktriangleupicon.png\"},{\"tier\":15,\"tierName\":\"PLATINUM 1\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/15/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/15/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/15/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/15/ranktriangleupicon.png\"},{\"tier\":16,\"tierName\":\"PLATINUM 2\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/16/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/16/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/16/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/16/ranktriangleupicon.png\"},{\"tier\":17,\"tierName\":\"PLATINUM 3\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/17/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/17/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/17/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/17/ranktriangleupicon.png\"},{\"tier\":18,\"tierName\":\"DIAMOND 1\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/18/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/18/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/18/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/18/ranktriangleupicon.png\"},{\"tier\":19,\"tierName\":\"DIAMOND 2\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/19/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/19/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/19/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/19/ranktriangleupicon.png\"},{\"tier\":20,\"tierName\":\"DIAMOND 3\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/20/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/20/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/20/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/20/ranktriangleupicon.png\"},{\"tier\":21,\"tierName\":\"IMMORTAL 1\",\"division\":\"ECompetitiveDivision::IMMORTAL\",\"divisionName\":\"IMMORTAL\",\"color\":\"bb3d65ff\",\"backgroundColor\":\"ff5551ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/21/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/21/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/21/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/21/ranktriangleupicon.png\"},{\"tier\":22,\"tierName\":\"IMMORTAL 2\",\"division\":\"ECompetitiveDivision::IMMORTAL\",\"divisionName\":\"IMMORTAL\",\"color\":\"bb3d65ff\",\"backgroundColor\":\"ff5551ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/22/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/22/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/22/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/22/ranktriangleupicon.png\"},{\"tier\":23,\"tierName\":\"IMMORTAL 3\",\"division\":\"ECompetitiveDivision::IMMORTAL\",\"divisionName\":\"IMMORTAL\",\"color\":\"bb3d65ff\",\"backgroundColor\":\"ff5551ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/23/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/23/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/23/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/23/ranktriangleupicon.png\"},{\"tier\":24,\"tierName\":\"RADIANT\",\"division\":\"ECompetitiveDivision::RADIANT\",\"divisionName\":\"RADIANT\",\"color\":\"ffffaaff\",\"backgroundColor\":\"ffedaaff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/24/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/24/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/24/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/edb72a72-7e6d-6010-9591-7c053bbdbf48/24/ranktriangleupicon.png\"}],\"assetPath\":\"ShooterGame/Content/UI/Screens/Shared/Competitive/Episode3_CompetitiveTierDataTable\"},{\"uuid\":\"e4e9a692-288f-63ca-7835-16fbf6234fda\",\"assetObjectName\":\"Episode4_CompetitiveTierDataTable\",\"tiers\":[{\"tier\":0,\"tierName\":\"UNRANKED\",\"division\":\"ECompetitiveDivision::UNRANKED\",\"divisionName\":\"UNRANKED\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/0/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/0/largeicon.png\",\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":1,\"tierName\":\"Unused1\",\"division\":\"ECompetitiveDivision::INVALID\",\"divisionName\":\"Unused1\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":null,\"largeIcon\":null,\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":2,\"tierName\":\"Unused2\",\"division\":\"ECompetitiveDivision::INVALID\",\"divisionName\":\"Unused2\",\"color\":\"ffffffff\",\"backgroundColor\":\"00000000\",\"smallIcon\":null,\"largeIcon\":null,\"rankTriangleDownIcon\":null,\"rankTriangleUpIcon\":null},{\"tier\":3,\"tierName\":\"IRON 1\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/3/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/3/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/3/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/3/ranktriangleupicon.png\"},{\"tier\":4,\"tierName\":\"IRON 2\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/4/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/4/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/4/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/4/ranktriangleupicon.png\"},{\"tier\":5,\"tierName\":\"IRON 3\",\"division\":\"ECompetitiveDivision::IRON\",\"divisionName\":\"IRON\",\"color\":\"4f514fff\",\"backgroundColor\":\"828282ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/5/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/5/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/5/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/5/ranktriangleupicon.png\"},{\"tier\":6,\"tierName\":\"BRONZE 1\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/6/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/6/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/6/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/6/ranktriangleupicon.png\"},{\"tier\":7,\"tierName\":\"BRONZE 2\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/7/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/7/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/7/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/7/ranktriangleupicon.png\"},{\"tier\":8,\"tierName\":\"BRONZE 3\",\"division\":\"ECompetitiveDivision::BRONZE\",\"divisionName\":\"BRONZE\",\"color\":\"a5855dff\",\"backgroundColor\":\"7c5522ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/8/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/8/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/8/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/8/ranktriangleupicon.png\"},{\"tier\":9,\"tierName\":\"SILVER 1\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/9/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/9/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/9/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/9/ranktriangleupicon.png\"},{\"tier\":10,\"tierName\":\"SILVER 2\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/10/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/10/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/10/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/10/ranktriangleupicon.png\"},{\"tier\":11,\"tierName\":\"SILVER 3\",\"division\":\"ECompetitiveDivision::SILVER\",\"divisionName\":\"SILVER\",\"color\":\"bbc2c2ff\",\"backgroundColor\":\"d1d1d1ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/11/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/11/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/11/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/11/ranktriangleupicon.png\"},{\"tier\":12,\"tierName\":\"GOLD 1\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/12/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/12/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/12/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/12/ranktriangleupicon.png\"},{\"tier\":13,\"tierName\":\"GOLD 2\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/13/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/13/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/13/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/13/ranktriangleupicon.png\"},{\"tier\":14,\"tierName\":\"GOLD 3\",\"division\":\"ECompetitiveDivision::GOLD\",\"divisionName\":\"GOLD\",\"color\":\"eccf56ff\",\"backgroundColor\":\"eec56aff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/14/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/14/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/14/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/14/ranktriangleupicon.png\"},{\"tier\":15,\"tierName\":\"PLATINUM 1\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/15/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/15/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/15/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/15/ranktriangleupicon.png\"},{\"tier\":16,\"tierName\":\"PLATINUM 2\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/16/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/16/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/16/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/16/ranktriangleupicon.png\"},{\"tier\":17,\"tierName\":\"PLATINUM 3\",\"division\":\"ECompetitiveDivision::PLATINUM\",\"divisionName\":\"PLATINUM\",\"color\":\"59a9b6ff\",\"backgroundColor\":\"00c7c0ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/17/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/17/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/17/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/17/ranktriangleupicon.png\"},{\"tier\":18,\"tierName\":\"DIAMOND 1\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/18/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/18/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/18/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/18/ranktriangleupicon.png\"},{\"tier\":19,\"tierName\":\"DIAMOND 2\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/19/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/19/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/19/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/19/ranktriangleupicon.png\"},{\"tier\":20,\"tierName\":\"DIAMOND 3\",\"division\":\"ECompetitiveDivision::DIAMOND\",\"divisionName\":\"DIAMOND\",\"color\":\"b489c4ff\",\"backgroundColor\":\"763bafff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/20/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/20/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/20/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/20/ranktriangleupicon.png\"},{\"tier\":21,\"tierName\":\"IMMORTAL 1\",\"division\":\"ECompetitiveDivision::IMMORTAL\",\"divisionName\":\"IMMORTAL\",\"color\":\"bb3d65ff\",\"backgroundColor\":\"ff5551ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/21/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/21/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/21/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/21/ranktriangleupicon.png\"},{\"tier\":22,\"tierName\":\"IMMORTAL 2\",\"division\":\"ECompetitiveDivision::IMMORTAL\",\"divisionName\":\"IMMORTAL\",\"color\":\"bb3d65ff\",\"backgroundColor\":\"ff5551ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/22/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/22/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/22/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/22/ranktriangleupicon.png\"},{\"tier\":23,\"tierName\":\"IMMORTAL 3\",\"division\":\"ECompetitiveDivision::IMMORTAL\",\"divisionName\":\"IMMORTAL\",\"color\":\"bb3d65ff\",\"backgroundColor\":\"ff5551ff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/23/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/23/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/23/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/23/ranktriangleupicon.png\"},{\"tier\":24,\"tierName\":\"RADIANT\",\"division\":\"ECompetitiveDivision::RADIANT\",\"divisionName\":\"RADIANT\",\"color\":\"ffffaaff\",\"backgroundColor\":\"ffedaaff\",\"smallIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/24/smallicon.png\",\"largeIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/24/largeicon.png\",\"rankTriangleDownIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/24/ranktriangledownicon.png\",\"rankTriangleUpIcon\":\"https://media.valorant-api.com/competitivetiers/e4e9a692-288f-63ca-7835-16fbf6234fda/24/ranktriangleupicon.png\"}],\"assetPath\":\"ShooterGame/Content/UI/Screens/Shared/Competitive/Episode4_CompetitiveTierDataTable\"}]}");


        public static Entitlements.Rootobject Entitlements_Instance { get; set; } = null;
        public Entitlements.Rootobject Entitlements
        {
            get
            {
                if (Entitlements_Instance == null)
                    Entitlements_Instance = Get_Entitlements();


                return Entitlements_Instance;
            }
            set { Entitlements_Instance = value; }
        }
        public Entitlements.Rootobject Get_Entitlements()
        {
            using (WebClient wc = new())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                return JsonSerializer.Deserialize<Entitlements.Rootobject>(wc.DownloadString($"https://pd.{user.region}.a.pvp.net/store/v1/entitlements/{user.puuid}"));
            }
        }

        public StoreFrontV2.Root Store_GetStorefrontV2()
        {
            using (WebClient wc = new())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                return JsonSerializer.Deserialize<StoreFrontV2.Root>(wc.DownloadString($"https://pd.{user.region}.a.pvp.net/store/v2/storefront/{user.puuid}"));
            }
        }

        public static Buddies.Root Buddies_Instance { get; set; } = null;
        public Buddies.Root Buddies
        {
            get
            {
                if (Buddies_Instance == null)
                    Buddies_Instance = GetBuddies();


                return Buddies_Instance;
            }
            set { Buddies = value; }
        }

        public Buddies.Root GetBuddies()
        {
            var list = GetValData<Buddies.Root>("/buddies");
            list.data = list.data.OrderBy(x => x.displayName).ToList();
            return list;
        }

        public static Agents.Root Agents_Instance { get; set; }
        public Agents.Root Agents
        {
            get
            {
                if (Agents_Instance == null)
                    Agents_Instance = GetAgents();


                return Agents_Instance;
            }
            set { Agents_Instance = value; }
        }

        public Agents.Root GetAgents() => GetValData<Agents.Root>("/agents");


        public static Weapons.Root Weapons_Instance { get; set; } = null;

        public Weapons.Root Weapons
        {
            get
            {
                if (Weapons_Instance == null)
                    Weapons_Instance = GetWeapons();


                return Weapons_Instance;
            }
            set { Weapons = value; }
        }

        public Weapons.Root GetWeapons()
        {
            var list = GetValData<Weapons.Root>("/weapons");
            list.data = list.data.OrderBy(x => x.displayName).ToList();
            return list;
        }

        public static Cards.Root Cards_Instance { get; set; }
        public Cards.Root Cards
        {
            get
            {
                if (Cards_Instance == null)
                    Cards_Instance = GetCards();


                return Cards_Instance;
            }
            set { Cards_Instance = value; }
        }
        public Cards.Root GetCards()
        {
            var list = GetValData<Cards.Root>("/playercards");
            list.Data = list.Data.OrderBy(x => x.DisplayName).ToList();
            return list;
        }

        public static Titles.Rootobject Titles_Instance { get; set; }
        public Titles.Rootobject Titles
        {
            get
            {
                if (Titles_Instance == null)
                    Titles_Instance = GetTitles();


                return Titles_Instance;
            }
            set { Titles_Instance = value; }
        }
        public Titles.Rootobject GetTitles()
        {
            var list = GetValData<Titles.Rootobject>("/playertitles");
            list.data = list.data.OrderBy(x => x.displayName).ToList();
            return list;
        }

        private const string UVALAPIEndpoint = "https://valorant-api.com/v1";
        public static T GetValData<T>(string url)
        {
            return JsonSerializer.Deserialize<T>(new WebClient().DownloadString($"{UVALAPIEndpoint}{url}"));
        }


        public Wallet.Rootobject Store_GetWallet()//Get the currently available items in the store
        {
            using (WebClient wc = new())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                string url = $"https://pd.{user.region}.a.pvp.net/store/v1/wallet/{user.puuid}";
                return JsonSerializer.Deserialize<Wallet.Rootobject>(wc.DownloadString(url));
            }
        }

        public string GetAccountCreationDate()
        {
            DateTimeOffset accountCreateDate = DateTimeOffset.FromUnixTimeMilliseconds(user.CreationDate);
            return DateDiffInWords(DateTime.UtcNow, accountCreateDate.DateTime);
        }

        public static string DateDiffInWords(DateTime dateTimeBegin, DateTime dateTimeEnd)
        {
            TimeSpan diff = dateTimeBegin.Subtract(dateTimeEnd);
            if (diff.Days > 10)
            {
                string s = string.Format("{0} {1} at {2}"
                    , dateTimeEnd.ToString("MMM dd")
                    , dateTimeEnd.ToString("yyyy")
                    , dateTimeEnd.ToString("HH:mm"));
                return s;
            }
            if (diff.Days > 1)
                return string.Format("{0} days ago", Convert.ToInt32(diff.Days));
            if (diff.Days == 1)
                return "Yesterday";
            if (diff.Hours > 1)
                return string.Format("{0} hours ago", Convert.ToInt32(diff.Hours));
            if (diff.Minutes > 1)
                return string.Format("{0} minutes ago", Convert.ToInt32(diff.Minutes));
            if (diff.Seconds > 1)
                return string.Format("{0} seconds ago", Convert.ToInt32(diff.Seconds));

            //we should not reach here
            return string.Empty;
        }


        //https://github.com/techchrism/valorant-api-docs/blob/trunk/docs/Store/GET%20Store_GetEntitlements.md
        /// <summary>
        /// Get an unused buddy from the user's entitlements
        /// </summary>
        /// <param name="loadout">The loadout to compare the buddy to</param>
        /// <param name="targetBuddy">The buddy the user should set, if the user does not own it it will return a random unused buddy, if the user owns it, it will return the corresponding buddy</param>
        /// <returns></returns>
        public Entitlements.Entitlement GetUnusedBuddy(Loadout.Root loadout, Buddies.Datum targetBuddy)
        {
            var buddyEntitlements = Entitlements.EntitlementsByTypes.First(x => x.ItemTypeID.Equals("dd3bf334-87f3-40bd-b043-682a57a8dc3a")).Entitlements.ToList();

            List<string> UsingBuddies = new List<string>();

            foreach (var item in loadout.Guns)
            {
                if (item.CharmInstanceID != null)
                    UsingBuddies.Add(item.CharmInstanceID);
            }

            foreach (var buddy in UsingBuddies)
                buddyEntitlements.RemoveAll(x => x.InstanceID.Equals(buddy));

            if (buddyEntitlements.Count == 0)
                return null;

            foreach (var legitBuddy in buddyEntitlements)
            {
                if (targetBuddy.levels[0].uuid == legitBuddy.ItemID)
                    return legitBuddy;
            }

            return buddyEntitlements.First();
        }

        public Buddies.Datum BuddyLevelToBuddyObj(string BuddyLevel) => Buddies.data.First(x => x.levels[0].uuid.Equals(BuddyLevel));
        public Buddies.Datum BuddyIDToBuddyObj(string BuddyID) => Buddies.data.First(x => x.uuid.Equals(BuddyID));



        public CoreGame_FetchPlayer.Root CoreGame_FetchPlayer()
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);

                string url = $"https://glz-{user.region}-1.{user.region}.a.pvp.net/core-game/v1/players/{user.puuid}";
                try
                {
                    return JsonSerializer.Deserialize<CoreGame_FetchPlayer.Root>(wc.DownloadString(url));
                }
                catch
                {
                    return null;
                }
            }
        }

        public CoreGame_FetchMatch.Root CoreGame_FetchMatch(string MatchID)
        {
            using (WebClient wc = new())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                string url = $"https://glz-{user.region}-1.{user.region}.a.pvp.net/core-game/v1/matches/{MatchID}";
                try
                {
                    return JsonSerializer.Deserialize<CoreGame_FetchMatch.Root>(wc.DownloadString(url));
                }
                catch
                {
                    return null;
                }
            }
        }

        public CoreGame_FetchMatch.Root CoreGame_FetchMatchLoadouts(string MatchID)
        {
            using (WebClient wc = new())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                string url = $"https://glz-{user.region}-1.{user.region}.a.pvp.net/core-game/v1/matches/{MatchID}/loadouts";
                try
                {
                    return JsonSerializer.Deserialize<CoreGame_FetchMatch.Root>(wc.DownloadString(url));
                }
                catch
                {
                    return null;
                }
            }
        }

        public CoreGame_FetchPlayer.Root Pregame_GetPlayer()
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                string url = $"https://glz-{user.region}-1.{user.region}.a.pvp.net/pregame/v1/players/{user.puuid}";
                try
                {
                    return JsonSerializer.Deserialize<CoreGame_FetchPlayer.Root>(wc.DownloadString(url));
                }
                catch
                {
                    return null;
                }
            }
        }
        public bool? Pregame_SelectCharacter(string pregameID, string agentID)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                    wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                    string url = $"https://glz-{user.region}-1.{user.region}.a.pvp.net/pregame/v1/matches/{pregameID}/select/{agentID}";
                    string response = wc.UploadString(url, "POST", "");
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool? Pregame_LockCharacter(string pregameID, string agentID)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                    wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                    string url = $"https://glz-{user.region}-1.{user.region}.a.pvp.net/pregame/v1/matches/{pregameID}/lock/{agentID}";
                    string response = wc.UploadString(url, "POST", "");
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        public Dictionary<string, string> IDToUsername(string[] puuidArray)
        {
            using (WebClient wc = new())
            {
                //No auth required for name service
                string url = $"https://pd.{user.region}.a.pvp.net/name-service/v2/players";

                string req = wc.UploadString(url, "PUT", JsonSerializer.Serialize(puuidArray));

                var response = JsonSerializer.Deserialize<NameService.Root[]>(req);

                Dictionary<string, string> result = new Dictionary<string, string>();

                foreach (var item in response)
                {
                    result.Add(item.Subject, $"{item.GameName}#{item.TagLine}");
                }

                return result;
            }
        }

        public bool CoreGame_DisassociatePlayer(string matchID)
        {
            try
            {
                using (WebClient wc = new())
                {
                    wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                    wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                    string url = $"https://glz-{user.region}-1.{user.region}.a.pvp.net/core-game/v1/players/{user.puuid}/disassociate/{matchID}";
                    string response = wc.UploadString(url, "POST", "");
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public FetchCompetitiveUpdates.Root MMR_FetchCompetitiveUpdates(string puuid)
        {
            using (WebClient wc = new())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                wc.Headers.Add(RiotPlatformHeader, RiotPlatformHeaderValue);
                string url = $"https://pd.{user.region}.a.pvp.net/mmr/v1/players/{puuid}/competitiveupdates?queue=competitive&endIndex=3&startIndex=0";
                return JsonSerializer.Deserialize<FetchCompetitiveUpdates.Root>(wc.DownloadString(url));
            }
        }

        public static string RiotClientVersion_Instance { get; set; }
        public string RiotClientVersion
        {
            get
            {
                if (RiotClientVersion_Instance == null)
                    RiotClientVersion_Instance = GetClientVersion();


                return RiotClientVersion_Instance;
            }
            set { RiotClientVersion_Instance = value; }
        }

        public string GetClientVersion()
        {
            return JsonSerializer.Deserialize<JsonElement>(new WebClient().DownloadString("https://valorant-api.com/v1/version")).GetProperty("data").GetProperty("riotClientVersion").GetString();
        }


        public class PlayerGameInfo
        {
            public string ID = "";
            public string Name = "";
            public string RankIMGURL = "";
            public string RankName = "";
            public int Elo = 0;

            public string Peak_RankIMGURL = "";
            public string Peak_RankName = "";


            public string VandalSkinIMG = "";
        }


        public List<PlayerGameInfo> GetPlayerInfo(string[] puuidArray)
        {
            List<PlayerGameInfo> info = new();

            var names = IDToUsername(puuidArray);
            foreach (var name in names)
            {
                PlayerGameInfo player = new();
                player.Name = name.Value;
                player.ID = name.Key;
                var matches = MMR_FetchCompetitiveUpdates(name.Key);

                int currentTier = 0, currentRR = 0;

                if (matches.Matches.Count > 0)
                {
                    currentTier = matches.Matches[0].TierAfterUpdate;
                    currentRR = matches.Matches[0].RankedRatingAfterUpdate;
                }


                player.Elo = CalculateElo(currentTier, currentRR);

                player.RankIMGURL = $"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/{currentTier}/smallicon.png";
                player.RankName = CompTiers.Data[0].Tiers.First(x => x.TierNum.Equals(currentTier)).TierName.ToLower().FirstCharToUpper();

                int PeakRank = Get_PeakRank(name.Key);

                player.Peak_RankName = CompTiers.Data[0].Tiers.First(x => x.TierNum.Equals(PeakRank)).TierName.ToLower().FirstCharToUpper();
                player.Peak_RankIMGURL = $"https://media.valorant-api.com/competitivetiers/564d8e28-c226-3180-6285-e48a390db8b1/{PeakRank}/smallicon.png";

                info.Add(player);
            }

            return info;

        }

        public int Get_PeakRank(string puuid)
        {
            using (WebClient wc = new())
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {user.Token}");
                wc.Headers.Add(RiotEntitlementHeader, user.Entitlement);
                wc.Headers.Add(RiotPlatformHeader, RiotPlatformHeaderValue);
                wc.Headers.Add("X-Riot-ClientVersion", RiotClientVersion);


                JsonElement response = JsonSerializer.Deserialize<JsonElement>(wc.DownloadString($"https://pd.{user.region}.a.pvp.net/mmr/v1/players/{puuid}"));

                JsonElement.ObjectEnumerator seasonslist;
                try
                {
                    seasonslist = response.GetProperty("QueueSkills").GetProperty("competitive").GetProperty("SeasonalInfoBySeasonID").EnumerateObject();
                }
                catch (Exception ex)
                {
                    return 0;//Hasnt played comp
                }


                int heighestRank = 0;

                try
                {
                    foreach (var item in seasonslist)
                    {
                        foreach (var Wins in item.Value.GetProperty("WinsByTier").EnumerateObject())
                        {
                            int curRank = int.Parse(Wins.Name);
                            if (curRank > heighestRank)
                                heighestRank = curRank;
                        }


                    }
                }
                catch (Exception ex)//Hasn't won a game
                {
                    foreach (var item in seasonslist)
                    {
                        int curRank = item.Value.GetProperty("CompetitiveTier").GetInt32();
                        if (curRank > heighestRank)
                            heighestRank = curRank;
                    }
                }

                return heighestRank;
            }
        }
        int CalculateElo(int tier, int RR)
        {
            if (tier < 3)
                return 0;

            if (tier > 20)
                return 1800 + RR;
            else
                return ((tier * 100) - 300) + RR;
        }
    }
}
