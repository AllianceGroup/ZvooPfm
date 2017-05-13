using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Wrappers;

namespace mPower.Framework
{
    public class MongoLog
    {
        /// <summary>
        /// MongoDB Server
        /// </summary>
        private readonly MongoServer _server;

        /// <summary>
        /// Name of database 
        /// </summary>
        private readonly string _databaseName;

        private readonly string _logsCollectionName;


        /// <summary>
        /// Opens connection to MongoDB Server
        /// </summary>
        public MongoLog(MPowerSettings settings)
        {
            _databaseName = MongoUrl.Create(settings.LogsDatabaseConnectionString).DatabaseName;
            _server = new MongoClient(settings.LogsDatabaseConnectionString).GetServer();
            _logsCollectionName = settings.LogsCollectionName;
        }

        /// <summary>
        /// MongoDB Server
        /// </summary>
        public MongoServer Server => _server;

        /// <summary>
        /// Get database
        /// </summary>
        public MongoDatabase Database => _server.GetDatabase(_databaseName);

        private const long CollectionMaxSizeInBytes = 1 * 1024 * 1024 * 1024;// = 1Gb 
        private const long CollectionMaxDocumentsCount = 500000;

        public MongoCollection Logs => GetCappedCollection(_logsCollectionName);

        private MongoCollection GetCappedCollection(string collectionName)
        {
            var collection = Database.GetCollection(collectionName);
            if (collection.Exists() && !collection.IsCapped())
            {
                collection.Drop();
                var optionsDocument = new CollectionOptionsBuilder()
                    .SetCapped(true)
                    .SetMaxDocuments(CollectionMaxDocumentsCount)
                    .SetMaxSize(CollectionMaxSizeInBytes)
                    .ToBsonDocument();
                var options = new CollectionOptionsWrapper(optionsDocument);
                Database.CreateCollection(collectionName, options);
                collection = Database.GetCollection(collectionName);
            }
            return collection;
        }
    }
}