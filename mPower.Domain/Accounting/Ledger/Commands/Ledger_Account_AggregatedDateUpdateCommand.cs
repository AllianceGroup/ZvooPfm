using System;
using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Accounting.Ledger.Commands
{
    public class Ledger_Account_AggregatedDateUpdateCommand : Command
    {
        public string AccountId { get; set; }
        public string LedgerId { get; set; }
        public DateTime Date { get; set; }
    }

    public class Ledger_Account_AggregatedDateUpdateCommandHandler : IMessageHandler<Ledger_Account_AggregatedDateUpdateCommand>
    {
        private readonly IRepository _repository;

        public Ledger_Account_AggregatedDateUpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Ledger_Account_AggregatedDateUpdateCommand message)
        {
            var ar = _repository.GetById<LedgerAR>(message.LedgerId);
            ar.SetCommandMetadata(message.Metadata);
            ar.ChangeAccountAggregationDate(message.AccountId, message.Date);
            _repository.Save(ar);
        }
    }
}
