using System.Web.Mvc;
using mPower.Framework.Environment.MultiTenancy;

namespace Default
{
    /// <summary>
    /// Need to be added to all "landing" controllers to activate referral codes handling
    /// </summary>
    public class ReferralProgramAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var referralCode = filterContext.HttpContext.Request.QueryString["rc"];

            if (!string.IsNullOrEmpty(referralCode))
            {
                var sessionContext = TenantTools.Selector.TenantsContainer.GetInstance<SessionContext>();

                sessionContext.ReferralCode = referralCode;
            }
        }
    }
}