using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Member: AccessRecord
    {
        [CsvField(Name = "memberStatus")]
        public string Status { get; set; }

        [CsvField(Name = "organizationCustomerIdentifier")]
        public string OrganizationCustomerId { get; set; }

        [CsvField(Name = "programCustomerIdentifier")]
        public string ProgramCustomerId { get; set; }

        [CsvField(Name = "memberCustomerIdentifier")]
        public string MemberCustomerId { get; set; }

        [CsvField(Name = "previousMemberCustomerIdentifier")]
        public string PreviousMemberCustomerId { get; set; }

        [CsvField(Name = "phoneNumber")]
        public string Phone { get; set; }

        [CsvField(Name = "streetLine1")]
        public string StreetLine1 { get; set; }

        [CsvField(Name = "streetLine2")]
        public string StreetLine2 { get; set; }

        [CsvField(Name = "postalCode")]
        public string PostalCode { get; set; }

        [CsvField(Name = "city")]
        public string City { get; set; }

        [CsvField(Name = "state")]
        public string State { get; set; }

        [CsvField(Name = "country")]
        public string Country { get; set; }

        [CsvField(Name = "fullName")]
        public string FullName { get; set; }

        [CsvField(Name = "firstName")]
        public string FirstName { get; set; }

        [CsvField(Name = "middleName")]
        public string MiddleName { get; set; }

        [CsvField(Name = "lastName")]
        public string LastName { get; set; }

        [CsvField(Name = "emailAddress")]
        public string Email { get; set; }

        [CsvField(Name = "membershipRenewalDate")]
        public string MembershipRenewalDate { get; set; }

        [CsvField(Name = "productIdentifier")]
        public string ProductId { get; set; }

        [CsvField(Name = "productTemplateField1")]
        public string ProductTemplateField1 { get; set; }

        [CsvField(Name = "productTemplateField2")]
        public string ProductTemplateField2 { get; set; }

        [CsvField(Name = "productTemplateField3")]
        public string ProductTemplateField3 { get; set; }

        [CsvField(Name = "productTemplateField4")]
        public string ProductTemplateField4 { get; set; }

        [CsvField(Name = "productTemplateField5")]
        public string ProductTemplateField5 { get; set; }

        [CsvField(Name = "productRegistrationKey")]
        public string ProductRegistrationKey { get; set; }
    }
}