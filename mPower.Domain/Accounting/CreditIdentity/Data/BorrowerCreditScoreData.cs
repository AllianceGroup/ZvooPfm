using System;
using System.Collections.Generic;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class BorrowerCreditScoreData
    {
        public string Bureau { get; set; }
        public DateTime InquiryDate { get; set; }
        public decimal Score { get; set; }
        public List<BorrowerCreditScoreFactorData> CreditScoreFactors { get; set; }  
    }
}