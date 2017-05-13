using System.Web.Mvc;

namespace Default.Areas.Credit
{
	public class CreditAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Credit";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Credit_default",
				"Credit/{controller}/{action}/{id}",
				new { action = "Dashboard", controller="Score", area="Credit", id = UrlParameter.Optional }
			);
		}
	}
}
