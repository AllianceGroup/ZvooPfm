using System;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{
    public class CreditIdentityData
    {
        public string Suffix { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ClientKey { get; set; }
        public string AlertSubscriptionId { get; set; }
    }
}