using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using mPower.Framework;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.Documents;

namespace mPower.TempDocuments.Server.DocumentServices
{
    public class MerchantDocumentService : BaseTemporaryService<MerchantDocument, BaseFilter>, IMerchantDocumentService
    {
        public MerchantDocumentService(MongoTemp temp) : base(temp)
        {
        }

        protected override MongoCollection Items
        {
            get { return _temp.Merchants; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(BaseFilter filter)
        {
            yield break;
        }

        public IEnumerable<string> GetAllIds()
        {
            return Items.FindAllAs<MerchantDocument>().SetFields("_id").Select(x => x.Id).ToList();
        }
    }

    public interface IMerchantDocumentService
    {
        void InsertMany(IEnumerable<MerchantDocument> merchants);
        MerchantDocument GetById(string id);
        IEnumerable<string> GetAllIds();
        void Insert(MerchantDocument merchantDocument);
    }
}