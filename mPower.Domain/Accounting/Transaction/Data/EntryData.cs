using System;

namespace mPower.Domain.Accounting.Transaction.Data
{
    public class EntryData
    {
        public EntryData()
        {
            
        }

        public EntryData(string accountId, Int64 debit, Int64 credit, DateTime bookedDate)
        {
            AccountId = accountId;
            DebitAmountInCents = debit;
            CreditAmountInCents = credit;
            BookedDate = bookedDate;
        }


        public String AccountId { get; set; }
        public DateTime BookedDate { get; set; }
        public Int64 CreditAmountInCents { get; set; }
        public Int64 DebitAmountInCents { get; set; }
        public String Payee { get; set; }
        public String Memo { get; set; }
    }
}