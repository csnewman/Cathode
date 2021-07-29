using System;

namespace Cathode.Gateway.Certificates
{
    public class AcmeSettings : GatewaySetting
    {
        public const string Id = "acme";

        public override string Type => Id;

        public string? Server { get; set; }

        public string? Email { get; set; }

        public string? Domain { get; set; }

        public byte[]? AccountKey { get; set; }

        public byte[]? Certificate { get; set; }

        protected bool Equals(AcmeSettings other)
        {
            return Server == other.Server && Email == other.Email && Domain == other.Domain &&
                   Equals(AccountKey, other.AccountKey) && Equals(Certificate, other.Certificate);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AcmeSettings)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Server, Email, Domain, AccountKey, Certificate);
        }

        public override object Clone()
        {
            return new AcmeSettings
            {
                Server = Server,
                AccountKey = AccountKey,
                Certificate = Certificate,
                Domain = Domain,
                Email = Email
            };
        }
    }
}