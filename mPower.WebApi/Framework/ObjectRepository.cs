using System;
using mPower.Framework.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace mPower.WebApi.Framework
{
    public class ObjectRepository : IObjectRepository
    {
        private readonly IServiceProvider _serviceProvider;

        public ObjectRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TOutput Load<TInput, TOutput>(TInput input)
        {
            //var factory = _serviceProvider.GetService<IObjectFactory<TInput, TOutput>>();
            var factory = ActivatorUtilities.GetServiceOrCreateInstance<IObjectFactory<TInput, TOutput>>(_serviceProvider);
            if (factory == null) return default(TOutput);
            return factory.Load(input);
        }
    }
}