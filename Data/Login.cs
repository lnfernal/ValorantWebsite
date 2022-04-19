using ValorantManager.JsonModels;

namespace ValorantManager.Data
{

    public enum LoginState
    {
        LoggedOut,
        LoggingIn,
        LoggedIn,
        WrongLogin
    }

    public class User
    {
        public string auth_username { get; set; }
        public string auth_password { get; set; }
        public string Name { get; set; }
        public string Token { get; set; } = null;
        public string Entitlement { get; set; }
        public string puuid { get; set; }
        public long CreationDate { get; set; }
        public Classes.Region.RiotServer riotServer { get; set; }
        public LoginState loginState { get; set; } = LoginState.LoggedOut;

        public Wallet.Rootobject wallet { get; set; } = null;

    }
}
