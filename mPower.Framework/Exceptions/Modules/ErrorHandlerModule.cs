using System;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using NLog;
using mPower.Framework.Exceptions;
using mPower.Framework.Environment;

namespace mPower.Framework.Modules
{
    public class ErrorHandlerModule : IHttpModule
    {
        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication application)
        {
            application.Error += OnError;
        }

        private void OnError(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            Exception exception = application.Server.GetLastError();

            if (exception is MpowerSecurityException)
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            else if(exception is MpowerNotFoundException)
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            
            var idGenerator = new MongoObjectIdGenerator();

            if (HttpContext.Current.Cache != null)
            {
                HttpContext.Current.Cache["ErrorId"] = idGenerator.Generate();
            }
            HttpContext.Current.Response.TrySkipIisCustomErrors = true;

            if (exception is HttpRequestValidationException || exception is HttpAntiForgeryException)
            {
                // in any case, ((HttpException)exception).GetHttpCode() will return 500
                // that's why for changing destination error page we have to redirect manually
                RedirectToErrorPage((int)HttpStatusCode.Forbidden, exception);
            }
        }

        #region Helper Methods

        private static void RedirectToErrorPage(int statusCode, Exception exception = null)
        {
            var httpContext = HttpContext.Current;

            if (!httpContext.IsCustomErrorEnabled)
            {
                return;
            }

            if (exception != null)
            {
                LogException(exception);
            }

            var redirectUrl = GetRedirectUrl(statusCode);

            httpContext.Response.Clear();
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.TrySkipIisCustomErrors = true;
            httpContext.ClearError();

            if (!string.IsNullOrEmpty(redirectUrl))
            {
                httpContext.Response.Redirect(redirectUrl);
            }
        }

        private static void LogException(Exception exception)
        {
            var logger = MPowerLogManager.CurrentLogger;
            logger.Warn(String.Format("{1} exception type: {0}", exception.GetType(), exception.Message));
        }

        private static string GetRedirectUrl(int statusCode)
        {
            var redirectUrl = string.Empty;
            var errorsSection = WebConfigurationManager.GetSection("system.web/customErrors") as CustomErrorsSection;
            if (errorsSection != null)
            {
                redirectUrl = errorsSection.DefaultRedirect;

                if (errorsSection.Errors.Count > 0)
                {
                    var item = errorsSection.Errors[statusCode.ToString(CultureInfo.InvariantCulture)];

                    if (item != null)
                    {
                        redirectUrl = item.Redirect;
                    }
                }
            }
            return redirectUrl;
        }

        #endregion
    }
}
