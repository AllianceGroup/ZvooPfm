using System.Web.SessionState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using mPower.Environment.MultiTenancy;
using StructureMap;
using System.Text.RegularExpressions;
using mPower.Framework.Exceptions;

namespace mPower.Framework.Environment.MultiTenancy
{
    /// <summary>
    /// Controller factory that will inject StructureMap dependencies into the constructor of the requested controller type
    /// </summary>
    public class ContainerControllerFactory : IControllerFactory
    {
        /// <summary>
        /// Cache of controller types for container
        /// </summary>
        private readonly ThreadSafeDictionary<IContainer, IDictionary<string, Type>> typeCache;

        /// <summary>
        /// Initializes a new instance of the ContainerControllerFactory class that will initialize
        /// controllers through StructureMap containers
        /// </summary>
        /// <param name="resolver">
        ///     Dependency container resolver from which to pull dependency instances
        /// </param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="resolver"/> is null</exception>
        public ContainerControllerFactory(IContainerResolver resolver)
        {
            Ensure.Argument.NotNull(resolver, "resolver");
            this.ContainerResolver = resolver;
            this.typeCache = new ThreadSafeDictionary<IContainer, IDictionary<string, Type>>();
        }

        /// <summary>
        /// Gets the dependency container resolver used to initialize controllers
        /// </summary>
        public IContainerResolver ContainerResolver { get; private set; }

        /// <summary>
        /// Creates a controller
        /// </summary>
        /// <param name="requestContext">Request context</param>
        /// <param name="controllerName">Controller name</param>
        /// <returns>Controller instance</returns>
        public virtual IController CreateController(RequestContext requestContext, string controllerName)
        {
            var controllerType = this.GetControllerType(requestContext, controllerName);

            if (controllerType == null)
                throw new MpowerNotFoundException(String.Format("The IControllerFactory 'ContainerControllerFactory' did not return a controller for the name '{0}'.", controllerName));

            var controller = this.ContainerResolver.Resolve(requestContext).GetInstance(controllerType) as IController;

            // ensure the action invoker is a ContainerControllerActionInvoker
            if (controller != null && controller is Controller && !((controller as Controller).ActionInvoker is ContainerControllerActionInvoker))
                (controller as Controller).ActionInvoker = new ContainerControllerActionInvoker(this.ContainerResolver, new HandleErrorWithElmahAttribute());

            return controller;
        }

        /// <summary>
        /// Releases a controller instance 
        /// </summary>
        /// <param name="controller">Controller to release</param>
        public void ReleaseController(IController controller)
        {
            if (controller != null && controller is IDisposable)
                ((IDisposable)controller).Dispose();
        }

        /// <summary>
        /// Gets all controller types from the container
        /// </summary>
        /// <param name="container">Container form which to pull controller types</param>
        /// <returns>All controllers from the container</returns>
        public static IEnumerable<Type> GetControllersFor(IContainer container)
        {
            Ensure.Argument.NotNull(container);
            return container.Model.InstancesOf<IController>().Select(x => x.PluginType).Distinct();
        }

        /// <summary>
        /// Gets controller type for request
        /// </summary>
        /// <param name="requestContext">Request context</param>
        /// <param name="controllerName">Controller name</param>
        /// <returns>Controller type for <paramref name="controllerName"/></returns>
        protected virtual Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            Ensure.Argument.NotNull(requestContext, "requestContext");
            Ensure.Argument.NotNullOrEmpty(controllerName, "controllerName");

            var container = this.ContainerResolver.Resolve(requestContext);

            var area = (requestContext.RouteData.Values["area"] ?? String.Empty).ToString().ToLowerInvariant();

            var typeDictionary = this.typeCache.GetOrAdd(container, () => container.GetInstance<ControllersDictionary>().Controllers);

            Type found = null;
            if (typeDictionary.TryGetValue(ControllersDictionary.ControllerFriendlyName(controllerName, area), out found))
                return found;
            return null;
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Default;
        }
    }
}
