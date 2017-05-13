using System.Web.Mvc;

namespace Default.Areas.Finance
{
    public class FinanceAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Finance";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            /* route to support extension based format */
            context.MapRoute(
                "Finance_RespondToExtension",
                "Finance/{controller}/{action}.{format}/{id}",
                new { controller = "Start", action = "Index", id = UrlParameter.Optional, app = "Finance" }
            );

            context.MapRoute(
               "Finance_Reports",
               "Finance/Reports/{action}/{id}",
               new { controller = "Reports", action = "Index", id = UrlParameter.Optional, area = "", app = "Finance" } // Parameter defaults
             );

            context.MapRoute(
              "Finance_Accounts",
              "Finance/Accounts/{action}/{id}",
              new { controller = "Accounts", action = "Index", id = UrlParameter.Optional, area = "", app = "Finance" } // Parameter defaults
            );

            context.MapRoute(
               "Finance_Transactions",
               "Finance/Transactions/{action}/{id}",
               new { controller = "Transactions", action = "Index", id = UrlParameter.Optional, area = "", app = "Finance" } // Parameter defaults
           );

            context.MapRoute(
           "Finance_Budgets",
           "Finance/Budget/{action}/{id}",
               new { controller = "Budget", action = "Index", id = UrlParameter.Optional, area = "", app = "Finance" } // Parameter defaults
           );

            context.MapRoute(
               "Finance_Trends",
               "Finance/Trends/{action}/{id}",
                   new { controller = "Trends", action = "Index", id = UrlParameter.Optional, area = "", app = "Finance" } // Parameter defaults
               );

            

          //  context.MapRoute(
          //    "Finance_Dashboard", // Route name
          //    "Finance/Dashboard/{action}", // URL with parameters
          //    new { controller = "LedgerDashboard", action = "Index", id = UrlParameter.Optional, area = "", app="Finance" } // Parameter defaults
          //);

            context.MapRoute(
                "Finance_default",
                "Finance/{controller}/{action}/{id}",
                new { action = "Index", controller = "Start", area = "Finance", id = UrlParameter.Optional, app = "Finance" }
            );
        }
    }
}
