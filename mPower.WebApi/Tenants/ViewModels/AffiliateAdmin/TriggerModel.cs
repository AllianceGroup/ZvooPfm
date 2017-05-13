using System.ComponentModel.DataAnnotations;
using mPower.Domain.Application.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.ViewModels.AffiliateAdmin
{
    public class TriggerModel
    {
        public EmailTypeEnum Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string MessageId { get; set; }

        public TriggerStatusEnum Status { get; set; }

        public SelectList MessagesList { get; set; }

        public SelectList StatusesList { get; set; }
    }
}