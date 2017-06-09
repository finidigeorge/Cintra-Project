using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Controllers.Auth
{
    public class JwtTokenOptions
    {

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public DateTime IssuedAt => DateTime.UtcNow;

        public TimeSpan ValidFor;

        public DateTime ExpiredAt => IssuedAt.Add(ValidFor);

        public SigningCredentials SigningCredentials { get; set; }

        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());
    }
}
