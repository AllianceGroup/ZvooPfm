using System;
using NLog;
using mPower.Framework;

namespace mPower.Schedule.Server.Environment
{
    /// <summary>
    /// Factory class to create Quartz server implementations from.
    /// </summary>
    public class QuartzServerFactory
    {
        private static readonly Logger Logger = MPowerLogManager.CurrentLogger;

        /// <summary>
        /// Creates a new instance of an Quartz.NET server core.
        /// </summary>
        /// <returns></returns>
        public static IQuartzServer CreateServer()
        {
            string typeName = Configuration.ServerImplementationType;

            Type t = Type.GetType(typeName, true);

            Logger.Debug("Creating new instance of server type '" + typeName + "'");
            var retValue = (IQuartzServer) Activator.CreateInstance(t);
            Logger.Debug("Instance successfully created");
            return retValue;
        }
    }
}