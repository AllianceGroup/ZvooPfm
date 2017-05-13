using System.Web.Mvc;
using System.Web.Routing;
using Default.Factories.ViewModels;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using mPower.Framework.Mvc.Validation;
using mPower.OfferingsSystem;

namespace mPower.Web.Admin
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*robotstxt}", new { robotstxt = @"(.*/)?robots.txt(/.*)?" });
            
            routes.MapRoute(
               "TransactionDetail",
               "business/transactiondetail/{id}/{from}/{to}/{p}",
               new
               {
                   controller = "business",
                   action = "transactiondetail",
                   from = "",
                   to = "",
                   p = "0" // show sub accounts or not (0 = false, 1 = true)
               }
           );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                new[] {"mPower.Web.Admin.Controllers"} // namespace
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            // Application bootstrapping

            var container = HttpApplicationStructureMapContext.Current;

            new Bootstrapper().BootstrapStructureMap(container);
            //Scan for IObjectFactory<TInput,TOutput> and IValidator implementations
            var settings = container.GetInstance<MPowerSettings>();
            container.Configure(config =>
            {
                config.AddRegistry<OfferingSystemRegistry>();
                config.Scan(scanner =>
                {
                    scanner.Assembly(settings.DefaultTenantAssembly);
                    scanner.AssemblyContainingType<AccountsSidebarFactory>();
                    scanner.AddAllTypesOf(typeof(IObjectFactory<,>));
                    scanner.AddAllTypesOf(typeof(IValidator<>));

                });
            });

            DependencyResolver.SetResolver(new StructureMapDependencyResolver(HttpApplicationStructureMapContext.Current));
        }
    }
}