using System;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using NLog.Targets;
using NLog;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace mPower.Framework.Mongo
{
    public class NLogMongoTarget : Target
    {
        public class NlogMongoItem
        {
            [BsonId]
            public string Id { get; set; }

            public DateTime Date { get; set; }

            public string LogMessage { get; set; }

            public string UserId { get; set; }

            public string UserEmail { get; set; }

            public string Level { get; set; }

            public string ExceptionMessage { get; set; }

            public string ExceptionTrace { get; set; }

            public string RequestUrl { get; set; }
        }

        [Target("MongoTarget")]
        public sealed class MongoTarget : TargetWithLayout
        {
            protected override void Write(LogEventInfo logEvent)
            {
                string logMessage = logEvent.FormattedMessage; //this.Layout.Render(logEvent);
                var exceptionMessage = logEvent.Exception != null ? logEvent.Exception.Message : String.Empty;
                var exceptionTrace = logEvent.Exception != null ? logEvent.Exception.ToString() : String.Empty;
                var httpContext = HttpContext.Current;
                var validContext = httpContext != null && httpContext.Session != null;
                var userId = validContext ? (httpContext.Session["UserId"] as String) ?? String.Empty : String.Empty;
                var userEmail = validContext ? (httpContext.Session["UserEmail"] as String) ?? String.Empty : String.Empty;
                var requestUrl = validContext ? httpContext.Request.Url.AbsoluteUri : String.Empty;
                WriteLogToMongo(logMessage, logEvent.Level.Name, exceptionMessage, exceptionTrace, userId, userEmail, requestUrl);
            }

            private void WriteLogToMongo(string logMessage, string level, string exceptionMessage, string exceptionTrace, string userId, string userEmail, string requestUrl)
            {
                var logsService = ServiceLocator.Current.GetInstance<NLogMongoService>();

                var id = HttpContext.Current == null || HttpContext.Current.Cache == null || HttpContext.Current.Cache["ErrorId"] == null
                             ? ObjectId.GenerateNewId().ToString()
                             : HttpContext.Current.Cache["ErrorId"].ToString();
                logsService.Insert(new NlogMongoItem
                {
                    Date = DateTime.Now,
                    Id = id,
                    UserId = userId,
                    UserEmail = userEmail,
                    LogMessage = logMessage,
                    ExceptionMessage = exceptionMessage,
                    ExceptionTrace = exceptionTrace,
                    Level = level,
                    RequestUrl = requestUrl,
                });
            }
        }
    }
}
