using System.Web.Mvc;

namespace mPower.Framework.Environment
{
    public class GetStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var key = PassStateAttribute.TempDataTransferKey;
            var modelState = filterContext.Controller.TempData[key] as ModelStateDictionary;

            if (modelState != null)
            {
                //Only Import if we are viewing 
                if (filterContext.Result is ViewResult)
                {
                    filterContext.Controller.ViewData.ModelState.Merge(modelState);
                }
                else if (filterContext.Result is RedirectToRouteResult)
                {
                    // do nothing
                }
                else
                {
                    //Otherwise remove it. 
                    filterContext.Controller.TempData.Remove(key);
                }
            }

            // the same for view bag message
            key = PassStateAttribute.ViewBagMessageKey;
            var message = filterContext.Controller.TempData[key] as string;

            if (!string.IsNullOrEmpty(message))
            {
                //Only Import if we are viewing 
                if (filterContext.Result is ViewResult)
                {
                    filterContext.Controller.ViewBag.Message = message;
                }
                else if (filterContext.Result is RedirectToRouteResult)
                {
                    // do nothing
                }
                else
                {
                    //Otherwise remove it. 
                    filterContext.Controller.TempData.Remove(key);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}