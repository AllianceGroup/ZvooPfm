using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson.Serialization;
using mPower.Documents.DocumentServices;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.DocumentServices;
using mPower.Web.Admin.Models;

namespace mPower.Web.Admin.Controllers
{
    public class EventsLogController : BaseAdminController
    {
        private readonly EventLogDocumentService _eventLogService;

        public EventsLogController(EventLogDocumentService eventLogService)
        {
            _eventLogService = eventLogService;
        }

        public ActionResult Index()
        {
            var filter = new EventLogFilter() {PagingInfo = new PagingInfo() {Take = 30}};
            var eventLogs = _eventLogService.GetByFilter(filter);
            var model = new EventLogsModel() { Logs = eventLogs };

            return View(model);
        }

    }
}
