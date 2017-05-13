using System;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class BorrowerEmployerData
    {
        public string Name { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime DateReported { get; set; }
        public AddressData Address { get; set; } 
    }
}