using mPower.Aggregation.Contract;
using Paralect.Config.Settings;
using StructureMap;

namespace mPower.Framework.Registries
{
    public class SettingsRegistry
    {
        public SettingsRegistry(IContainer container)
        {
            var settings = SettingsMapper.Map<MPowerSettings>();
            var aggregationSettings = SettingsMapper.Map<AggregationSettings>();
            
            container.Configure(config =>
            {
                config.ForSingletonOf<MPowerSettings>().Use(settings);
                config.ForSingletonOf<AggregationSettings>().Use(aggregationSettings);
            });
        }
    }
}