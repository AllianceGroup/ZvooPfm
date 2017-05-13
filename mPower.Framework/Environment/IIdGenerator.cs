using System;

namespace mPower.Framework.Environment
{
    /// <summary>
    /// Id generation
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>
        /// Returns newly generated ID
        /// </summary>
        String Generate();

        
    }
}