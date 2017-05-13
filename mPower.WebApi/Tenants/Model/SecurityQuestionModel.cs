using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.Model
{
    public class SecurityQuestionModel
    {
        [Required]
        public string SecurityQuestion { get; set; }

        [Required]
        public string Answer { get; set; }

        public SelectList Questions { get; set; }
    }
}