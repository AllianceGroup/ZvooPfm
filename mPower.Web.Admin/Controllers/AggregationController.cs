using mPower.Aggregation.Client;
using mPower.Aggregation.Contract.Documents;
using mPower.Framework.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace mPower.Web.Admin.Controllers
{
    public class AggregationController : Controller
    {
        private readonly IAggregationClient _aggregation;

        public AggregationController(IAggregationClient aggregation)
        {
            _aggregation = aggregation;
        }

        [HttpGet]
        public ActionResult Logs(IntuitLogFilter filter)
        {
            filter = filter ?? new IntuitLogFilter();
            filter.PagingInfo = filter.PagingInfo ?? new PagingInfo();
            var response = _aggregation.GetLogs(filter.SearchKey, filter.UserId, filter.ExceptionId, filter.PagingInfo.ItemsPerPage, filter.PagingInfo.CurrentPage);
            filter.PagingInfo.TotalCount = response.TotalCount;
            filter.PagingInfo.ActualLoadedItemCount = response.Items.Count;
            var model = new IntuitLogsModel
            {
                Items = response.Items.Select(Map),
                Filter = filter
            };
            return View(model);
        }

        public IntuitLogItemModel Map(AggregationLogItem doc)
        {
            return new IntuitLogItemModel
            {
                Id = doc.Id,
                UserId = doc.LogonId,
                Date = doc.Date,
                LogMessage = doc.LogMessage,
                ExceptionId = doc.ExceptionId
            };
        }

    }

    public class IntuitLogsModel
    {
        public IntuitLogFilter Filter { get; set; }

        public IEnumerable<IntuitLogItemModel> Items { get; set; }
    }

    public class IntuitLogFilter : BaseFilter
    {
        public string UserId { get; set; }
        public string ExceptionId { get; set; }
        [Display(Name = "Search Key")]
        public string SearchKey { get; set; }
    }

    public class IntuitLogItemModel
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string LogMessage { get; set; }

        public string UserId { get; set; }

        public string ExceptionId { get; set; }
    }
}
