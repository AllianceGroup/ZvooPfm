using System;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class CreditorData
    {
        public string Name { get; set; }
        public string IndustryCodeDescription { get; set; }
        public string IndustryCodeSymbol { get; set; }
        public int IndustryCodeRank { get; set; }
        public string Bureau { get; set; }
        public DateTime InquiryDate { get; set; }
        public string Code { get; set; }
        public string Telephone { get; set; }
        public AddressData Address { get; set; } 
    }
}