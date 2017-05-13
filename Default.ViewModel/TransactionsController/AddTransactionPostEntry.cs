using System;
using System.ComponentModel.DataAnnotations;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.TransactionsController
{
    public class AddTransactionPostEntry
    {  
            
        [Required]
        public String AccountId { get; set; }
        public String Payee { get; set; }
            
        [Required]
        public Int64 Amount { get; set; }
            
        public String Memo { get; set; }
            
        [Required]
        public AmountTypeEnum AmountType { get; set; }
        
    }
}