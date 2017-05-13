using System.Collections.Generic;
using System.Web.Mvc;
using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using mPower.Documents.DocumentServices;
using mPower.Framework.Services;
using mPower.Web.Admin.Models;
using mPower.Documents.Documents;
using mPower.Framework.Mvc.Ajax;

namespace mPower.Web.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        private readonly EventLogDocumentService _eventLogService;

        public HomeController(EventLogDocumentService eventLogService)
        {
            _eventLogService = eventLogService;
        }

        public ActionResult Index()
        {
            var model = BuildLogsModel(20, DateTime.MinValue);

            return View(model);
        }

        public ActionResult RefreshItem(long lastDate)
        {
            var model = BuildLogsModel(20, new DateTime(lastDate, DateTimeKind.Local));

            if (model.Logs.Count > 0)
            {
                model.Logs.ForEach(x => { x.IsNew = true; });

                AjaxResponse.Render("#eventsLogContainer", "EventRows", model.Logs, UpdateStyle.Prepend);
                AjaxResponse.AddJsonItem("date", model.Logs[0].StoredDate.Ticks);
            }

            return Result();
        }

        public ActionResult EventDetails(string id)
        {
            var bsonDocument = _eventLogService.GetBsonDocumentById(id);
            var sett = new JsonWriterSettings();
            sett.Indent = true;
            
            return PartialView((object)bsonDocument.ToJson(sett));
        }

        private EventLogsModel BuildLogsModel(int take, DateTime lastEventDate, string userId = null)
        {
            var filter = new EventLogFilter() { PagingInfo = new PagingInfo() { Take = take }, MinStoredDate = lastEventDate, UserId = userId };
            var eventLogs = _eventLogService.GetByFilter(filter);

            var model = new EventLogsModel() { Logs = eventLogs };

            model.LastEventDate = model.Logs.Count > 0 ? model.Logs[0].StoredDate : DateTime.MinValue;

            return model;
        }
    }
}