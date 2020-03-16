using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc.Validation;

namespace Default.ViewModel.TransactionsController
{
    public class ImportTransactionViewModel
    {
        public ImportTransactionViewModel()
        {
            Transactions = new List<TransactionsController.Transactions>();
        }
        public string LedgerId { get; set; }

        public IEnumerable<SelectListItem> FilteredAccounts { get; set; }
        public IEnumerable<GroupedSelectListItem> Accounts { get; set; }

        [Required(ErrorMessage = "Please choose an account for this transaction")]
        public String AccountId { get; set; }

        public List<Transactions> Transactions { get; set; }
    }

    public class Transactions
    {
        public String Payee { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Numeric]
        public double AmountInDollars { get; set; }

        public String Memo { get; set; }

        [Required(ErrorMessage = "Please choose an account to categorize this transaction")]
        [NotEqualToProperty(OtherProperty = "AccountId", ErrorMessage = "Transaction category cannot be the same as the selected account")]
        public String OffSetAccountId { get; set; }

        [Required(ErrorMessage = "Date field is required")]
        public string BookedDate { get; set; }

        public TransactionType TransactionType { get; set; }

        public string ReferenceNumber { get; set; }

        public string Category { get; set; }
    }
}