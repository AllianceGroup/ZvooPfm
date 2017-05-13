using System.Collections.Generic;
using System.Web.Mvc;

namespace Default.Areas.Administration.Models
{
    public class CreateCampaignModel
    {
        public IEnumerable<SelectListItem> Segments { get; set; }

        public string SegmentId { get; set; }
    }
}