using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Framework;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.DocumentServices.Filters;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;

namespace mPower.TempDocuments.Server.Notifications.DocumentServices
{
    public class NotificationTempService : BaseTemporaryService<BaseNotification, NotificationFilter>
    {
        public NotificationTempService(MongoTemp mongoTemp)
            : base(mongoTemp)
        {
        }

        protected override MongoCollection Items
        {
            get { return _temp.Notifications; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(NotificationFilter filter)
        {
            var query = new List<IMongoQuery>();
            if (filter.Ids != null && filter.Ids.Count > 0)
            {
                query.Add(Query.In("_id", new BsonArray(filter.Ids)));
            }

            if (filter.Type.HasValue)
            {
                query.Add(Query.EQ("Type", filter.Type.Value));
            }

            if (filter.PublicKeys != null && filter.PublicKeys.Count > 0)
            {
                query.Clear();
                query.Add(Query.In("PublicKey", new BsonArray(filter.PublicKeys)));
            }
            return query;
        }

        public BaseNotification GetNotificationAndSetInProgress()
        {
            var query = Query.And(Query.LTE("SendDate", DateTime.UtcNow), Query.EQ("InProgress", false));
            var update = MongoDB.Driver.Builders.Update<BaseNotification>.Set(x => x.InProgress, true);

            return FindAndModify<BaseNotification>(query, SortBy.Ascending("SendDate"), update);
        }
    }
}
