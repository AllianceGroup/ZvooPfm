using System.Collections.Generic;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class BorrowerData
    {
        public List<BorrowerNameData> Names { get; set; }
        public List<BorrowerBirthDateData> BirthDates { get; set; }
        public List<AddressData> Addresses { get; set; }
        public List<BorrowerTelephoneData> Telephones { get; set; }
        public List<BorrowerCreditScoreData> CreditScores { get; set; }
        public List<BorrowerCreditStatementData> CreditStatements { get; set; }
        public List<BorrowerEmployerData> Employers { get; set; }
        public List<AddressData> PreviousAddresses { get; set; }
        public List<BorrowerSocialData> SocialSecurityNumbers { get; set; }

        public string SocialSecurityNumber { get; set; } 
    }
}