using NLog;
using NLog.Config;
using mPower.Framework.Mongo;

namespace mPower.Framework
{
    public class MPowerLogManager
    {
        private static Logger _logger;

        public static Logger CurrentLogger
        {
            get
            {
                if (_logger == null)
                {
                    ConfigurationItemFactory.Default.Targets.RegisterDefinition("MongoTarget", typeof(NLogMongoTarget));
                    _logger = LogManager.GetCurrentClassLogger();
                }
                return _logger;
            }
        }
    }
}