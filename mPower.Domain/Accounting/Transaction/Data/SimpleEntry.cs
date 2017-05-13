using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Transaction.Data
{
    public class SimpleEntry
    {
        public string AccountId { get; set; }

        public AccountTypeEnum AccountType { get; set; }

        public long DebitAmountInCents { get; set; }

        public long CreditAmountInCents { get; set; }

        public string AccountName { get; set; }

        public SimpleEntry(string accountId, AccountTypeEnum type,string accountName, long debitInCents, long creditInCents)
        {
            AccountId = accountId;
            AccountType = type;
            AccountName = accountName;
            DebitAmountInCents = debitInCents;
            CreditAmountInCents = creditInCents;
        } 
    }
}