using System.Collections.Generic;
using MongoDB.Driver;
using mPower.Documents.Documents.Membership;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Membership
{
    public class UserStatisticDocumentService : BaseDocumentService<UserStatisticDocument,BaseFilter>
    {
        public UserStatisticDocumentService(MongoRead mongo) : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.GetCollection("user_statistics"); }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(BaseFilter filter)
        {
            yield break;
        }
    }
}