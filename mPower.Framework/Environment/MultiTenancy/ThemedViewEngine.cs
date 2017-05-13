using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.IO;
using mPower.Framework.Utils.Extensions;

namespace mPower.Framework.Environment.MultiTenancy
{
    public class ThemedViewEngine : System.Web.Mvc.RazorViewEngine
    {
        private readonly ITenantSelector _tenantSelector;

        #region Constructors
        public ThemedViewEngine(ITenantSelector tenantSelector)
        {
            _tenantSelector = tenantSelector;

            // Search paths for the views
            ViewLocationFormats = new[]
                                       {
                                             "~/Tenants/{2}/Areas/{3}/Views/{1}/{0}.cshtml",
                                             "~/Tenants/{2}/Areas/{3}/Views/Shared/{0}.cshtml",
                                             "~/Tenants/{2}/Views/{1}/{0}.cshtml",
                                             "~/Tenants/{2}/Views/Shared/{0}.cshtml",
                                             
											 "~/TenantsContent/{2}/Areas/{3}/Views/{1}/{0}.cshtml",
                                             "~/TenantsContent/{2}/Areas/{3}/Views/Shared/{0}.cshtml",
                                             "~/TenantsContent/{2}/Views/{1}/{0}.cshtml",
                                             "~/TenantsContent/{2}/Views/Shared/{0}.cshtml",

                                             "~/Tenants/Default/Areas/{3}/Views/{1}/{0}.cshtml",
                                             "~/Tenants/Default/Areas/{3}/Views/Shared/{0}.cshtml",
                                             "~/Tenants/Default/Views/{1}/{0}.cshtml",
                                             "~/Tenants/Default/Views/Shared/{0}.cshtml",
                                             
                                             "~/Views/{1}/{0}.cshtml", //Added for resharper only.  Resharper uses this entry for view resolution
                                             "~/Views/Shared/{0}.cshtml", //Added for resharper only. Resharper uses this for view resolution
                                       };

            // Search parts for the partial views
            // The search parts for the partial & master views are the same as the regular views
            PartialViewLocationFormats = base.ViewLocationFormats;
            MasterLocationFormats = ViewLocationFormats;
        }
        #endregion

        #region Helper Methods

        private static string GetTheme()
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                throw new InvalidOperationException("Http Context cannot be null.");
            }

