using System;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class BorrowerBirthDateData
    {
        public decimal Age { get; set; }
        public DateTime BirthDate { get; set; }
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }  
    }
}