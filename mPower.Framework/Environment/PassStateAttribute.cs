using System.Web.Mvc;

namespace mPower.Framework.Environment
{
    public class PassStateAttribute : ActionFilterAttribute
    {
        public const string TempDataTransferKey = "TempDataTransferKey";
        public const string ViewBagMessageKey = "ViewBagMessageKey";

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //Only export when ModelState is not valid 
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                //Export if we are redirecting 
                if ((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult))
                {
                    filterContext.Controller.TempData[TempDataTransferKey] = filterContext.Controller.ViewData.ModelState;
                }
            }
            else if (!string.IsNullOrEmpty(filterContext.Controller.ViewBag.Message))
            {
                filterContext.Controller.TempData[ViewBagMessageKey] = filterContext.Controller.ViewBag.Message;
            }

            base.OnActionExecuted(filterContext);
        }
    }
}