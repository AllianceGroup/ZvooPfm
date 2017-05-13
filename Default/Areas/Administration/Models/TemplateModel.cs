using System.ComponentModel.DataAnnotations;

namespace Default.Areas.Administration.Models
{
    public class TemplateModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Html { get; set; }

        public bool IsNew
        {
            get { return string.IsNullOrEmpty(Id); }
        }
    }
}