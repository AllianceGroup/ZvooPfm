using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.GettingStartedController
{
    public class AssignAccountsToLedgerViewModel
    {
        public IEnumerable<SelectListItem> AccountTypes { get; set; }
        public IEnumerable<SelectListItem> LedgerChoices { get; set; }
        public string ContentServiceItemId { get; set; }
        public IList<UnAssignedAccount> Accounts { get; set; }
        public string ContentServiceDisplayName { get; set; }

        public AssignAccountsToLedgerViewModel()
        {
            Accounts = new List<UnAssignedAccount>();
            AccountTypes = Build();
        }

        public IEnumerable<SelectListItem> Build()
        {
            var values = new List<SelectListItem>
                             {
                                 new SelectListItem(){Text = "-- Choose Account Type --", Value=""},
                                 new SelectListItem(){Text = "Bank", Value = "Bank" },
                                 new SelectListItem(){Text = "Credit Card", Value = "CreditCard" },
                                 new SelectListItem(){Text = "Loan", Value = "Loan" },
                                 new SelectListItem(){Text = "Investment", Value = "Investment"}
                             };
            
            return values.ToList();

        }

        public bool IsValid()
        {
            return Accounts.Any(x => x.Selected) && Accounts.All(x => x.IsValid());
        }

    }

    
    public class UnAssignedAccount
    {
        public decimal? Balance { get; set; }
        public string Nickname { get; set; }
        public bool Selected { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string AssignedLedgerId { get; set; }
        public AccountLabelEnum? AssignedAccountType { get; set; }
        
        public bool IsValid()
        {
            if (Selected)
                return AssignedAccountType.HasValue;
            
            return true;
        }
    }
}
