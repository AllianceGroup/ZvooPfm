using StructureMap;

namespace mPower.Environment.MultiTenancy
{
    using System.Web.Routing;
    

    /// <summary>
    /// Resolver for dependency container
    /// </summary>
    public interface IContainerResolver
    {
        /// <summary>
        /// Resolves container based upon request context
        /// </summary>
        /// <param name="context">Requesting context</param>
        /// <returns>Dependency container for the request</returns>
        IContainer Resolve(RequestContext context);
    }
}