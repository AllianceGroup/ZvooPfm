using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using StructureMap;
using mPower.Environment.MultiTenancy;

namespace mPower.Framework.Environment.MultiTenancy
{
    public class ControllersDictionary
    {
        private Dictionary<string, Type> _controllers = new Dictionary<string, Type>();

        public Dictionary<string, Type> Controllers
        {
            get { return _controllers; }
        }

        /// <summary>
        /// Build dictionary where key ControllerName plus AreaName
        /// </summary>
        /// <param name="container"></param>
        public void BuildDictionary(IContainer container)
        {
            _controllers = GetControllersFor(container).ToDictionary(x => ControllerFriendlyName(x.Name, TryGetAreaName(x.FullName)));
        }

        /// <summary>
        /// Gets all controller types from the container
        /// </summary>
        /// <param name="container">Container form which to pull controller types</param>
        /// <returns>All controllers from the container</returns>
        private IEnumerable<Type> GetControllersFor(IContainer container)
        {
            Ensure.Argument.NotNull(container);

            var controllers = container.Model.InstancesOf<IController>().Select(x => x.PluginType).Distinct();

            return controllers;
        }

      

        /// <summary>
        /// Gets controller name without controller and lowercase
        /// </summary>
        /// <param name="typeName">Possible controller name</param>
        /// <param name="area">Possible area name</param>
        /// <returns>Name without controller + area name, lowercase</returns>
        public static string ControllerFriendlyName(string typeName, string area)
        {
            var controllerName = (typeName ?? string.Empty).ToLowerInvariant().Without("controller");
            return controllerName + area;
        }

        /// <summary>
        /// Try get area name from namespace
        /// </summary>
        /// <param name="fulltypeName"></param>
        /// <returns>area name, lowercase</returns>
        public static string TryGetAreaName(string fulltypeName)
        {
            var area = String.Empty;
            if (fulltypeName.Contains("Areas"))
            {
                var regexp = new Regex(@"\.areas\.([^\.]+)\.", RegexOptions.IgnoreCase);
                var match = regexp.Match(fulltypeName);
                area = match.Value.Replace("Areas", String.Empty).Replace(".", String.Empty).ToLowerInvariant();
            }

            return area;
        }

    }
}
