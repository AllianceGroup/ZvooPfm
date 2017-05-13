using System.ComponentModel.DataAnnotations;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Business.BusinessController
{
    public class CreateLedger
    {
        [Required]
        public string Name { get; set; }

        public LedgerTypeEnum TypeEnum { get; set; }
        [Required]
        public string Street { get; set; }
        
        public string Street2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string TaxId { get; set; }
    }
}