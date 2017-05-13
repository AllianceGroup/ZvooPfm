using System.Collections.Generic;
using mPower.Domain.Application.Enums;

namespace Default.Areas.Administration.Models
{
    public class TriggersListModel
    {
        public List<TriggersListItemModel> Triggers { get; set; }
    }

    public class TriggersListItemModel
    {
        public EmailTypeEnum Id { get; set; }

        public string Name { get; set; }

        public TriggerStatusEnum Status { get; set; }
    }
}