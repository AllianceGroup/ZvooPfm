using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Default.Areas.Administration.Models
{
    public class UserModel
    {
        public string UserId { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Please, provide valid email")]
        public string Email { get; set; }


        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }

        public string AffiliateId { get; set; }

        public bool AffiliateAdminView { get; set; }

        public bool AffiliateAdminEdit { get; set; }

        public bool AffiliateAdminDelete { get; set; }

        public bool GlobalAdminView { get; set; }

        public bool GlobalAdminEdit { get; set; }

        public bool GlobalAdminDelete { get; set; }

        public bool Agent { get; set; }

        public Dictionary<string, string> Affiliates { get; set; }

        public List<CreditIndentityModel> CreditIdentityDocuments { get; set; }
    }
}