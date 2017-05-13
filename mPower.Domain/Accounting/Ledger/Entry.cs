using System;

namespace mPower.Domain.Accounting.Ledger
{
    public class Entry
    {
        public String AccountId { get; set; }
        public DateTime BookedDate { get; set; }
        public Int64 CreditAmount { get; set; }
        public Int64 DebitAmount { get; set; }
        public String Payee { get; set; }
        public String Memo { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Entry(String accountId, DateTime bookedDate, Int64 creditAmount, Int64 debitAmount, string payee, string memo)
        {
            AccountId = accountId;
            BookedDate = bookedDate;
            CreditAmount = creditAmount;
            DebitAmount = debitAmount;
            Payee = payee;
            Memo = memo;
        }
    }
}