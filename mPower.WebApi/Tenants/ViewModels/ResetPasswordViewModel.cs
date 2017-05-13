using System.ComponentModel.DataAnnotations;
using Default;

namespace mPower.WebApi.Tenants.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [RegularExpression(Constants.Validation.Regexps.Password, ErrorMessage = Constants.Validation.Messages.InvalidPassword)]
        public string Password { get; set; }

        [Required]
        [RegularExpression(Constants.Validation.Regexps.Password, ErrorMessage = Constants.Validation.Messages.InvalidPasswordConfirmation)]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
