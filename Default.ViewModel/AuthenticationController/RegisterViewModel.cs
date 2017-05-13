using System;
using System.ComponentModel.DataAnnotations;
using mPower.Domain.Application.Enums;

namespace Default.ViewModel.AuthenticationController
{
    public class RegisterViewModel
    {
        [Required]
        [RegularExpression(@"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$", ErrorMessage = "Please, provide valid email")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ProductHandle { get; set; }

        public string ZipCode { get; set; }

        public DateTime BirthDate { get; set; }

        public GenderEnum? Gender { get; set; }
    }
}