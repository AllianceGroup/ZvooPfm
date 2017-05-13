using CsvHelper.Configuration;

namespace mPower.OfferingsSystem.Data
{
    public class Statement: AccessRecord
    {
        [CsvField(Name = "organizationCustomerIdentifier")]
        public string OrganizationCustomerId { get; set; }

        [CsvField(Name = "programCustomerIdentifier")]
        public string ProgrammCustomerId { get; set; }

        [CsvField(Name = "cardIdentifier")]
        public string CardId { get; set; }

        [CsvField(Name = "settlementIdentifier")]
        public string SettlementId { get; set; }

        [CsvField(Name = "statementIdentifier")]
        public string StatementId { get; set; }

        [CsvField(Name = "settlementStatus")]
        public string SettlementStatus { get; set; }

        [CsvField(Name = "productIdentifier")]
        public string ProductId { get; set; }

        [CsvField(Name = "transactionIdentifier")]
        public string TransactionId { get; set; }

        [CsvField(Name = "transactionStatus")]
        public string TransactionStatus { get; set; }

        [CsvField(Name = "transactionDatetime")]
        public string TransactionDatetime { get; set; }

        [CsvField(Name = "transactionGross")]
        public string TransactionGross { get; set; }

        [CsvField(Name = "transactionNet")]
        public string TransactionNet { get; set; }

        [CsvField(Name = "transactionTax")]
        public string TransactionTax { get; set; }

        [CsvField(Name = "transactionTip")]
        public string TransactionTip { get; set; }

        [CsvField(Name = "transactionReward")]
        public string TransactionReward { get; set; }
    }
}