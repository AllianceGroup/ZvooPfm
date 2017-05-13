using mPower.WebApi.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace mPower.WebApi.Services
{
    public class TokenService
    {
        private const string TokenAudience = "audience";
        private const string TokenIssuer = "issuer";

        public void Configure(IServiceCollection services)
        {
            // *** CHANGE THIS FOR PRODUCTION USE ***
            // Here, we're generating a random key to sign tokens - obviously this means
            // that each time the app is started the key will change, and multiple servers 
            // all have different keys. This should be changed to load a key from a file 
            // securely delivered to your application, controlled by configuration.
            //
            // See the RSAKeyUtils.GetKeyParameters method for an examle of loading from
            // a JSON file.
            var keyParams = RSAKeyUtils.GetRandomKey();


            var key = new RsaSecurityKey(keyParams);
            var tokenOptions = new TokenAuthOptions
            {
                Audience = TokenAudience,
                Issuer = TokenIssuer,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
            };
            services.AddSingleton(tokenOptions);

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Pfm", policy => policy.RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
        }
    }
}