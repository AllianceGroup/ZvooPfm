using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using mPower.Framework.Environment.MultiTenancy;

namespace mPower.Framework.Utils.Security
{
    public class DecryptAttribute : ActionFilterAttribute
    {
        private readonly IEncryptionService _encrypter;

        public List<string> ParametersNames { get; private set; }

        public DecryptAttribute()
        {
            ParametersNames = new List<string>();
            _encrypter = TenantTools.Selector.TenantsContainer.GetInstance<IEncryptionService>();
        }

        public DecryptAttribute(params string[] parametersNames)
            : this()
        {
            ParametersNames.AddRange(parametersNames);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var args = filterContext.ActionParameters;
            if (args.Count <= 0) return;
            
            foreach (var argName in ParametersNames.Where(args.ContainsKey))
            {
                if (args[argName] == null)
                    return;
                args[argName] = _encrypter.Decode(args[argName].ToString());
            }
        }
    }
}