            string domain = context.Request.Url.GetDomain();
            string cacheKey = String.Format(CultureInfo.InvariantCulture, "theme_for_{0}", domain);
            string theme = (string)context.Cache[cacheKey];
            return theme;
        }

        private string GetPath(ControllerContext controllerContext, string[] locations, string name,
            string tenant, string controller, string area, string cacheKeyPrefix, bool useCache, out string[] searchedLocations)
        {
            searchedLocations = new string[] { };

            if (string.IsNullOrEmpty(name))
                return string.Empty;

            if ((locations == null) || (locations.Length == 0))
                throw new InvalidOperationException("locations must not be null or emtpy.");


            bool isSpecificPath = IsSpecificPath(name);

            string key = this.CreateCacheKey(cacheKeyPrefix, name, isSpecificPath ? string.Empty : controller, tenant, area);

            if (useCache)
            {
                string viewLocation = this.ViewLocationCache.GetViewLocation(controllerContext.HttpContext, key);

                if (viewLocation != null)
                    return viewLocation;

            }

            if (!isSpecificPath)
                return this.GetPathFromGeneralName(controllerContext, locations, name, controller, tenant, area, key, ref searchedLocations);

            return this.GetPathFromSpecificName(controllerContext, name, key, ref searchedLocations);
        }

        private static bool IsSpecificPath(string name)
        {
            char firstCharacter = name[0];
            if (firstCharacter != '~')
            {
                return (firstCharacter == '/');
            }
            return true;
        }

        private string CreateCacheKey(string prefix, string name, string controllerName, string tenant, string area)
        {
            return string.Format(CultureInfo.InvariantCulture, ":ViewCacheEntry:{0}:{1}:{2}:{3}:{4}:{5}",
                new object[] { base.GetType().AssemblyQualifiedName, prefix, name, controllerName, tenant, area });
        }

        private string GetPathFromGeneralName(ControllerContext controllerContext, string[] locations, string name,
            string controller, string theme, string area, string cacheKey, ref string[] searchedLocations)
        {
            string virtualPath = string.Empty;
            searchedLocations = new string[locations.Length];
            for (int i = 0; i < locations.Length; i++)
            {
                string path = string.Format(CultureInfo.InvariantCulture, locations[i], new object[] { name, controller, theme, area });

                if (this.FileExists(controllerContext, path))
                {
                    searchedLocations = new string[] { };
                    virtualPath = path;
                    this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
                    return virtualPath;
                }
                searchedLocations[i] = path;
            }
            return virtualPath;
        }

        private string GetPathFromSpecificName(ControllerContext controllerContext, string name, string cacheKey, ref string[] searchedLocations)
        {
            string virtualPath = name;
            if (!this.FileExists(controllerContext, name))
            {
                virtualPath = string.Empty;
                searchedLocations = new[] { name };
            }
            this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, cacheKey, virtualPath);
            return virtualPath;
        }

        #endregion

        #region Override Default Behavior

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            try
            {
                return File.Exists(controllerContext.HttpContext.Server.MapPath(virtualPath));
            }
            catch (HttpException exception)
            {
                if (exception.GetHttpCode() != 0x194)
                {
                    throw;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            if (string.IsNullOrEmpty(partialViewName))
                throw new ArgumentException("partialViewName");

            string[] strArray;

            var tenant = _tenantSelector.Select(controllerContext.RequestContext).ApplicationName;

            string requiredString = controllerContext.RouteData.GetRequiredString("controller");
            var area = controllerContext.RouteData.Values["area"];
            var areaString = area == null ? null : area.ToString();

            string partialViewPath = this.GetPath(controllerContext, this.PartialViewLocationFormats, partialViewName, tenant, requiredString, areaString, "Partial", useCache, out strArray);

            if (string.IsNullOrEmpty(partialViewPath))
            {
                return new ViewEngineResult(strArray);
            }
            return new ViewEngineResult(this.CreatePartialView(controllerContext, partialViewPath), this);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            if (string.IsNullOrEmpty(viewName))
                throw new ArgumentException("viewName");


            string[] strArray;

            var tenant = _tenantSelector.Select(controllerContext.RequestContext).ApplicationName;

            string requiredString = controllerContext.RouteData.GetRequiredString("controller");
            var area = controllerContext.RouteData.Values["area"];
            var areaString = area == null ? null : area.ToString();

            string viewPath = this.GetPath(controllerContext, this.ViewLocationFormats,
                    viewName, tenant, requiredString, areaString, "View", useCache, out strArray);


            if (!string.IsNullOrEmpty(viewPath))
            {
                return new ViewEngineResult(this.CreateView(controllerContext, viewPath, null), this);
            }

            return new ViewEngineResult(strArray);
        }

        public string FindViewPath(ControllerContext controllerContext, string viewName, bool useCache = true)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            if (string.IsNullOrEmpty(viewName))
                throw new ArgumentException("viewName");


            string[] strArray;

            var tenant = _tenantSelector.Select(controllerContext.RequestContext).ApplicationName;

            string requiredString = controllerContext.RouteData.GetRequiredString("controller");

            var area = controllerContext.RouteData.Values["area"];
            var areaString = area == null ? null : area.ToString();

            string viewPath = this.GetPath(controllerContext, this.ViewLocationFormats,
                    viewName, tenant, requiredString, areaString, "View", useCache, out strArray);


            if (!string.IsNullOrEmpty(viewPath))
            {
                return viewPath;
            }

            throw new Exception("Path Not Found" + strArray);
        }

        #endregion
    }

}
