using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Web.Mvc;
using mPower.Framework.Environment.MultiTenancy;
using System;
using StructureMap;

namespace mPower.Framework.Utils.Extensions
{
    public static class ControllerExtentions
    {
        public static string GetChargifyUrl(this Controller controller)
        {
            return controller.GetTenant().ChargifyUrl;
        }

        public static string GetChargifySharedKey(this Controller controller)
        {
            return controller.GetTenant().ChargifySharedKey;
        }

        public static string GetChargifyApiKey(this Controller controller)
        {
            return controller.GetTenant().ChargifyApiKey;
        }

        public static string GetApplicationId(this Controller controller)
        {
            return controller.GetTenant().ApplicationId;
        }

        public static string GetApplicationName(this Controller controller)
        {
            return controller.GetTenant().ApplicationName;
        }

        public static string GetJanrainApplicationKey(this Controller controller)
        {
            return controller.GetTenant().JanrainAppApiKey;
        }

        public static string GetJanrainApplicationUrl(this Controller controller)
        {
            return controller.GetTenant().JanrainAppUrl;
        }

        public static string GetZillowWebServiceId(this Controller controller)
        {
            return controller.GetTenant().ZillowWebServiceId;
        }

        public static string GetMembershipApiKey(this Controller controller)
        {
            return controller.GetTenant().MembershipApiKey;
        }

        public static string GetSiteLegalName(this Controller controller)
        {
            return controller.GetTenant().LegalName;
        }

        public static string GetSiteName(this Controller controller)
        {
            return controller.GetTenant().DisplayName;
        }

        public static string GetEmailSuffix(this Controller controller)
        {
            return controller.GetTenant().EmailSuffix;
        }

        public static bool BfmEnabled(this Controller controller)
        {
            return controller.GetTenant().BfmEnabled;
        }

        public static bool PfmEnabled(this Controller controller)
        {
            return controller.GetTenant().PfmEnabled;
        }

        public static bool CreditAppEnabled(this Controller controller)
        {
            return controller.GetTenant().CreditAppEnabled;
        }

        public static bool MarketingEnabled(this Controller controller)
        {
            return controller.GetTenant().MarketingEnabled;
        }

        public static string GetTenantAddress(this Controller controller)
        {
            return controller.GetTenant().Address;
        }

        public static IApplicationTenant GetTenant(this Controller controller)
        {
            return TenantTools.Selector.Select(controller.ControllerContext.RequestContext);
        }

        public static IApplicationTenant GetTenantByMembershipApiKey(this Controller controller, string key)
        {
            return TenantTools.Selector.Tenants.FirstOrDefault(x => x.MembershipApiKey == key);
        }

        public static SmtpClient GetSmtpClient(this Controller controller)
        {
            return controller.GetTenant().Smtp;

        }

        public static void ClearErrorFor<T, TOut>(this Controller controller,T model, Expression<Func<T, TOut>> expression)
        {
            controller.ClearErrorFor(expression);
        }

        public static void ClearError(this Controller controller, string name)
        {
            if (controller.ModelState.ContainsKey(name))
            {
                controller.ModelState[name].Errors.Clear();
            }
        }

        public static void ClearErrorFor<T, TOut>(this Controller controller, Expression<Func<T, TOut>> expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            controller.ClearError(name);
        }

        public static string RenderViewToString(this Controller controller, string viewName, object model = null)
        {
            return MvcUtils.RenderPartialToStringRazor(controller.ControllerContext, viewName, model, controller.ViewData, controller.TempData);
        }
    }
}
