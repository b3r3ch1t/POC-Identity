using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Infra.CrossCutting.Identity.Authorization
{
    public class JwtTokenOptions
    {

        public static SymmetricSecurityKey SigningKey
        {
            get
            {

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                // Build config
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true)
                    .Build();


                var secretKey = config["AppSettings:Secret"];

                var key = Encoding.ASCII.GetBytes(secretKey);
                return new SymmetricSecurityKey(key);

            }
        }

        public string Issuer { get; set; }
        public string Subject { get; set; }
        public string Audience { get; set; }
        public DateTime NotBefore { get; set; } = DateTime.Now;
        public DateTime IssuedAt { get; set; } = DateTime.Now;
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromDays(7);
        public DateTime Expiration => IssuedAt.Add(ValidFor);
        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());
        public SigningCredentials SigningCredentials => new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);
    }
}
