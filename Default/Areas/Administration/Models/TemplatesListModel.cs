using System;
using System.Collections.Generic;
using mPower.Domain.Application.Enums;

namespace Default.Areas.Administration.Models
{
    public class TemplatesListModel
    {
        public List<TemplatesListItemModel> Templates { get; set; }
    }

    public class TemplatesListItemModel
    {
        public string Id { get; set; }

        public DateTime Created { get; set; }

        public string Name { get; set; }

        public TemplateStatusEnum Status { get; set; }

        public bool IsDefault { get; set; }
    }
}