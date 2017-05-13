using mPower.Framework.Mongo;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace mPower.Framework.Registries
{
    public class MongoRegistry: Registry
    {
        public MongoRegistry(IContainer container)
        {
            // Configure mongo driver
            MongoConvention.Configure();

            var settings = container.GetInstance<MPowerSettings>();

            container.Configure(config =>
            {
                // Mongo Read database
                config.For<MongoRead>().Singleton().Use(() => 
                    new MongoRead(settings.MongoReadDatabaseConnectionString));

                // Mongo Write database
                config.For<MongoWrite>().Singleton().Use(() => 
                    new MongoWrite(settings.MongoWriteDatabaseConnectionString));

                // Mongo Temporary database

                var mongoTemp = new MongoTemp(settings.MongoTempDatabaseConnectionString);
                mongoTemp.EnsureIndexes();
                config.For<MongoTemp>().Singleton().Use(() => mongoTemp);

                // Mongo Yodlee database
                config.For<MongoYodlee>().Singleton().Use(() =>
                    new MongoYodlee(settings.MongoYodleeDatabaseConnectionString));

                // Mongo Yodlee database
                config.For<MongoIntuit>().Singleton().Use(() =>
                    new MongoIntuit(settings.MongoIntuitDatabaseConnectionString));
                config.For<MongoLog>().Singleton().Use(() =>
                  new MongoLog(settings)); 
            });
        }
    }
}