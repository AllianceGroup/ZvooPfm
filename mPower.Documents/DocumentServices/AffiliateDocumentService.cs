using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Affiliate;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices
{
    public class AffiliateDocumentService : BaseDocumentService<AffiliateDocument, BaseFilter>
    {
        public AffiliateDocumentService(MongoRead mongo)
            : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.Affiliates; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(BaseFilter filter)
        {
            yield break;
        }

        public ChargifyProductDocument GetSignUpProduct(string affiliateId, bool trial)
        {
            var affiliate = GetById(affiliateId);
            var productIdWithoutTrial = affiliate.SignupProductIdWithoutTrial;
            var trialProductId = affiliate.SignupProductId;
            ChargifyProductDocument result = null;
            if (trial && trialProductId != 0)
            {
                result = affiliate.Products.FirstOrDefault(x => x.Id == trialProductId);
            }
            else if (productIdWithoutTrial != 0)
            {
                result = affiliate.Products.Single(x => x.Id == productIdWithoutTrial);
            }
            else
            {
                result = affiliate.Products.First(x => x.IsArchived == false);
            }

            return result;
        }

        public ChargifyProductDocument GetAdditionalCreditIdentityProduct(string affiliateId)
        {
            var affiliate = GetById(affiliateId);

            return affiliate.Products.Single(x => x.Id == affiliate.AdditionalCreditIdentityProduct);
        }
    }
}
