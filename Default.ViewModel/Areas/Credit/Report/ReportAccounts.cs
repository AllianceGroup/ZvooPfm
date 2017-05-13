using System;
using System.Collections.Generic;

namespace Default.ViewModel.Areas.Credit.Report
{
    public class ReportAccount
    {
        public string AccountName { get; set; }
        public decimal Count { get; set; }

        //block one - column one
        public string AccountStatus { get; set; }
        public DateTime AccountStatusDate { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime? DateClosed { get; set; }
        public DateTime? DateReported { get; set; }
        public DateTime? DateVerified { get; set; }
        public string Disputes { get; set; }
        public string Bureau { get; set; }
        public string ReferenceNumber { get; set; }

        //block one - column two 
        public string OpenClosed { get; set; }
        public string AccountDesignation { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string AccountCondition { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal HighBalance { get; set; }
        public string Industry { get; set; }
        public string Verification { get; set; }

        //block two - column one
        public string Description { get; set; }
        public decimal AmountPastDue { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal MonthlyPayment { get; set; }
        public decimal TermsInMonths { get; set; }
        public string PaymentFrequency { get; set; }
        public DateTime PaymentHistoryStart { get; set; }
        public List<Payment> PaymentHistory { get; set; } 

        //block two - column two
        public string CreditType { get; set; }
        public DateTime? DatePastDue { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public decimal Count30DaysLate { get; set; }
        public decimal Count60DaysLate { get; set; }
        public decimal Count90DaysLate { get; set; }
    }

    public class Payment
    {
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}
