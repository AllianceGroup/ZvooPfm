using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mPower.Domain.Membership.Enums;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Exceptions;
using mPower.Documents.DocumentServices.Membership;

namespace Default
{
    public class ApplicationAttribute : ActionFilterAttribute
    {
        private readonly ApplicationsEnum _application;

        private SessionContext _sessionContext { get; set; }

        public ApplicationAttribute(ApplicationsEnum application)
        {
            this._application = application;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var tenant = TenantTools.Selector.Select(filterContext.RequestContext);

            switch (_application)
            {
                    case ApplicationsEnum.Pfm:
                        if(!tenant.PfmEnabled)
                            throw new MpowerSecurityException("Access denied");
                    break;
                    case ApplicationsEnum.Bfm:
                        if (!tenant.BfmEnabled)
                            throw new MpowerSecurityException("Access denied");
                        break;
                    case ApplicationsEnum.CreditApp:
                        if (!tenant.CreditAppEnabled)
                            throw new MpowerSecurityException("Access denied");
                        break;
            }

        }


    }
}