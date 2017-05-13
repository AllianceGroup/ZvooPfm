using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Mvc.Validation;

namespace Default.ViewModel.TransactionsController
{
    public class EditStandardTransactionViewModel
    {
        public string EditType { get; set; }
        public bool Imported { get; set; }
        public string AccountName { get; set; }
        public string OffSetAccountName { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }
        public IEnumerable<SelectListItem> FilteredAccounts { get; set; }
        public IEnumerable<GroupedSelectListItem> Accounts { get; set; }

        [Required(ErrorMessage = "Please select an account for this transaction")]
        public String AccountId { get; set; }

        public String Payee { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Numeric]
        public string AmountInDollars { get; set; }

        public String Memo { get; set; }

        [Required(ErrorMessage = "Please, select transaction category")]
        [NotEqualToProperty(OtherProperty = "AccountId", ErrorMessage = "Transaction category cannot be the same as the selected account")]
        public String OffSetAccountId { get; set; }
        [Required(ErrorMessage = "Date is required")]
        public string BookedDate { get; set; }
        [Required]
        public string TransactionId { get; set; }

        public string ReferenceNumber { get; set; }


        public bool MemorizeCategorization { get; set; }

        public bool ApplyCategorizationToAll { get; set; }

    }
}