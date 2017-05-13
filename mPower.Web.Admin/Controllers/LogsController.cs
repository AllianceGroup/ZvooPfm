using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Framework;
using mPower.Framework.Mongo;
using MongoDB.Driver.Builders;
using mPower.Framework.Services;
using mPower.Framework.Utils;
using mPower.Web.Admin.Models;

namespace mPower.Web.Admin.Controllers
{
    public class LogsController : BaseAdminController
    {
        private readonly MPowerSettings _settings;
        private readonly NLogMongoService _logService;
        private Size _pdfPageBounds = new Size(300, 500);
        private int _pdfMaxLogsCount = 500000;

        public LogsController(MPowerSettings settings)
        {
            _settings = settings;
            _logService = new NLogMongoService(new MongoLog(settings));
        }

        public ActionResult Index(LogsGridFilter filter)
        {
            filter = filter ?? new LogsGridFilter();
            var model = BuildLogsModel(filter);
            return View(model);
        }

        public ActionResult Download(LogsGridFilter filter)
        {
            filter.PageSize = _pdfMaxLogsCount;
            filter.PageNr = 1;
            var model = BuildLogsModel(filter);
            var writer = new HtmlToPdfWriter(_settings);
            var html = MvcUtils.RenderPartialToStringRazor(ControllerContext, "Pdf", model, ViewData, TempData);
            var stream = new MemoryStream();
            writer.GeneratePdf(html,stream,_pdfPageBounds);
            return File(stream, "application/pdf");
        }

        private LogsModel BuildLogsModel(LogsGridFilter input)
        {
            // Search
            var filter = new NLogMongoFilter()
                         {
                             SearchKey = input.SearchKey
                         };
            // Date
            if (input.Date != LogDayFilterEnum.Any)
            {
                DateTime minDate, maxDate;
                GetDateRange(input.Date, out minDate, out maxDate);
                filter.MinDate = minDate;
                filter.MaxDate = maxDate;
            }
            // Level
            if (input.Level != LogLevelEnum.Any)
            {
                filter.Level = Enum.GetName(typeof(LogLevelEnum), input.Level);
            }
            // Sort
            switch (input.Sort)
            {
                case LogSortEnum.Date:
                    filter.SortExpression = "Date";
                    break;
                case LogSortEnum.Id:
                    filter.SortExpression = "Id";
                    break;
                case LogSortEnum.Level:
                    filter.SortExpression = "Level";
                    break;
                case LogSortEnum.Message:
                    filter.SortExpression = "LogMessage";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("sort");
            }
            var totalCount = (int)_logService.Count(filter);
            filter.PagingInfo = new PagingInfo()
                                    {
                                        CurrentPage = input.PageNr > 0 ? input.PageNr : 1,
                                        ItemsPerPage = input.PageSize,
                                        TotalCount = totalCount
                                    };

            var items = _logService.GetByFilter(filter);
            LogsModel model = FillModel(items, filter.PagingInfo, input);
            return model;
        }

        private void GetDateRange(LogDayFilterEnum date, out DateTime fromDate, out DateTime toDate)
        {
            fromDate = DateTime.MinValue;
            toDate = DateTime.MaxValue;

            switch (date)
            {
                case LogDayFilterEnum.Today:
                    fromDate = DateUtil.GetStartOfDay(DateTime.Now);
                    toDate = DateUtil.GetEndOfDay(DateTime.Now);
                    break;
                case LogDayFilterEnum.Yesterday:
                    fromDate = DateUtil.GetStartOfDay(DateTime.Now.AddDays(-1));
                    toDate = DateUtil.GetEndOfDay(DateTime.Now.AddDays(-1));
                    break;
                case LogDayFilterEnum.ThisWeek:
                    fromDate = DateUtil.GetStartOfCurrentWeek();
                    toDate = DateUtil.GetEndOfCurrentWeek();
                    break;
                case LogDayFilterEnum.LastWeek:
                    fromDate = DateUtil.GetStartOfLastWeek();
                    toDate = DateUtil.GetEndOfLastWeek();
                    break;
                case LogDayFilterEnum.ThisMonth:
                    fromDate = DateUtil.GetStartOfCurrentMonth();
                    toDate = DateUtil.GetEndOfCurrentMonth();
                    break;
                case LogDayFilterEnum.LastMonth:
                    fromDate = DateUtil.GetStartOfLastMonth();
                    toDate = DateUtil.GetEndOfLastMonth();
                    break;
            }
        }

