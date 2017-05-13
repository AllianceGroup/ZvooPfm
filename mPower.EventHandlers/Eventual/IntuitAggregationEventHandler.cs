using mPower.Aggregation.Client;
using mPower.Aggregation.Contract.Data;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Accounting.Transaction.Messages;
using mPower.Domain.Membership.User.Messages;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Eventual
{
    public class IntuitAggregationEventHandler:
        IMessageHandler<IntuitLogsDeletedMessage>,
        IMessageHandler<Aggregation_TransactionSavedMessage>
    {
        private readonly IAggregationClient _aggregation;
        private readonly UserDocumentService _userDocumentService;

        public IntuitAggregationEventHandler(IAggregationClient aggregation, UserDocumentService userDocumentService)
        {
            _aggregation = aggregation;
            _userDocumentService = userDocumentService;
        }

        public void Handle(IntuitLogsDeletedMessage message)
        {
            _aggregation.DeleteUserLogs(message.UserId);
        }

        public void Handle(Aggregation_TransactionSavedMessage message)
        {
            if (message.LatestPostedDate.HasValue)
            {
                var enableIntuitLogging = _userDocumentService.GetById(message.UserId).Settings.EnableIntuitLogging;
                _aggregation.ConfirmTransactionsSaved(new Metadata
                {
                    LogonId = message.UserId,
                    IsLoggingEnabled = enableIntuitLogging
                }, message.IntuitAccountId, message.LedgerId, message.LatestPostedDate.Value, message.SavedTransactionFinicityIds);
            }
        }
    }
}
