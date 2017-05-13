using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paralect.ServiceBus;

namespace mPower.Framework
{
    //TODO: Temp quick solution, remove this class later.
    public class AsyncServiceBus
    {
        private readonly IServiceBus _bus;

        public IServiceBus Bus
        {
            get
            {
                return _bus;
            }
        }

        public AsyncServiceBus(IServiceBus bus)
        {
            _bus = bus;
        }
    }
}
