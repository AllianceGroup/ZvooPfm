using System;
using MongoDB.Driver;

namespace mPower.Framework.Mongo
{
    public class MongoYodlee
    {
        /// <summary>
        /// MongoDB Server
        /// </summary>
        private readonly MongoServer _server;

        /// <summary>
        /// Name of database 
        /// </summary>
        private readonly string _databaseName;

        /// <summary>
        /// Opens connection to MongoDB Server
        /// </summary>
        public MongoYodlee(String connectionString)
        {
            _databaseName = MongoUrl.Create(connectionString).DatabaseName;
            _server = MongoServer.Create(connectionString);
        }

        /// <summary>
        /// MongoDB Server
        /// </summary>
        public MongoServer Server
        {
            get { return _server; }
        }

        /// <summary>
        /// Get database
        /// </summary>
        public MongoDatabase Database
        {
            get { return _server.GetDatabase(_databaseName); }
        }

        public MongoCollection ContentServices
        {
            get { return Database.GetCollection("content_services"); }
        }

        public MongoCollection ContentServiceItems
        {
            get { return Database.GetCollection("content_service_items"); }
        }

      

    }
}
