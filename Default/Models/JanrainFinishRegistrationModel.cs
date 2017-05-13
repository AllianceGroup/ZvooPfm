using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Default.Models
{
    public class JanrainFinishRegistrationModel
    {
        [Required]
        [RegularExpression(@"^([\w-_]+\.)*[\w-_]+@([\w-_]+\.)*[\w-_]+\.[\w-_]+$", ErrorMessage = "Please input valid email address")]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }
    }
}