using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;

namespace mPower.Framework.Environment.MultiTenancy
{

    public interface ITenantConfigurer
    {
        void Configure(Container container);
    } 
}
