using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class BorrowerCreditScoreFactorDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string Bureau { get; set; }
        public string FactorAbbreviation { get; set; }
        public string FactorDescription { get; set; }
        public string FactorSymbol { get; set; }
        public int FactorRank { get; set; }  
    }
}
