using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Affiliate;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices
{
    public class AffiliateAnalyticsDocumentService : BaseDocumentService<AffiliateAnalyticsDocument, BaseFilter>
    {
        public AffiliateAnalyticsDocumentService(MongoRead mongo) : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.GetCollection("affiliates_statistics"); }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(BaseFilter filter)
        {
            yield break;
        }
    }
}