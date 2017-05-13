using System.ComponentModel.DataAnnotations;
using mPower.Domain.Application.Enums;

namespace mPower.WebApi.Tenants.Model.AffiliateAdmin
{
    public class TriggerUpdateModel
    {
        public EmailTypeEnum Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string MessageId { get; set; }

        public TriggerStatusEnum Status { get; set; }
    }
}
