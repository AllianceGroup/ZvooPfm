using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using mPower.Domain.Application.Enums;

namespace Default.Areas.Administration.Models
{
    public class MessageModel
    {
        public string Id { get; set; }

        [Required]
        public string TemplateId { get; set; }

        public Dictionary<string, string> Templates { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Html { get; set; }

        public TemplateStatusEnum Status { get; set; }

        public bool IsUsedInTrigger { get; set; }

        public List<NuggetListItemModel> Nuggets { get; set; }

        public bool IsNew
        {
            get { return string.IsNullOrEmpty(Id); }
        }
    }
}