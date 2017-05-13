using System;
using Paralect.Domain;
using mPower.Framework.Environment;

namespace mPower.Framework
{
    public class MpowerAggregateRoot : AggregateRoot
    {
        private readonly MongoObjectIdGenerator _idGenerator;

        public MpowerAggregateRoot()
        {
            _idGenerator = new MongoObjectIdGenerator();
        }

        private ICommandMetadata _commandMetadata;

        public void SetCommandMetadata(ICommandMetadata commandMetadata)
        {
            _commandMetadata = commandMetadata;
        }

        public new void Apply(IEvent evt)
        {
            if(_commandMetadata == null)
                throw new ArgumentException("You should send command metadata to Aggregate Root before Apply event");

            evt.Metadata.UserId = _commandMetadata.UserId;
            evt.Metadata.CommandId = _commandMetadata.CommandId;
            evt.Metadata.EventId = _idGenerator.Generate();

            base.Apply(evt);
        }
    }
}
