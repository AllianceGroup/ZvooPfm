using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using mPower.Domain.Accounting;

namespace Default.Areas.Administration.Models
{
    public class ProfileModel
    {
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [RegularExpression(Constants.Validation.Regexps.Email, ErrorMessage = Constants.Validation.Messages.InvalidEmail)]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Logo/Image")]
        public string Logo { get; set; }

        public string Address { get; set; }

        public string Category { get; set; }


        public HttpPostedFileBase LogoFile { get; set; }

        public List<RootExpenseAccount> SpendingCategories { get; set; }
    }
}