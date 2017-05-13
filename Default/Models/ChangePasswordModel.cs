using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Default.Models
{
    public class ChangePasswordModel
    {
        [Required, DisplayName("Current Password")]
        public string OldPassword { get; set; }

        [Required, DisplayName("New Password")]
        [RegularExpression(Constants.Validation.Regexps.Password, ErrorMessage = Constants.Validation.Messages.InvalidPassword)]
        public string NewPassword { get; set; }

        [Required, DisplayName("Confirm Password")]
        [Compare("NewPassword", ErrorMessage = Constants.Validation.Messages.InvalidPasswordConfirmation)]
        public string NewPasswordConfirmation { get; set; }
    }
}