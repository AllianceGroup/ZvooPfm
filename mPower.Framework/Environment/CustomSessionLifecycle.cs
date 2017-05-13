using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using StructureMap.Pipeline;

namespace mPower.Framework.Environment
{
    public class CustomSessionLifecycle : ILifecycle
    {

        public void EjectAll(ILifecycleContext context)
        {
            throw new NotImplementedException();
        }

        public IObjectCache FindCache(ILifecycleContext context)
        {
            throw new NotImplementedException();
        }

        public string Description => "CustomSessionLifecycle";
    }
}
