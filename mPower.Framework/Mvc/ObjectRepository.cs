using System.Collections.Generic;
using StructureMap;

namespace mPower.Framework.Mvc
{
    public class ObjectRepository : IObjectRepository
    {
        private IContainer container;

        public ObjectRepository(IContainer container)
        {
            this.container = container;
        }

        public TOutput Load<TInput, TOutput>(TInput input)
        {
            var factory = container.TryGetInstance<IObjectFactory<TInput, TOutput>>();
            if (factory == null) return default(TOutput);
            return factory.Load(input);
        }

        public TOutput Load<TOutput>()
        {
            var factory = container.TryGetInstance<IObjectFactory<TOutput>>();
            if (factory == null) return default(TOutput);
            return factory.Load();
        }
    }
}