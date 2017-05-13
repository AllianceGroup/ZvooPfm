using System.ComponentModel.DataAnnotations;

namespace Default.ViewModel.Areas.Business.BusinessController
{
    public class UpdateAccountViewModel
    {

        [Required]
        public string LedgerId { get; set; }
        [Required]
        public string AccountId { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Label { get; set; }
        [Required]
        public string Number { get; set; }

        public string ParentAccountId { get; set; }
    }
}
