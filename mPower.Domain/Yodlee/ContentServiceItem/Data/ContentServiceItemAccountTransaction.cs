using System;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Yodlee.ContentServiceItem.Data
{
    public class ContentServiceItemAccountTransaction
    {
        public string Status { get; set; }
        public string Type { get; set; }
        public string BankTransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string CategorizationKeyword { get; set; }
        public double PrincipalAmount { get; set; }
        public double InterestAmount { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}