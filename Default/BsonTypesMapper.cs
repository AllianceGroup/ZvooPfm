using System.Web.Mvc;
using MongoDB.Bson.Serialization;
using TUICreditScore20;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Accounting.CreditIdentity.Events;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using mPower.TempDocuments.Server.Documents;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;
using TaxLien = mPower.Domain.Accounting.CreditIdentity.Data.TaxLien;

namespace Default
{
    public static class BsonTypesMapper
    {
        public static void RegisterBsonMaps()
        {
            BsonClassMap.RegisterClassMap<Affiliate_CreatedEvent>();
            BsonClassMap.RegisterClassMap<Affiliate_SynchronizedChargifyProductsEvent>();
            BsonClassMap.RegisterClassMap<Affiliate_UpdatedEvent>();
            BsonClassMap.RegisterClassMap<Affiliate_DeleteEvent>();
            BsonClassMap.RegisterClassMap<Affiliate_Email_Content_AddedEvent>();
            BsonClassMap.RegisterClassMap<Affiliate_Email_Content_ChangedEvent>();
            BsonClassMap.RegisterClassMap<Affiliate_Email_Content_DeletedEvent>();
            BsonClassMap.RegisterClassMap<Affiliate_Email_Template_AddedEvent>();
            BsonClassMap.RegisterClassMap<Affiliate_Email_Template_ChangedEvent>();
            BsonClassMap.RegisterClassMap<Affiliate_Email_Template_DeletedEvent>();
            BsonClassMap.RegisterClassMap<Affiliate_NotificationTypeEmail_AddedEvent>();
            BsonClassMap.RegisterClassMap<Affiliate_NotificationTypeEmail_ChangedEvent>();
            BsonClassMap.RegisterClassMap<Ledger_Transaction_CreatedEvent>();
            BsonClassMap.RegisterClassMap<Ledger_Transaction_ModifiedEvent>();
            BsonClassMap.RegisterClassMap<Ledger_CreatedEvent>();
            BsonClassMap.RegisterClassMap<Ledger_DeletedEvent>();
            BsonClassMap.RegisterClassMap<Ledger_Account_BalanceChangedEvent>();
            BsonClassMap.RegisterClassMap<Ledger_Account_AddedEvent>();
            BsonClassMap.RegisterClassMap<Ledger_Account_ArchivedEvent>();
            BsonClassMap.RegisterClassMap<Ledger_Account_RemovedEvent>();
            BsonClassMap.RegisterClassMap<Ledger_Account_UpdatedEvent>();
            BsonClassMap.RegisterClassMap<Ledger_Budget_ExceededEvent>();
            BsonClassMap.RegisterClassMap<Ledger_User_AddedEvent>();
            BsonClassMap.RegisterClassMap<Ledger_User_RemovedEvent>();
            BsonClassMap.RegisterClassMap<CreditIdentity_Alerts_AddedEvent>();
            BsonClassMap.RegisterClassMap<CreditIdentity_CreatedEvent>();
            BsonClassMap.RegisterClassMap<CreditIdentity_DeletedEvent>();
            BsonClassMap.RegisterClassMap<CreditIdentity_Report_AddedEvent>();
            BsonClassMap.RegisterClassMap<CreditIdentity_Questions_SetEvent>();
            BsonClassMap.RegisterClassMap<CreditIdentity_EnrolledEvent>();
            BsonClassMap.RegisterClassMap<CreditIdentity_CanceledEnrollEvent>();
            BsonClassMap.RegisterClassMap<CreditIdentity_MarkedAsVerifiedEvent>();
            BsonClassMap.RegisterClassMap<User_CreatedEvent>();
            BsonClassMap.RegisterClassMap<User_ActivatedEvent>();
            BsonClassMap.RegisterClassMap<User_DeactivatedEvent>();
            BsonClassMap.RegisterClassMap<User_DeletedEvent>();
            BsonClassMap.RegisterClassMap<User_LoggedInEvent>();
            BsonClassMap.RegisterClassMap<User_MobileLoggedInEvent>();
            BsonClassMap.RegisterClassMap<User_PasswordChangedEvent>();
            BsonClassMap.RegisterClassMap<User_UpdatedEvent>();
            BsonClassMap.RegisterClassMap<User_UpdatedSecurityLevelEvent>();
            BsonClassMap.RegisterClassMap<User_UpdatedSecurityQuestionEvent>();
            BsonClassMap.RegisterClassMap<User_PermissionRemovedEvent>();
            BsonClassMap.RegisterClassMap<User_PermissionAddedEvent>();
            BsonClassMap.RegisterClassMap<User_UpdatedResetPasswordTokenEvent>();
            BsonClassMap.RegisterClassMap<User_PasswordResettedEvent>();
            BsonClassMap.RegisterClassMap<User_WindwosLiveSignedupEvent>();
            BsonClassMap.RegisterClassMap<User_FacebookSignedupEvent>();
            BsonClassMap.RegisterClassMap<User_TwitterSignedupEvent>();
            BsonClassMap.RegisterClassMap<User_GoogleSignedupEvent>();
            BsonClassMap.RegisterClassMap<User_AddedFacebookAccountEvent>();
            BsonClassMap.RegisterClassMap<User_AddedGoogleAccountEvent>();
            BsonClassMap.RegisterClassMap<User_AddedTwitterAccountEvent>();
            BsonClassMap.RegisterClassMap<User_AddedWindowsLiveAccountEvent>();
            BsonClassMap.RegisterClassMap<User_Notification_UpdatedEvent>();
            BsonClassMap.RegisterClassMap<User_Subscription_SubscribedEvent>();
            BsonClassMap.RegisterClassMap<User_Subscription_DeletedEvent>();
            BsonClassMap.RegisterClassMap<User_Subscription_ReceivedWebhookEvent>();
            // documents
            BsonClassMap.RegisterClassMap<LowBalanceAlertDocument>();
            BsonClassMap.RegisterClassMap<LargePurchaseAlertDocument>();
            BsonClassMap.RegisterClassMap<AvailableCreditAlertDocument>();
            BsonClassMap.RegisterClassMap<UnusualSpendingAlertDocument>();
            BsonClassMap.RegisterClassMap<OverBudgetAlertDocument>();
            BsonClassMap.RegisterClassMap<CalendarEventAlertDocument>();
            BsonClassMap.RegisterClassMap<TransactionDuplicateDocument>();
            BsonClassMap.RegisterClassMap<EntryDuplicateDocument>();
            BsonClassMap.RegisterClassMap<LegalItem>();
            BsonClassMap.RegisterClassMap<MaritalItem>();
            BsonClassMap.RegisterClassMap<TaxLien>();
            BsonClassMap.RegisterClassMap<Bankruptcy>();
            BsonClassMap.RegisterClassMap<SelectList>();
            BsonClassMap.RegisterClassMap<MonthYear>();
        }
    }
}
