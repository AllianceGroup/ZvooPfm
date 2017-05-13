using System.Collections.Generic;
using mPower.Environment.MultiTenancy;
using StructureMap.TypeRules;
using System;
using System.Web.Mvc;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace mPower.Framework.Environment.MultiTenancy
{
    /// <summary>
    /// Controller convention that will override a controller if the controller has the same name
    /// </summary>
    public class ControllerConvention : IRegistrationConvention
    {

        private List<Type> _loadedTypes;

        public ControllerConvention()
        {
            _loadedTypes = new List<Type>();
        }

        /// <summary>
        /// Processes types from an assembly for controllers and implements controller overriding behavior
        /// </summary>
        /// <param name="type">Type to process</param>
        /// <param name="registry">Registry on which to register controllers</param>
        public void Process(Type type, Registry registry)
        {
            if (registry == null) return;

            //See if a controller with the same name/area has been previously loaded 
            var types =
                _loadedTypes.FindAll(
                    x =>
                    GetControllerFriendlyName(x.Name, x.FullName) == GetControllerFriendlyName(type.Name, type.FullName));

            if (types.Count > 0) return;

            // Check to see if type is a valid controller
            if (IsValidController(type))
            {
                registry.AddType(typeof(IController), type);
                _loadedTypes.Add(type);
                return;
            }

            //The code below is based on the StructureMap default Convention Scanner
            if (!type.IsConcrete()) return;

            var pluginType = FindPluginType(type);
            //if (pluginType == null || !Constructor.HasConstructors(type)) return;

            registry.AddType(pluginType, type);
            _loadedTypes.Add(type);
        }

        /// <summary>
        /// Returns Type of Type Name matches Interface naming convention
        /// </summary>
        /// <param name="concreteType">Type to check against convention</param>
        /// <returns>Type that matches convention</returns>
        public virtual Type FindPluginType(Type concreteType)
        {
            string interfaceName = "I" + concreteType.Name;
            Type[] interfaces = concreteType.GetInterfaces();
            return Array.Find(interfaces, t => t.Name == interfaceName);
        }


        /// <summary>
        /// Gets whether a type is a valid controller
        /// </summary>
        /// <param name="type">Type to test for controller</param>
        /// <returns>Value indicating whether the type is a controller</returns>
        private static bool IsValidController(Type type)
        {
            return type != null && !type.IsAbstract && typeof(IController).IsAssignableFrom(type) &&
                   type.Name.EndsWith("Controller") && type.IsPublic;
        }

        /// <summary>
        /// Get controller name with area if are exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        private static string GetControllerFriendlyName(string name, string fullName)
        {
             return ControllersDictionary.ControllerFriendlyName(name, ControllersDictionary.TryGetAreaName(fullName));
        }

        /// <summary>
        /// Interceptor that will replace an instance of a requested type
        /// </summary>
        //private class TypeReplacementInterceptor : TypeInterceptor
        //{
        //    /// <summary>
        //    /// Type to intercept and replace
        //    /// </summary>
        //    private readonly Type typeToReplace;

        //    /// <summary>
        //    /// Type used as the replacement
        //    /// </summary>
        //    private readonly Type replacementType;

        //    /// <summary>
        //    /// Initializes a new instance of the TypeReplacementInterceptor class that will replace an instance
        //    /// </summary>
        //    /// <param name="typeToReplace">Requested type to intercept</param>
        //    /// <param name="replacementType">Type to replace</param>
        //    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="typeToReplace"/> or <paramref name="replacementType"/> is null</exception>
        //    public TypeReplacementInterceptor(Type typeToReplace, Type replacementType)
        //    {
        //        Ensure.Argument.NotNull(typeToReplace, "typeToReplace");
        //        Ensure.Argument.NotNull(replacementType, "replacementType");

        //        this.typeToReplace = typeToReplace;
        //        this.replacementType = replacementType;
        //    }

        //    /// <summary>
        //    /// Matches type against requested replacement type
        //    /// </summary>
        //    /// <param name="type">Type to test for replacement</param>
        //    /// <returns>Value indicating whether type should be intercepted</returns>
        //    public bool MatchesType(Type type)
        //    {
        //        return type != null && type.Equals(this.typeToReplace);
        //    }

        //    /// <summary>
        //    /// Replaces an instance with the replacement type
        //    /// </summary>
        //    /// <param name="target">Instance already initialized</param>
        //    /// <param name="context">Context of the interception</param>
        //    /// <returns>New object that replaces the <paramref name="target"/> with an object with the replacement type</returns>
        //    public object Process(object target, IContext context)
        //    {
        //        // Sanity check: If the context is null, we can't do anything about it!
        //        if (context == null)
        //            return target;
        //        return context.GetInstance(this.replacementType);
        //    }
        //}

        public void ScanTypes(TypeSet types, Registry registry)
        {
            throw new NotImplementedException();
        }
    }
}