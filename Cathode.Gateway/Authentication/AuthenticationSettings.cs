namespace Cathode.Gateway.Authentication
{
    public class AuthenticationSettings : GatewaySetting
    {
        public const string SettingId = "authentication";
        public const string TypeId = "authentication";

        public override string Type => TypeId;

        public string JwtIssuer { get; set; }

        public byte[] JwtPrivateKey { get; set; }
    }
}