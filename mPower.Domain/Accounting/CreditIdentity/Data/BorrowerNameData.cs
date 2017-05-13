using System;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class BorrowerNameData
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Prefix { get; set; }
        public string Description { get; set; }
        public DateTime InquiryDate { get; set; }
        public string Suffix { get; set; }
        public string Reference { get; set; }
        public string Bureau { get; set; } 
    }
}