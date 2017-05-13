using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Framework;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.DocumentServices.Filters;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;

namespace mPower.TempDocuments.Server.Notifications.DocumentServices
{
    public class DashboardAlertTempService : BaseTemporaryService<DashboardAlertDocument, AlertFilter>
    {
        public DashboardAlertTempService(MongoTemp mongoTemp) : base(mongoTemp)
        {
        }

        protected override MongoCollection Items
        {
            get { return _temp.Alerts; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(AlertFilter filter)
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

        public List<DashboardAlertDocument> GetLatestAlerts(string userId, string ledgerId, int? limit = null)
        {
            var query = Query.And(Query.EQ("UserId", userId), Query.EQ("LedgerId", ledgerId));

            var list = GetByQuery(query, x =>
            {
                x.SetSortOrder(SortBy.Descending("CreatedDate"));
                if (limit.HasValue)
                {
                    x.SetLimit(limit.Value);
                }
            });

            return list;
        }
    }
}