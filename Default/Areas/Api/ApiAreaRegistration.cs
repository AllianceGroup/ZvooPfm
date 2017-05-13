using System.Web.Mvc;

namespace Default.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Api_default",
                "api/{controller}/{action}/{id}",
                new { action = "Index", area = "api", id = UrlParameter.Optional }
            );
        }
    }
}
