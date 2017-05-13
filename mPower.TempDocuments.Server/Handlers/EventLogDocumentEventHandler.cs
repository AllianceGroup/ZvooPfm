using System;
using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices;
using mPower.Documents.Documents;
using mPower.Domain.Accounting.CreditIdentity.Events;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;

namespace mPower.EventHandlers.Eventual
{
    public class EventLogDocumentEventHandler :
        IMessageHandler<Affiliate_CreatedEvent>,
        IMessageHandler<Affiliate_SynchronizedChargifyProductsEvent>,
        IMessageHandler<Affiliate_UpdatedEvent>,
        IMessageHandler<Affiliate_DeleteEvent>,
        IMessageHandler<Affiliate_Email_Content_AddedEvent>,
        IMessageHandler<Affiliate_Email_Content_ChangedEvent>,
        IMessageHandler<Affiliate_Email_Content_DeletedEvent>,
        IMessageHandler<Affiliate_Email_Template_AddedEvent>,
        IMessageHandler<Affiliate_Email_Template_ChangedEvent>,
        IMessageHandler<Affiliate_Email_Template_DeletedEvent>,
        IMessageHandler<Transaction_CreatedEvent>,
        IMessageHandler<Transaction_DeletedEvent>,
        IMessageHandler<Transaction_ModifiedEvent>,
        IMessageHandler<Ledger_CreatedEvent>,
        IMessageHandler<Ledger_DeletedEvent>,
        IMessageHandler<Ledger_Account_BalanceChangedEvent>,
        IMessageHandler<Ledger_Account_AddedEvent>,
        IMessageHandler<Ledger_Account_ArchivedEvent>,
        IMessageHandler<Ledger_Account_RemovedEvent>,
        IMessageHandler<Ledger_Account_UpdatedEvent>,
        IMessageHandler<Ledger_Budget_ExceededEvent>,
        IMessageHandler<Ledger_User_AddedEvent>,
        IMessageHandler<Ledger_User_RemovedEvent>,
        IMessageHandler<CreditIdentity_Alerts_AddedEvent>,
        IMessageHandler<CreditIdentity_CreatedEvent>,
        IMessageHandler<CreditIdentity_DeletedEvent>,
        IMessageHandler<CreditIdentity_Report_AddedEvent>,
        IMessageHandler<CreditIdentity_Questions_SetEvent>,
        IMessageHandler<CreditIdentity_EnrolledEvent>,
        IMessageHandler<CreditIdentity_CanceledEnrollEvent>,
        IMessageHandler<CreditIdentity_MarkedAsVerifiedEvent>,
        IMessageHandler<User_CreatedEvent>,
        IMessageHandler<User_ActivatedEvent>,
        IMessageHandler<User_DeactivatedEvent>,
        IMessageHandler<User_DeletedEvent>,
        IMessageHandler<User_LoggedInEvent>,
        IMessageHandler<User_MobileLoggedInEvent>,
        IMessageHandler<User_PasswordChangedEvent>,
        IMessageHandler<User_UpdatedEvent>,
        IMessageHandler<User_UpdatedSecurityLevelEvent>,
        IMessageHandler<User_UpdatedSecurityQuestionEvent>,
        IMessageHandler<User_PermissionRemovedEvent>,
        IMessageHandler<User_PermissionAddedEvent>,
        IMessageHandler<User_UpdatedResetPasswordTokenEvent>,
        IMessageHandler<User_PasswordResettedEvent>,
        IMessageHandler<User_Subscription_SubscribedEvent>,
        IMessageHandler<User_Subscription_DeletedEvent>,
        IMessageHandler<User_WindwosLiveSignedupEvent>,
        IMessageHandler<User_FacebookSignedupEvent>,
        IMessageHandler<User_TwitterSignedupEvent>,
        IMessageHandler<User_GoogleSignedupEvent>,
        IMessageHandler<User_AddedFacebookAccountEvent>,
        IMessageHandler<User_AddedGoogleAccountEvent>,
        IMessageHandler<User_AddedTwitterAccountEvent>,
        IMessageHandler<User_AddedWindowsLiveAccountEvent>,
        IMessageHandler<User_Notification_UpdatedEvent>,
        IMessageHandler<User_Subscription_ReceivedWebhookEvent>,
        IMessageHandler<Affiliate_Faq_AddedEvent>,
        IMessageHandler<Affiliate_Faq_UpdatedEvent>,
        IMessageHandler<Affiliate_Faq_DeletedEvent>
    {
        private readonly EventLogDocumentService _eventLogService;

        public EventLogDocumentEventHandler(EventLogDocumentService eventLogService)
        {
            _eventLogService = eventLogService;
        }

        private void SaveEvent(IEvent evt, string text = null)
        {
            _eventLogService.Insert(EventLogDocument.Create(evt, text));
        }

        public void Handle(Affiliate_CreatedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(Affiliate_SynchronizedChargifyProductsEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(Affiliate_UpdatedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(Affiliate_DeleteEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(Affiliate_Email_Content_AddedEvent message)
        {
            var text = String.Format("New email content was added.");

            SaveEvent(message, text);
        }

        public void Handle(Affiliate_Email_Content_ChangedEvent message)
        {
            var text = String.Format("Email content was updated.");

            SaveEvent(message, text);
        }

        public void Handle(Affiliate_Email_Content_DeletedEvent message)
        {
            var text = String.Format("Email content was deleted.");

            SaveEvent(message, text);
        }
        public void Handle(Affiliate_Faq_AddedEvent message)
        {
            var text = String.Format("New FAQ document was added.");

            SaveEvent(message, text);
        }

        public void Handle(Affiliate_Faq_UpdatedEvent message)
        {
            var text = String.Format("FAQ document was changed.");

            SaveEvent(message, text);
        }

        public void Handle(Affiliate_Faq_DeletedEvent message)
        {
            var text = String.Format("FAQ document was deleted.");

            SaveEvent(message, text);
        }

        public void Handle(Affiliate_Email_Template_AddedEvent message)
        {
            var text = String.Format("New email template was added.");

            SaveEvent(message, text);
        }

        public void Handle(Affiliate_Email_Template_ChangedEvent message)
        {
            var text = String.Format("New email template was changed.");

            SaveEvent(message, text);
        }

        public void Handle(Affiliate_Email_Template_DeletedEvent message)
        {
            var text = String.Format("Email template was deleted.");

            SaveEvent(message, text);
        }

        public void Handle(Transaction_CreatedEvent message)
        {
            var text = String.Format("New transaction was created for ledger '{0}'.", message.LedgerId);

            SaveEvent(message, text);
        }

        public void Handle(Transaction_ModifiedEvent message)
        {
            var text = String.Format("Transaction of ledger '{0}' was modified.", message.LedgerId);

            SaveEvent(message, text);
        }

        public void Handle(Transaction_DeletedEvent message)
        {
            var text = String.Format("Transaction of ledger '{0}' was deleted.", message.LedgerId);

            SaveEvent(message, text);
        }

        public void Handle(Ledger_CreatedEvent message)
        {
            var text = String.Format("New ledger '{0}' was created.", message.LedgerId);
            SaveEvent(message, text);
        }

        public void Handle(Ledger_DeletedEvent message)
        {
            var text = String.Format("Ledger was deleted");
            SaveEvent(message, text);
        }

        public void Handle(Ledger_Account_BalanceChangedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(Ledger_Account_AddedEvent message)
        {
            var text = String.Format("New '{0}' account '{1}' was added to the ledger {2}", message.AccountId, message.Name, message.LedgerId);
            SaveEvent(message, text);
        }

        public void Handle(Ledger_Account_ArchivedEvent message)
        {
            var text = String.Format("Account '{0}' from the ledger {1} was archived", message.AccountId, message.LedgerId);
            SaveEvent(message, text);
        }

        public void Handle(Ledger_Account_RemovedEvent message)
        {
            var text = String.Format("Account from the ledger {0} was remove", message.LedgerId);
            SaveEvent(message, text);
        }

        public void Handle(Ledger_Account_UpdatedEvent message)
        {
            var text = String.Format("Account from the ledger {0} was updated", message.LedgerId);
            SaveEvent(message, text);
        }

        public void Handle(Ledger_Budget_ExceededEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(Ledger_User_AddedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(Ledger_User_RemovedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(CreditIdentity_Alerts_AddedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(CreditIdentity_CreatedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(CreditIdentity_DeletedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(CreditIdentity_Report_AddedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(CreditIdentity_Questions_SetEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(CreditIdentity_EnrolledEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(CreditIdentity_CanceledEnrollEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(CreditIdentity_MarkedAsVerifiedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_CreatedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_ActivatedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_DeactivatedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_DeletedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_LoggedInEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_MobileLoggedInEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_PasswordChangedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_UpdatedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_UpdatedSecurityLevelEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_UpdatedSecurityQuestionEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_PermissionRemovedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_PermissionAddedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_UpdatedResetPasswordTokenEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_PasswordResettedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_Subscription_SubscribedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_Subscription_DeletedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_WindwosLiveSignedupEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_FacebookSignedupEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_TwitterSignedupEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_GoogleSignedupEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_AddedFacebookAccountEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_AddedGoogleAccountEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_AddedTwitterAccountEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_AddedWindowsLiveAccountEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_Notification_UpdatedEvent message)
        {
            SaveEvent(message);
        }

        public void Handle(User_Subscription_ReceivedWebhookEvent message)
        {
            SaveEvent(message);
        }
    }
}