        public LogsModel FillModel(IEnumerable<NLogMongoTarget.NlogMongoItem> logs, PagingInfo pagingInfo, LogsGridFilter filter)
        {
            var model = new LogsModel();
            model.Logs = logs;
            model.PaginInfo = pagingInfo;
            model.SearchKey = filter.SearchKey;

            model.DateOptions = new List<SelectListItem>();
            model.DateOptions.Add(new SelectListItem() { Text = "[Any]", Value = "1" });
            model.DateOptions.Add(new SelectListItem() { Text = "Today's", Value = "2" });
            model.DateOptions.Add(new SelectListItem() { Text = "Yesterday's", Value = "3" });
            model.DateOptions.Add(new SelectListItem() { Text = "This Week's", Value = "4" });
            model.DateOptions.Add(new SelectListItem() { Text = "Last Week's", Value = "5" });
            model.DateOptions.Add(new SelectListItem() { Text = "This Month's", Value = "6" });
            model.DateOptions.Add(new SelectListItem() { Text = "Last Month's", Value = "7" });
            foreach (var item in model.DateOptions)
            {
                if (item.Value == ((int)filter.Date).ToString())
                    item.Selected = true;
            }

            model.SortOptions = new List<SelectListItem>();
            model.SortOptions.Add(new SelectListItem() { Text = "Date", Value = "1" });
            model.SortOptions.Add(new SelectListItem() { Text = "Id", Value = "2" });
            model.SortOptions.Add(new SelectListItem() { Text = "Level", Value = "3" });
            model.SortOptions.Add(new SelectListItem() { Text = "Message", Value = "4" });
            foreach (var item in model.SortOptions)
            {
                if (item.Value == ((int)filter.Sort).ToString())
                    item.Selected = true;
            }

            model.LevelOptions = new List<SelectListItem>();
            model.LevelOptions.Add(new SelectListItem() { Text = "[Any]", Value = "1" });
            model.LevelOptions.Add(new SelectListItem() { Text = "Warn", Value = "2" });
            model.LevelOptions.Add(new SelectListItem() { Text = "Info", Value = "3" });
            model.LevelOptions.Add(new SelectListItem() { Text = "Error", Value = "4" });
            model.LevelOptions.Add(new SelectListItem() { Text = "Fatal", Value = "5" });
            model.LevelOptions.Add(new SelectListItem() { Text = "Trace", Value = "6" });
            model.LevelOptions.Add(new SelectListItem() { Text = "Debug", Value = "7" });
            model.LevelOptions.Add(new SelectListItem() { Text = "Off", Value = "8" });
            foreach (var item in model.LevelOptions)
            {
                if (item.Value == ((int)filter.Level).ToString())
                    item.Selected = true;
            }

            return model;
        }
    }

    public class LogsGridFilter
    {
        public string SearchKey { get; set; }
        public LogDayFilterEnum Date { get; set; }
        public LogSortEnum Sort { get; set; }
        public LogLevelEnum Level { get; set; }
        public int PageNr { get; set; }
        public int PageSize { get; set; }

        public LogsGridFilter()
        {
            SearchKey = "";
            Date = LogDayFilterEnum.Any;
            Sort = LogSortEnum.Date;
            Level = LogLevelEnum.Any;
            PageNr = 0;
            PageSize = 20;
        }
    }

    public enum LogSortEnum
    {
        Date = 1,
        Id,
        Level,
        Message,
    }

    public enum LogDayFilterEnum
    {
        Any = 1,
        Today,
        Yesterday,
        ThisWeek,
        LastWeek,
        ThisMonth,
        LastMonth
    }

    public enum LogLevelEnum
    {
        Any = 1,
        Warn,
        Info,
        Error,
        Fatal,
        Trace,
        Debug,
        Off
    }
}
