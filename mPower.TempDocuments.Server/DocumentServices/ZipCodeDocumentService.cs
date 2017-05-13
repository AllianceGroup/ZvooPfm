using System.Collections.Generic;
using MongoDB.Driver;
using mPower.Framework;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.Documents;

namespace mPower.TempDocuments.Server.DocumentServices
{
    public class ZipCodeDocumentService : BaseTemporaryService<ZipCodeDocument, OfferFilter>
    {
        public ZipCodeDocumentService(MongoTemp temp) : base(temp)
        {
        }

        protected override MongoCollection Items
        {
            get { return _temp.Database.GetCollection("zipcodes"); }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(OfferFilter filter)
        {
            yield break;
        }
    }
}