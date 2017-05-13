using System.ComponentModel.DataAnnotations;

namespace Default.Areas.Administration.Models
{
   public class FaqModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Html { get; set; }

        public bool IsActive { get; set; }
    }
}
