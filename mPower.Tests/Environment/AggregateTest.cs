using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using mPower.Framework;
using NUnit.Framework;
using Paralect.Domain;
using Paralect.Domain.EventBus;
using Paralect.ServiceBus;
using Paralect.ServiceBus.Dispatching;
using Paralect.Transitions;
using StructureMap;

namespace mPower.Tests.Environment
{
    [TestFixture]
    public abstract class AggregateTest<TAggregate>
        where TAggregate : AggregateRoot
    {
        protected String _id;
        protected TAggregate _aggregate;
        protected IEventBus _eventBus;
        protected List<IEvent> _actualEvents;
        protected Exception _lastExceptions;
        protected Dispatcher _eventDispatcher;
        protected bool _createNewDatabase;
        protected MongoRead _mongoRead;

        protected IContainer _container;

        public abstract IEnumerable<IEvent> Given();
        public abstract IEnumerable<ICommand> When();
        public abstract IEnumerable<IEvent> Expected();

        [TestFixtureSetUp]
        public void Setup()
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.BootstrapStructureMap();
            _eventDispatcher = bootstrapper.BootsrapDispatcher();
            _container = bootstrapper.Container;
            _eventBus = _container.GetInstance<IEventBus>();
        }

        [SetUp]
        public void Prepare()
        {
            _id = Guid.NewGuid().ToString();
            _actualEvents = new List<IEvent>();
            PrepareEvents();
            var bus = GetInstance<IServiceBus>();
            foreach (var command in When())
            {
                bus.Send(command);
            }
            _actualEvents = ((InMemoryEventBus)_eventBus).Events;
            _lastExceptions = bus.GetLastException();

        }

        protected virtual void PrepareEvents()
        {
            var store = GetInstance<ITransitionStorage>();
            using (var stream = store.OpenStream(_id))
            {
                var transitionEvents = Given().Select(e => new TransitionEvent("", e, null)).ToList();
                stream.Write(new Transition(new TransitionId(_id, 1), DateTime.Now, transitionEvents, null));
            }
        }

        [TearDown]
        public void CleanUp()
        {
            if (_createNewDatabase)
                _mongoRead.Database.Drop();
        }

        public void Validate(params string[] exclude)
        {
            var expectedEvents = Expected().ToList();
            Assert.AreEqual(expectedEvents.Count, _actualEvents.Count);

            for (int i = 0; i < _actualEvents.Count; i++)
            {
                var actual = _actualEvents[i];
                var expected = expectedEvents[i];

                var excludeList = new List<string>(exclude);
                excludeList.Add("Metadata");
                excludeList.Add("NewBalanceOfAggregatedAccount");
                var equal = ObjectComparer.AreObjectsEqual(expected, actual, IgnoreList.Create(excludeList.ToArray())); // ignore property with Metadata name
                Assert.IsTrue(equal);
            }
        }

        public void DispatchEvents(Action testReadModel, bool createNewDatabase = true)
        {
            ApplyCreateNewDatabaseSetting(createNewDatabase);

            try
            {

                var currentEvents = new IEvent[_actualEvents.Count];
                //we need to copy our events, because of _actualEvents can be changed (we are sending event from event handler)
                // or alternatevely we can use simple for loop
                _actualEvents.CopyTo(currentEvents);
                var events = Given().Concat(currentEvents);

                foreach (var @event in events)
                {
                    _eventDispatcher.Dispatch(@event);
                }
                if (testReadModel != null)
                    testReadModel();
            }
            finally
            {

            }
        }

        public void AssertException<TException>()
        {
            if (_lastExceptions == null)
                throw new Exception(String.Format("Message [{0}] expected.", typeof(TException).FullName));

            if (_actualEvents.Count > 0)
                throw new Exception(String.Format("Events shouldn't be published because of exception"));
        }

        /// <summary>
        /// Get instanse of specified type
        /// </summary>
        public T GetInstance<T>()
        {
            return _container.GetInstance<T>();
        }

        protected void ApplyCreateNewDatabaseSetting(bool createNewDatabase)
        {
            var sessionId = String.Format("mpower_{0}", ObjectId.GenerateNewId());
            var settings = GetInstance<MPowerSettings>();
            _createNewDatabase = createNewDatabase;

            if (createNewDatabase)
            {
                _mongoRead = new MongoRead(settings.MongoTestReadDatabaseConnectionString + sessionId);
                _container.Configure(config => config.For<MongoRead>().Use(_mongoRead));
            }
        }
    }
}