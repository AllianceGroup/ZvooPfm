using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger.Data
{
    public class SimpleEntry
    {
        public AccountTypeEnum AccountType { get; set; }

        public long DebitAmountInCents { get; set; }

        public long CreditAmountInCents { get; set; }

        public SimpleEntry(AccountTypeEnum type, long debitInCents, long creditInCents)
        {
            AccountType = type;
            DebitAmountInCents = debitInCents;
            CreditAmountInCents = creditInCents;
        } 
    }
}