using System.Collections.Generic;
using Paralect.Domain;
using Paralect.ServiceBus.Dispatching;
using mPower.Framework;

namespace mPower.Tests.Environment
{
    public class TestEventService : IEventService
    {
        private readonly Dispatcher _dispatcher;

        public List<IEvent> Events { get; private set; }

        public TestEventService(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            Events = new List<IEvent>();
        }

        public void Send(params IEvent[] events)
        {
            foreach (var @event in events)
            {
                Events.Add(@event);
                _dispatcher.Dispatch(@event);
            }
        }
    }
}