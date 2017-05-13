using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.Transaction.Messages;
using mPower.Framework;

namespace mPower.Domain.Accounting.Transaction.Commands
{
    public class Transaction_DeleteMultipleCommandHandler : IMessageHandler<Transaction_DeleteMultipleCommand>
    {
        private readonly IRepository _repository;
        private readonly IEventService _eventService;

        public Transaction_DeleteMultipleCommandHandler(IRepository repository, IEventService eventService)
        {
            _repository = repository;
            _eventService = eventService;
        }

        public void Handle(Transaction_DeleteMultipleCommand message)
        {
            foreach (var transactionId in message.TransactionIds)
            {
                var transactionAr = _repository.GetById<TransactionAR>(transactionId);

                transactionAr.SetCommandMetadata(message.Metadata);
                transactionAr.DeleteTransaction(message.LedgerId, true);

                _repository.Save(transactionAr);
            }

            var deleteMultipleMessage = new Transaction_DeleteMultipleMessage()
            {
                LedgerId = message.LedgerId,
                TransactionIds = message.TransactionIds
            };

            _eventService.Send(deleteMultipleMessage);
        }
    }
}
