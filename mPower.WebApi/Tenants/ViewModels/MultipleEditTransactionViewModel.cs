using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mPower.WebApi.Tenants.ViewModels
{
    public class MultipleEditTransactionViewModel
    {
        public List<string> Transactions { get; set; }
        [Required]
        public string AccountId { get; set; }
        public string Memo { get; set; }
    }
}