using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices
{
    public class EventLogDocumentService : BaseTemporaryService<EventLogDocument, EventLogFilter>
    {
        public EventLogDocumentService(MongoTemp read)
            : base(read)
        {
        }

        protected override MongoCollection Items
        {
            get { return _temp.EventLogs; }
        }

        protected override IMongoSortBy BuildSortExpression(EventLogFilter filter)
        {
            return SortBy.Descending("StoredDate");
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(EventLogFilter filter)
        {
          
            if (!String.IsNullOrEmpty(filter.UserId))
            {
                yield return Query.EQ("UserId", filter.UserId);
            }

            if (filter.MinStoredDate.HasValue)
            {

                yield return Query.GT("StoredDate", TimeZoneInfo.ConvertTimeToUtc(filter.MinStoredDate.Value));
            }
        }

        public List<EventLogDocument> GetUserActivity(string user, PagingInfo paging)
        {
            return GetByFilter(new EventLogFilter() { PagingInfo = paging, UserId = user }).ToList();
        }
    }
}
