using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using mPower.Environment.MultiTenancy;
using StructureMap;

namespace mPower.Framework.Environment.MultiTenancy
{
    /// <summary>
    /// Default tenant selector that will select tenants based on request URL path
    /// </summary>
    public class TenantSelector : ITenantSelector
    {
        /// <summary>
        /// Initializes a new instance of the DefaultTenantSelector class that selects tenants based on the request URL
        /// </summary>
        /// <param name="tenants">Tenants used by the application</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="tenants"/> is null</exception>
        public TenantSelector(IEnumerable<IApplicationTenant> tenants)
        {
            Ensure.Argument.NotNull(tenants, "tenants");
            this.Tenants = tenants;
        }

        /// <summary>
        /// Gets the tenants used by the application
        /// </summary>
        public IEnumerable<IApplicationTenant> Tenants { get; private set; }

        /// <summary>
        /// Selects URL based upon their base URL
        /// </summary>
        /// <param name="context">Context of the request</param>
        /// <returns>Application tenant for the request</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="context"/> is null</exception>
        /// <exception cref="MultiTenancy.Core.Specification.TenantNotFoundException">Thrown when a tenant doesn't match the request</exception>
        public IApplicationTenant Select(RequestContext context)
        {
            Ensure.Argument.NotNull(context, "context");

            string baseurl = context.HttpContext.BaseUrl().TrimEnd('/');


            if (context.HttpContext.Request.Cookies["EmulationUrl"] != null)
                baseurl = context.HttpContext.Request.Cookies["EmulationUrl"].Value.TrimEnd('/');


            var valid = from tenant in this.Tenants
                        from path in tenant.UrlPaths
                        where path.Trim().TrimEnd('/').Equals(baseurl, StringComparison.OrdinalIgnoreCase)
                        select tenant;


            if (!valid.Any())
            {
                valid = from tenant in this.Tenants
                        where tenant.ApplicationName == "Default"
                        select tenant;
            }

            return valid.First();
        }

        /// <summary>
        /// Global container for all tenants
        /// </summary>
        public IContainer TenantsContainer { get; set; }
    }
}