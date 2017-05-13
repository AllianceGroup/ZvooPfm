using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using mPower.Tests.Environment;

namespace mPower.Tests.MoqObjects
{
    public class MockFactory
    {
        private readonly IContainer _container;

        public MockFactory(IContainer container)
        {
            _container = container;
        }

        public TMock Create<TMock>() 
        {
            return _container.GetInstance<TMock>();
        }
    }
}
