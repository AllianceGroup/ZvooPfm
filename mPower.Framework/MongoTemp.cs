using System;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace mPower.Framework
{
    public class MongoTemp
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
        public MongoTemp(String connectionString)
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

        public MongoCollection Imports
        {
            get { return Database.GetCollection("imports");}
        }

        public MongoCollection Alerts
        {
            get { return Database.GetCollection("alerts"); }
        }

        public MongoCollection Notifications
        {
            get { return Database.GetCollection("notifications"); }
        }

        public MongoCollection CommandLogs
        {
            get { return Database.GetCollection("command_logs"); }
        }

        public MongoCollection Offers
        {
            get { return Database.GetCollection("offers"); }
        }

        public MongoCollection OfferGroups
        {
            get { return Database.GetCollection("offergroups"); }
        }
        public MongoCollection Merchants
        {
            get { return Database.GetCollection("merchants"); }
        }

        public MongoCollection EventLogs
        {
            get { return Database.GetCollection("event_logs"); }
        }

        public void EnsureIndexes()
        {
            Offers.CreateIndex(IndexKeys.GeoSpatial("GeoLocation"));
            Offers.CreateIndex(IndexKeys.GeoSpatial("GeoLocation").Ascending("MerchantIndex.First5"));
            Offers.CreateIndex(IndexKeys.GeoSpatial("GeoLocation").Ascending("MerchantIndex.First5").Ascending("Declined"));
            Offers.CreateIndex(IndexKeys.Ascending("Merchant"));
            Offers.CreateIndex(IndexKeys.Ascending("Title"));
        }
    }
}
