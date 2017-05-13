using System.Collections.Generic;
using mPower.Documents.Documents;
using mPower.Framework.Mongo;
using mPower.TempDocuments.Server.Documents;

namespace mPower.Web.Admin.Models
{
    public class EventLogsModel
    {
        public List<EventLogDocument> Logs { get; set; }

        public System.DateTime LastEventDate { get; set; }
    }
}