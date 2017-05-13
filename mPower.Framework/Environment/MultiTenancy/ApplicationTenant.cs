using System.Net.Mail;
using StructureMap;
using System.Collections.Generic;
using mPower.Framework.Environment.Sitemap;

namespace mPower.Framework.Environment.MultiTenancy
{
    /// <summary>
    /// Application tenant that defines default behavior for view engine registration
    /// </summary>
    public class ApplicationTenant : IApplicationTenant
    {
        /// <summary>
        /// Gets the name of the application (company name)
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets the name of the application (company name)
        /// </summary>
        public string LegalName { get; set; }

        /// <summary>
        /// Gets the name of the application (company name)
        /// </summary>
        public string EmailSuffix { get; set; }

        /// <summary>
        /// Gets the name of the application (company name)
        /// </summary>
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// Gets the name of the application (company name)
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets the affiliate smtp server
        /// </summary>
        public SmtpClient Smtp { get; set; }

        /// <summary>
        /// Gets the base URL paths the applicaiton should utilize
        /// </summary>
        public IEnumerable<string> UrlPaths { get; set; }

        /// <summary>
        /// Gets the default dependency container
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// The returned container should be the same as the
        /// container as the container in the application settings
        /// </remarks>
        public IContainer DependencyContainer { get; set; }

        /// <summary>
        /// Shared key for the affiliate site on chargify 
        /// </summary>
        public string ChargifySharedKey { get; set; }

        public string ChargifyApiKey { get; set; }

        /// <summary>
        /// Url for the affiliate site on chargify 
        /// </summary>
        /// <remarks>
        /// https://affiliate_domain.chargify.com
        /// </remarks>
        public string ChargifyUrl { get; set; }

        public string ApplicationId { get; set; }

        public string MembershipApiKey { get; set; }

        public string AssemblyName { get; set; }

        public string JanrainAppApiKey { get; set; }

        public string JanrainAppUrl { get; set; }

        public string ZillowWebServiceId { get; set; }

        public bool PfmEnabled { get; set; }

        public bool BfmEnabled { get; set; }

        public bool CreditAppEnabled { get; set; }

        public bool SavingsEnabled { get; set; }

        public bool MarketingEnabled { get; set; }

        public string Address { get; set; }

        public TenantSiteMap SiteMap { get; set; }
    }
}