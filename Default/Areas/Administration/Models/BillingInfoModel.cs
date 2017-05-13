using System.ComponentModel.DataAnnotations;

namespace Default.Areas.Administration.Models
{
    public class BillingInfoModel
    {
        [Required]
        [Display(Name = "Street Address")]
        public string Street { get; set; }

        [Display(Name = "Street Address2")]
        public string Street2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [Display(Name = "Zip/Postal Code")]
        public string ZipCode { get; set; }


        [Required]
        [Display(Name = "Name on Card")]
        public string NameOnCard { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Expiration Month")]
        public string ExpirationMonth { get; set; }

        [Required]
        [Display(Name = "Expiration Year")]
        public string ExpirationYear { get; set; }

        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; }
    }
}