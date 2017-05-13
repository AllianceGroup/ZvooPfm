using System;
using System.Globalization;
using System.IO;
using System.Web;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Utils.Extensions;
using StructureMap;

namespace mPower.Framework.Modules
{
    public class ThemeHttpModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.BeginRequest += (new EventHandler(this.Application_BeginRequest));
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
           
            if (application.Context.Cache == null)
                return;
            
            
            var baseUrl = application.Context.Request.Url.BaseUrl().TrimEnd('/');
            
            if (baseUrl.Contains("localhost"))
            {
                if (application.Context.Request.Cookies["EmulationUrl"] != null)
                    baseUrl = application.Context.Request.Cookies["EmulationUrl"].Value;
            }

            var cacheKey = String.Format(CultureInfo.InvariantCulture, "content_for_{0}", baseUrl);

            if (application.Context.Cache[cacheKey] == null)
            {
                var tenant = TenantTools.Selector.Select(application.Request.RequestContext);
                application.Context.Cache[cacheKey] = String.Format("~/TenantsContent/{0}/content/", tenant.ApplicationName);

            }
        }

        public void Dispose() { }
    }
}