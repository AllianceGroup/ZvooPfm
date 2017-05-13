using System;
using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using Paralect.Domain.Utilities;

namespace mPower.Tests.Environment
{
    [TestFixture]
    public abstract class DirectAggregateTest<TAggregate> 
        where TAggregate : AggregateRoot
    {
        protected String _id;
        protected TAggregate _aggregate;

        public abstract IEnumerable<IEvent> Given();
        public abstract void When();

        [Test]
        public abstract void Then();

        [SetUp]
        public void Prepare()
        {
            _id = Guid.NewGuid().ToString();

            _aggregate = AggregateCreator.CreateAggregateRoot<TAggregate>();
            _aggregate.LoadFromEvents(Given());
        }
    }
}
