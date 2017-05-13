using System.ComponentModel.DataAnnotations;

namespace Default.ViewModel
{
    public class Loan
    {
        [Required]
        public double Amount { get; set; }

        [Required]
        public double Payment { get; set; }

        [Required]
        public double Interest { get; set; }
    }
}
