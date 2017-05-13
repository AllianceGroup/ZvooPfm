using mPower.WebApi.Middlewares;
using mPower.WebApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace mPower.WebApi.Extensions
{
    public static class TokenExtension
    {
        public static IApplicationBuilder UseTokenAuthentication(this IApplicationBuilder applicationBuilder)
        {
            return new TokenMiddleware().Configure(applicationBuilder);
        }

        public static void AddTokenAuthentication(this IServiceCollection serviceCollection)
        {
            new TokenService().Configure(serviceCollection);
        }
    }
}