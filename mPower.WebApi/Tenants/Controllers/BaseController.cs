using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using Microsoft.AspNetCore.Mvc;
using Paralect.Domain;

namespace mPower.WebApi.Tenants.Controllers
{
    public class BaseController : Controller
    {
        public ICommandService CommandService { get; set; }
        public IApplicationTenant Tenant { get; set; }

        public BaseController(ICommandService command, IApplicationTenant tenant)
        {
            CommandService = command;
            Tenant = tenant;
        }

        protected string GetUserId()
        {
            return HttpContext.User.Identity.Name;
        }

        protected string GetLedgerId()
        {
            return HttpContext.Request.Headers["LedgerId"];
        }

        protected bool GetAggregationLoggingEnabled()
        {
            return true;
        }

        public string AppName { get; set; }


        protected virtual void Send(params ICommand[] commands)
        {
            foreach (var command in commands)
            {
                command.Metadata.UserId = GetUserId();
            }
            CommandService.Send(commands);
        }
    }
}