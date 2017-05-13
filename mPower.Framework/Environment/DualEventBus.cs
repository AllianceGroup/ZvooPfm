using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paralect.Domain;
using Paralect.Domain.EventBus;
using Paralect.ServiceBus;

namespace mPower.Framework.Environment
{
    public class DualEventBus : IEventBus
    {
        private readonly IServiceBus _bus;
        private readonly AsyncServiceBus _asyncBus;

        public DualEventBus(IServiceBus bus, AsyncServiceBus asyncBus)
        {
            this._bus = bus;
            _asyncBus = asyncBus;
        }

        public void Publish(IEnumerable<IEvent> eventMessages)
        {
            foreach (var message in eventMessages)
                Publish(message);

        }

        public void Publish(IEvent eventMessage)
        {
            _bus.Publish(eventMessage);
            _asyncBus.Bus.Publish(eventMessage);
        }
    }
}
