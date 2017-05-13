using System;
using System.Collections.Generic;

namespace Default.Areas.Administration.Models
{
    public class FaqListModel
    {
        public List<FaqListItemModel> FaqList { get; set; }
    }

    public class FaqListItemModel
    {
        public string Id { get; set; }

        public DateTime Created { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string Html { get; set; }
    }
}
