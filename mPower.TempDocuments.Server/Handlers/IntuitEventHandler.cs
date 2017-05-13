using mPower.Aggregation.Client;
using mPower.Aggregation.Contract.Data;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Accounting.Ledger.Messages;
using mPower.Domain.Membership.User.Events;
using Paralect.ServiceBus;

namespace mPower.TempDocuments.Server.Handlers
{
    public class IntuitEventHandler :
        IMessageHandler<Ledger_Account_RemovedMessage>,
        IMessageHandler<User_DeletedEvent>
    {
        private readonly IAggregationClient _aggregation;
        private readonly UserDocumentService _userService;

        public IntuitEventHandler(IAggregationClient aggregation, UserDocumentService userService)
        {
            _aggregation = aggregation;
            _userService = userService;
        }

        public void Handle(Ledger_Account_RemovedMessage message)
        {
            if (message.IntuitAccountId.HasValue)
            {
                _aggregation.DeleteAccount(GetMetadata(message.UserId), message.IntuitAccountId.Value, message.LedgerId);
            }
        }

        public void Handle(User_DeletedEvent message)
        {
            _aggregation.DeleteUser(GetMetadata(message.Metadata.UserId), message.UserId);
        }

        private Metadata GetMetadata(string userId)
        {
            var user = _userService.GetById(userId);
            var meta = new Metadata { LogonId = userId };
            if (user != null)
                meta.IsLoggingEnabled = user.Settings.EnableIntuitLogging;

            return meta;
        }
    }
}
