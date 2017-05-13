using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Documents.Documents.Membership;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Membership
{
    public class UserLoginsDocumentService : BaseDocumentService<UserLoginDocument, UserLoginsFilter>
    {
        public UserLoginsDocumentService(MongoRead mongo)
            : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.UserLogins; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(UserLoginsFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.UserId))
            {
                yield return Query.EQ("UserId", filter.UserId);
            }

            if (filter.From.HasValue)
            {
                yield return Query.GTE("LoginDate", filter.From.Value);
            }
            if (filter.To.HasValue)
            {
                yield return Query.LTE("LoginDate", filter.To.Value);
            }

            if (!String.IsNullOrEmpty(filter.SearchKey))
            {
                yield return Query.Or(
                    Query.Matches("UserName", BsonRegularExpression.Create(filter.SearchKey.ToLower())),
                    Query.Matches("UserEmail", BsonRegularExpression.Create(filter.SearchKey.ToLower())),
                    Query.Matches("AffiliateName", BsonRegularExpression.Create(filter.SearchKey.ToLower())));
            }
        }

        protected override IMongoSortBy BuildSortExpression(UserLoginsFilter filter)
        {
            var sortExpression = SortBy.Null;

            switch (filter.SortByField)
            {
                case UserLoginsSortEnum.LoginDate:
                    sortExpression = SortBy.Descending("LoginDate").Ascending("UserName");
                    break;
                case UserLoginsSortEnum.AffiliateName:
                    sortExpression = SortBy.Ascending("AffiliateName");
                    break;
            }

            return sortExpression;
        }
    }
}
