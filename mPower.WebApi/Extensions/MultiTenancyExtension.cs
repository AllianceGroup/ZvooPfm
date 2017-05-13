using mPower.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace mPower.WebApi.Extensions
{
    public static class MultiTenancyExtension
    {
        public static void AddMultiTenancy(this IServiceCollection servieCollection, IContainer container)
        {
            new MultiTenancyService(container).Configure();
        }
    }
}