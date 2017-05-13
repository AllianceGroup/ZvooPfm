using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Transaction: AccessRecord
    {
        [CsvField(Name = "recordStatus")]
        public string RecordStatus { get; set; }

        [CsvField(Name = "recordStatusMessage")]
        public string RecordStatusMessage { get; set; }

        [CsvField(Name = "organizationCustomerIdentifier")]
        public string OrganizationCustomerId { get; set; }

        [CsvField(Name = "programCustomerIdentifier")]
        public string ProgrammCustomerId { get; set; }

        [CsvField(Name = "memberCustomerIdentifier")]
        public string MemberCustomerId { get; set; }

        [CsvField(Name = "cardIdentifier")]
        public string CardId { get; set; }

        [CsvField(Name = "midValue")]
        public string MidValue { get; set; }

        [CsvField(Name = "transactionIdentifier")]
        public string TransactionId { get; set; }

        [CsvField(Name = "transactionDatetime")]
        public string TransactionDatetime { get; set; }

        [CsvField(Name = "transactionGross")]
        public string TransactionGross { get; set; }

        [CsvField(Name = "authorizationCode")]
        public string AuthorizationCode { get; set; }

        [CsvField(Name = "transactionNet")]
        public string TransactionNet { get; set; }

        [CsvField(Name = "transactionTax")]
        public string TransactionTax { get; set; }

        [CsvField(Name = "transactionTip")]
        public string TransactionTip { get; set; }

        [CsvField(Name = "transactionReward")]
        public string TransactionReward { get; set; }

        [CsvField(Name = "transactionStatus")]
        public string TransactionStatus { get; set; }

        [CsvField(Name = "transactionCode")]
        public string TransactionCode { get; set; }

        [CsvField(Name = "offerIdentifier")]
        public string OfferIdentifier { get; set; }
    }
}