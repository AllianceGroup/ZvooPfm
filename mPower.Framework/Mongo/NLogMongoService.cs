using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Wrappers;
using mPower.Framework.Services;

namespace mPower.Framework.Mongo
{
    public class NLogMongoService : BaseMongoService<NLogMongoTarget.NlogMongoItem, NLogMongoFilter>
    {
        private readonly MongoLog _mongoLog;

        public NLogMongoService(MongoLog mongoLog)
        {
            _mongoLog = mongoLog;
        }

        protected override MongoCollection Items
        {
            get { return _mongoLog.Logs; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(NLogMongoFilter filter)
        {
            if (!String.IsNullOrEmpty(filter.SearchKey))
            {
                yield return Query.Or(Query.EQ("_id", filter.SearchKey),
                                           Query.EQ("UserId", filter.SearchKey),
                                           Query.EQ("UserEmail", filter.SearchKey),
                                           Query.Matches("LogMessage", BsonRegularExpression.Create(filter.SearchKey.ToLower(), "-i-")),
                                           Query.Matches("ExceptionMessage", BsonRegularExpression.Create(filter.SearchKey.ToLower(), "-i-")));         
            }
            // Date
            if (filter.MinDate != null )
            {
                yield return Query.GTE("Date", filter.MinDate.Value);
            }

            if (filter.MaxDate != null)
            {
                yield return Query.LTE("Date", filter.MaxDate.Value);
            }

            // Level
            if (!String.IsNullOrEmpty(filter.Level))
            {
                yield return Query.EQ("Level", filter.Level);
            }
        }

        protected override IMongoSortBy BuildSortExpression(NLogMongoFilter filter)
        {
            if (!String.IsNullOrEmpty(filter.SortExpression))
            {
                return SortBy.Descending(filter.SortExpression);
            }
            return SortBy.Null;
        }
    }

    public class NLogMongoFilter : BaseFilter
    {
        public string SearchKey { get; set; }

        public DateTime? MaxDate { get; set; }

        public DateTime? MinDate { get; set; }

        public string Level { get; set; }

        public String SortExpression { get; set; }

        public String LogMessageContains { get; set; }
    }
}
