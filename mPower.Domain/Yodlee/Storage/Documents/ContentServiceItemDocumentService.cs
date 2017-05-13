using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Framework.Mongo;

namespace mPower.Domain.Yodlee.Storage.Documents
{
    public class ContentServiceItemDocumentService : BaseYodleeDocumentService<ContentServiceItemDocument , ContentServiceItemDocumentFilter>
    {
        public ContentServiceItemDocumentService(MongoYodlee mongo)
            : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _db.ContentServiceItems; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(ContentServiceItemDocumentFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.UserId))
            {
               yield return Query.EQ("UserId", filter.UserId);
            }

            if (!string.IsNullOrEmpty(filter.ContentServiceId))
            {
                yield return Query.EQ("ContentServiceId", filter.ContentServiceId);
            }

            if (!string.IsNullOrEmpty(filter.AuthenticationReferenceId))
            {
                yield return Query.EQ("AuthenticationReferenceId", filter.AuthenticationReferenceId);
            }

            if (!string.IsNullOrEmpty(filter.Id))
            {
                yield return Query.EQ("_id", filter.Id);
            }
        }

        public List<ContentServiceItemDocument> GetByUserId(string userId)
        {
            return GetByFilter(new ContentServiceItemDocumentFilter() {UserId = userId}).ToList();
        }

      
    }
}
