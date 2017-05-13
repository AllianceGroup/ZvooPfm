using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class BorrowerDocument
    {
        [BsonId]
        public string Id { get; set; }

        public List<BorrowerNameDocument> Names { get; set; }
        public List<BorrowerBirthDateDocument> BirthDates { get; set; }
        public List<AddressDocument> Addresses { get; set; }
        public List<BorrowerTelephoneDocument> Telephones { get; set; }
        public List<BorrowerCreditScoreDocument> CreditScores { get; set; }
        public List<BorrowerCreditStatementDocument> CreditStatements { get; set; }
        public List<BorrowerEmployerDocument> Employers { get; set; }
        public List<AddressDocument> PreviousAddresses { get; set; }
        public List<BorrowerSocialDocument> SocialSecurityNumbers { get; set; }

        public string SocialSecurityNumber { get; set; } 
    }
}
