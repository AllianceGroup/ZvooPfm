using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Framework.Mongo;

namespace mPower.Domain.Yodlee.Storage.Documents
{
    public class ContentServiceDocumentService : BaseYodleeDocumentService<ContentServiceDocument , ContentServiceDocumentFilter>
    {
        

        public ContentServiceDocumentService(MongoYodlee mongo)
            : base(mongo)
        {
           
        }

        protected override MongoCollection Items
        {
            get { return _db.ContentServices; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(ContentServiceDocumentFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ContentServiceDisplayName))
            {
                //TODO: This filter will work very, very slow. We probably should use lucene or solr to make it fast and real full text search
                yield return
                    Query.Or(
                    Query.Matches("contentServiceDisplayName", BsonRegularExpression.Create(filter.ContentServiceDisplayName, "-i")),
                    Query.Matches("siteDisplayName", BsonRegularExpression.Create(filter.ContentServiceDisplayName, "-i")),
                    Query.Matches("homeUrl", BsonRegularExpression.Create(filter.ContentServiceDisplayName, "-i")),
                    Query.Matches("organizationDisplayName", BsonRegularExpression.Create(filter.ContentServiceDisplayName, "-i")));
            }
        }


        public List<ContentServiceDocument> Search(string term)
        {
            return this.GetByFilter(new ContentServiceDocumentFilter(){ContentServiceDisplayName = term});
            
        }
    }
}
