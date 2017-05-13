using mPower.Framework.Utils;

namespace mPower.Domain.Accounting.Enums
{
    public enum TransactionType
    {
        [IifName("BILL")]
        Bill,

        [IifName("CHECK")]
        Check,

        [IifName("DEPOSIT")]
        Deposit,

        [IifName("INVOICE")]
        Invoice,

        [IifName("PAYMENT")]
        BillPayment, // Changed from Payment to Bill Payment on 9/7

        [IifName("GENERAL JOURNAL")]
        GeneralJournal,
       
        CreditCard,

        Transfer,

        BankTransfer,

        Unknown = int.MaxValue, // just for internal usage
    }
}
