using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace mPower.Framework
{
    public class MongoRead
    {
        /// <summary>
        ///     MongoDB Server
        /// </summary>
        private readonly MongoServer _server;

        /// <summary>
        ///     Name of database
        /// </summary>
        private readonly string _databaseName;

        public MongoUrl MongoUrl { get; }

        /// <summary>
        ///     Opens connection to MongoDB Server
        /// </summary>
        public MongoRead(string connectionString)
        {
            MongoUrl = MongoUrl.Create(connectionString);
            _databaseName = MongoUrl.DatabaseName;
            _server = new MongoClient(connectionString).GetServer();
        }

        public void EnsureIndexes()
        {
            Entries.CreateIndex(IndexKeys.Ascending("LedgerId", "TransactionId").Descending("BookedDate"));
            Entries.CreateIndex(IndexKeys.Ascending("LedgerId", "TransactionId"));
            Entries.CreateIndex(IndexKeys.Ascending("OffsetAccountId").Descending("BookedDate"));
            UserSegments.CreateIndex(IndexKeys.Ascending("Key"));
            //EventLogs.CreateIndex(IndexKeys.Descending("StoredDate"));
            Entries.CreateIndex("OffsetAccountId", "LedgerId");
            Entries.CreateIndex("AccountId", "LedgerId");
            UserSegments.CreateIndex(
                IndexKeys.Ascending("UserId",
                    "FormattedDate",
                    "UserSegmentType",
                    "AggregateData.Merchant",
                    "AggregateData.SpendingCategory")
                , IndexOptions.SetName("UserSegment_MainIndex"));
        }

        /// <summary>
        ///     MongoDB Server
        /// </summary>
        public MongoServer Server => _server;

        /// <summary>
        ///     Get database
        /// </summary>
        public MongoDatabase Database => _server.GetDatabase(_databaseName);

        public List<string> AllDatabases()
        {
            return _server.GetDatabaseNames().ToList();
        }

        public MongoDatabase GetDatabase(string name)
        {
            return _server.GetDatabase(name);
        }

        public MongoCollection Ledgers => Database.GetCollection("ledgers");

        public MongoCollection Transactions => Database.GetCollection("transactions");

        public MongoCollection ImportedTransactions => Database.GetCollection("imported_transactions");

        public MongoCollection Entries => Database.GetCollection("entries");

        public MongoCollection EntryDuplicates => Database.GetCollection("entry_duplicates");

        public MongoCollection TransactionDuplicates => Database.GetCollection("transaction_duplicates");

        public MongoCollection TransactionsStatistics => Database.GetCollection("transactions_statistics");

        public MongoCollection Calendars => Database.GetCollection("calendars");

        // Mobile collections
        public MongoCollection MobileLedgers => Database.GetCollection("mobileledgers");

        public MongoCollection MobileAccounts => Database.GetCollection("mobileaccounts");

        public MongoCollection MobileEntries => Database.GetCollection("mobileentries");

        public MongoCollection MobileCreditIdentities => Database.GetCollection("mobile_credit_identities");

        public MongoCollection MobileBudgets => Database.GetCollection("mobile_budgets");

        /// <summary>
        ///     Membership users collection
        /// </summary>
        public MongoCollection Users => Database.GetCollection("users");

        public MongoCollection WebHooks => Database.GetCollection("webhooks");

        public MongoCollection RawWebHooks => Database.GetCollection("raw_webhooks");

        public MongoCollection Affiliates => Database.GetCollection("affiliates");

        public MongoCollection UserLogins => Database.GetCollection("user_logins");

        /// <summary>
        ///     Credit Report collections
        /// </summary>
        public MongoCollection CreditIdentities => Database.GetCollection("credit_identities");

        public MongoCollection Test => Database.GetCollection("test");

        public MongoCollection DebtElimintations => Database.GetCollection("debt_eliminations");

        public MongoCollection Budgets => Database.GetCollection("budgets");

        public MongoCollection Goals => Database.GetCollection("goals");

        public MongoCollection UserSegments => Database.GetCollection("user_segments");

        public MongoCollection UserGlobalSegments => Database.GetCollection("user_segments_global");

        public MongoCollection Advisers => Database.GetCollection("advisers");

        public MongoCollection MarketHistoryData => Database.GetCollection("MarketHistoryData");

        public MongoCollection GetCollection(string name)
        {
            return Database.GetCollection(name);
        }
    }
}
