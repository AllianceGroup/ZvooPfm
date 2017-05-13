﻿
using StructureMap;

namespace mPower.Framework.Environment.MultiTenancy
{
    /// <summary>
    /// Filter that needs a dependency container injected
    /// </summary>
    public interface IContainerFilter
    {
        /// <summary>
        /// Gets or sets the dependency container used by the filter execution
        /// </summary>
        IContainer DependencyContainer { get; set; }
    }
}