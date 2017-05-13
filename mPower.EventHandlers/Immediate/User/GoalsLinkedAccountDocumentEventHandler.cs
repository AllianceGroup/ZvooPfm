using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Membership.User.Events;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Immediate.User
{
    public class GoalsLinkedAccountDocumentEventHandler : 
        IMessageHandler<User_GoalsLinkedAccount_SetEvent>,
        IMessageHandler<Ledger_DeletedEvent>,
        IMessageHandler<Ledger_Account_RemovedEvent>
    {
        private readonly UserDocumentService _userService;

        public GoalsLinkedAccountDocumentEventHandler(UserDocumentService userService)
        {
            _userService = userService;
        }

        public void Handle(User_GoalsLinkedAccount_SetEvent message)
        {          
            _userService.SetGoalsLinkedAccount(message.UserId,message.LedgerId,message.AccountId);
        }

        public void Handle(Ledger_DeletedEvent message)
        {
            _userService.RemoveGoalsLinkedAccount(message.LedgerId);
        }

        public void Handle(Ledger_Account_RemovedEvent message)
        {
            _userService.RemoveGoalsLinkedAccount(message.LedgerId,message.AccountId);
        }
    }
}