using System.Collections.Generic;
using System.Web.Mvc;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Framework.Mongo;
using mPower.Framework.Services;
using mPower.Web.Admin.Controllers;

namespace mPower.Web.Admin.Models
{
    public class LogsModel
    {
        public IEnumerable<NLogMongoTarget.NlogMongoItem> Logs { get; set; }

		public string LogId { get; set; }

		public string SearchKey { get; set; }

		public List<SelectListItem> DateOptions { get; set; }

		public List<SelectListItem> SortOptions { get; set; }

		public List<SelectListItem> LevelOptions { get; set; }

        public PagingInfo PaginInfo { get; set; }
    }
}