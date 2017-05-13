using System;
using MongoDB.Driver;

namespace mPower.Framework.Mongo
{
    public class MongoIntuit
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
        public MongoIntuit(String connectionString)
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

        public MongoCollection Institutions
        {
            get { return Database.GetCollection("institutions"); }
        }

        public MongoCollection Accounts
        {
            get { return Database.GetCollection("accountdocuments"); }
        }

        public MongoCollection Logs
        {
            get { return Database.GetCollection("logs"); }
        }

    }
}

