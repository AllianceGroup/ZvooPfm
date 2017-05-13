using System.Net.Mail;
using StructureMap;
using System.Collections.Generic;
using mPower.Framework.Environment.Sitemap;

namespace mPower.Framework.Environment.MultiTenancy
{

    /// <summary>
    /// Implementers of this interface must specify semantics of the application
    /// </summary>
    /// <remarks>The assembly of the implementer must contain the necessary pieces of the application</remarks>
    public interface IApplicationTenant
    {
        /// <summary>
        /// Gets the name of the application (company name)
        /// </summary>
        string ApplicationName { get; }

        string ApplicationId { get; }

        /// <summary>
        /// Gets the Legal Name of the Tenant (Tenant, llc)
        /// </summary>
        string LegalName { get; }

        /// <summary>
        /// Gets the email suffix of the Tenant (@tenant.com)
        /// </summary>
        string EmailSuffix { get; }

        /// <summary>
        /// Gets the Diplay contact Phone Number
        /// </summary>
        string ContactPhoneNumber { get; }


        /// <summary>
        /// Gets the name to Display for the Tenant
        /// </summary>
        string DisplayName { get; }


        /// <summary>
        /// Gets the smtp client information for the affilaite server
        /// </summary>
        SmtpClient Smtp { get; }

        /// <summary>
        /// Gets the base URL paths the applicaiton should utilize
        /// </summary>
        IEnumerable<string> UrlPaths { get; }

        /// <summary>
        /// Gets the default dependency container
        /// </summary>
        /// <remarks>
        ///     The returned container should be the same as the 
        ///     container as the container in the application settings
        /// </remarks>
        IContainer DependencyContainer { get; }

        /// <summary>
        /// Shared key for the affiliate site on chargify 
        /// </summary>
        string ChargifySharedKey { get; }    
        
        string ChargifyApiKey { get; }

        /// <summary>
        /// Chargify url for the affiliate site on chargify 
        /// </summary>
        string ChargifyUrl { get; }

        /// <summary>
        /// Secret api key of membership api
        /// </summary>
        string MembershipApiKey { get; }

        /// <summary>
        /// Assembly name to scan via ControllerConvention
        /// </summary>
        string AssemblyName { get; }

        /// <summary>
        /// Application Key for Janrain 
        /// </summary>
        string JanrainAppApiKey { get; }

        /// <summary>
        /// Token Url from Janrain
        /// </summary>
        string JanrainAppUrl { get; }

        /// <summary>
        /// Application Id for Zillow Web Service
        /// </summary>
        string ZillowWebServiceId { get; }


        bool PfmEnabled { get;  }

        bool BfmEnabled { get;  }

        bool CreditAppEnabled { get;  }

        bool SavingsEnabled { get;}

        bool MarketingEnabled { get; }

        string Address { get; }

        TenantSiteMap SiteMap { get; }
    }
}