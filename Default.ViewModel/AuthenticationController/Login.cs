using System.ComponentModel.DataAnnotations;
using mPower.Domain.Membership.Enums;

namespace Default.ViewModel.AuthenticationController
{
    public class Login
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "e-mail address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string JanrainAppUrl { get; set; }

        public bool SsoEnabled { get { return !string.IsNullOrEmpty(JanrainAppUrl); } }

        public bool PersistCookie { get; set; }
        public SecurityLevelEnum SecurityLevel { get; set; }
    }

    public class ExtendedLogin : Login
    {
        public string SecurityQuestionAnswer { get; set; }
        public string SecurityQuestion { get; set; }
    }
}
