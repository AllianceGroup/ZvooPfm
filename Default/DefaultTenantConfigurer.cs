using mPower.Framework.Environment.MultiTenancy;
using StructureMap;

namespace Default
{
    /// <summary>
    /// Use this for the custom Container Configuration
    /// </summary>
    public class TestTenantConfigurer : ITenantConfigurer
    {
        public void Configure(Container container)
        {
            container.Configure(config =>
            {
               
            });
        }
    }
}