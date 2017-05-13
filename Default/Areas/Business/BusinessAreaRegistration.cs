using System.Web.Mvc;

namespace Default.Areas.Api
{
    public class BusinessAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Business";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
             "Business_Reports",
             "Business/Reports/{action}/{id}",
             new { controller = "Reports", action = "Index", id = UrlParameter.Optional, area = "", app = "Business" } // Parameter defaults
           );

            context.MapRoute(
              "Business_Accounts",
              "Business/Accounts/{action}/{id}",
              new { controller = "Accounts", action = "Index", id = UrlParameter.Optional, area = "", app = "Business" } // Parameter defaults
            );

            context.MapRoute(
                "Business_Transactions",
                "Business/Transactions/{action}/{id}",
                new { controller = "Transactions", action = "Index", id = UrlParameter.Optional, area = "", app = "Business" } // Parameter defaults
            );

            context.MapRoute(
              "Business_Budgets",
              "Business/Budget/{action}/{id}",
                  new { controller = "Budget", action = "Index", id = UrlParameter.Optional, area = "", app = "Business" } // Parameter defaults
              );

            context.MapRoute(
               "Business_Trends",
               "Business/Trends/{action}/{id}",
                   new { controller = "Trends", action = "Index", id = UrlParameter.Optional, area = "", app = "Business" } // Parameter defaults
               );

            //context.MapRoute(
            //    "Business_Dashboard", // Route name
            //    "Business/Dashboard/{action}", // URL with parameters
            //    new { controller = "LedgerDashboard", action = "Index", id = UrlParameter.Optional, area = "", app = "Business" } // Parameter defaults
            //);

            context.MapRoute(
                "Business_default",
                "Business/{controller}/{action}/{id}",
                new { action = "Index", controller = "Start", area = "Business", id = UrlParameter.Optional, app = "Business" }
            );
        }
    }
}
