using System.Collections.Generic;
using mPower.Documents.Documents;
using mPower.Documents.Documents.Membership;
using mPower.Framework.Services;

namespace Default.Areas.Administration.Models
{
    public class ActivityListModel
    {
        public PagingInfo Paging { get; set; }

        public List<EventLogDocument> Activities { get; set; }

        public string UserFullName { get; set; }

        public string UserId { get; set; }
    }
}
