using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Default.ViewModel.Areas.Shared;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.TransactionsController
{
    public class AddTransactionPostModel
    {
        public TransactionType Type { get; set; }
        public DateTime BookedDate { get; set; }
        public List<Category> Categories { get; set; } 
        public List<AddTransactionPostEntry> Entries { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; } 
        
        public string OffSetAccountId { get; set; }

        public AddTransactionPostModel()
        {
            Categories = new List<Category>();
            Entries = new List<AddTransactionPostEntry>();
        }
    }


    public class EditMultipleEntriesViewModel
    {
        public string LedgerId { get; set; }
        public string TransactionsIds { get; set; }

        public int TransactionsCount { get; set; }

        public IEnumerable<GroupedSelectListItem> Accounts { get; set; }

        [Required]
        public string AccountId { get; set; }

        public String Memo { get; set; }
    }
}