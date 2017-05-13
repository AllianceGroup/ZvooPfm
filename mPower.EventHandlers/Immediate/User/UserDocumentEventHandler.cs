using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.ExternalServices.Janrain;
using mPower.Domain.Application.Enums;
using mPower.Domain.Membership.User.Events;
using mPower.Domain.Membership.User.Messages;
using mPower.Framework;
using Paralect.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mPower.EventHandlers.Immediate.User
{
    public class UserDocumentEventHandler :
        IMessageHandler<User_CreatedEvent>,
        IMessageHandler<User_ActivatedEvent>,
        IMessageHandler<User_DeactivatedEvent>,
        IMessageHandler<User_DeletedEvent>,
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
        IMessageHandler<User_Subscription_CreatedEvent>,
        IMessageHandler<User_Subscription_ReceivedWebhookEvent>,
        IMessageHandler<User_Realestate_AddedEvent>,
        IMessageHandler<User_Realestate_DeletedEvent>,
        IMessageHandler<User_WindwosLiveSignedupEvent>,
        IMessageHandler<User_FacebookSignedupEvent>,
        IMessageHandler<User_TwitterSignedupEvent>,
        IMessageHandler<User_GoogleSignedupEvent>,
        IMessageHandler<User_AddedFacebookAccountEvent>,
        IMessageHandler<User_AddedGoogleAccountEvent>,
        IMessageHandler<User_AddedTwitterAccountEvent>,
        IMessageHandler<User_AddedWindowsLiveAccountEvent>,
        IMessageHandler<User_Notification_UpdatedEvent>,
        IMessageHandler<User_AddedYodleeAccountEvent>,
        IMessageHandler<User_Email_AddedEvent>,
        IMessageHandler<User_Email_RemovedEvent>,
        IMessageHandler<User_AutoUpdateDateSetEvent>,
        IMessageHandler<User_Phone_AddedEvent>,
        IMessageHandler<User_SecuritySettingsUpdatedEvent>,
        IMessageHandler<User_ZipCode_ChangedEvent>,
        IMessageHandler<User_Phone_RemovedEvent>,
        IMessageHandler<User_MerchantInfo_UpdatedEvent>,
        IMessageHandler<User_Merchant_BillingInfo_UpdatedEvent>,
        IMessageHandler<User_Realestate_IncludeInWorthEvent>
    {
        private readonly UserDocumentService _userService;
        private readonly AffiliateDocumentService _affiliateService;
        private readonly IEventService _eventService;

        public UserDocumentEventHandler(UserDocumentService userService, AffiliateDocumentService affiliateService, IEventService eventService)
        {
            _userService = userService;
            _affiliateService = affiliateService;
            _eventService = eventService;
        }

        public void Handle(User_CreatedEvent message)
        {
            string affiliateName = message.ApplicationId;
            var affiliate = _affiliateService.GetById(message.ApplicationId);
            affiliateName = affiliate == null ? affiliateName : affiliate.ApplicationName.ToLower();

            var user = new UserDocument
                           {
                               CreateDate = message.CreateDate,
                               Email = message.Email,
                               FirstName = message.FirstName,
                               Id = message.UserId,
                               IsActive = message.IsActive,
                               Password = message.Password,
                               LastName = message.LastName,
                               UserName = message.UserName,
                               ApplicationId = message.ApplicationId,
                               AffiliateName = affiliateName,
                               Notifications = GetDefaultNotifications(),
                               ZipCode = message.ZipCode,
                               BirthDate = message.BirthDate,
                               Gender = message.Gender,
                               ReferralCode = message.ReferralCode,
                           };

            _userService.Insert(user);

            _eventService.Send(new User_CreatedMessage
                                   {
                                       UserId = user.Id,
                                       CreateDate = user.CreateDate,
                                       AffiliateId = user.ApplicationId,
                                       AffiliateName = user.AffiliateName,
                                       IsActive = user.IsActive
                                   });
        }

        public void Handle(User_ActivatedEvent message)
        {
            _userService.SetActivation(message.UserId, true);
        }

        public void Handle(User_DeactivatedEvent message)
        {
            _userService.SetActivation(message.UserId, false);
        }

        public void Handle(User_DeletedEvent message)
        {
            var user = _userService.GetById(message.UserId);

            _userService.RemoveById(message.UserId);

            if (user != null)
            {
                _eventService.Send(new User_DeletedMessage
                                       {
                                           UserId = user.Id,
                                           AffiliateId = user.ApplicationId,
                                           AffiliateName = user.AffiliateName,
                                       });
            }
        }

        public void Handle(User_MobileLoggedInEvent message)
        {       
            _userService.SetLastMobileLogin(message.UserId, message.Date, message.AccessToken);
        }

        public void Handle(User_PasswordChangedEvent message)
        {
            _userService.ChangePassword(message.UserId, message.NewPassword, message.ChangeDate);
        }

        public void Handle(User_UpdatedEvent updatedEvent)
        {
            var user = _userService.GetById(updatedEvent.UserId);
            if (user != null)
            {
                _userService.UpdateUser(updatedEvent);
                
                _eventService.Send(new User_UpdatedMessage
                {
                    UserId = updatedEvent.UserId,
                    FirstName = updatedEvent.FirstName,
                    LastName = updatedEvent.LastName,
                    Email = updatedEvent.Email,
                    ApplicationId = user.ApplicationId,
                    ZipCode = user.ZipCode,
                    BirthDate = user.BirthDate,
                    Gender = user.Gender,
                });
            }
        }

        public void Handle(User_UpdatedSecurityLevelEvent message)
        {
            _userService.SetSecurityLevel(message.UserId,message.SecurityLevel);
        }

        public void Handle(User_UpdatedSecurityQuestionEvent message)
        {
            _userService.SetSequrityQuestion(message.UserId,message.Question,message.Answer);
        }

        public void Handle(User_PermissionRemovedEvent message)
        {
            _userService.RemovePermissions(message.UserId,message.Permission);
        }

        public void Handle(User_PermissionAddedEvent message)
        {
            _userService.AddPermissions(message.UserId,message.Permission);
        }

        public void Handle(User_UpdatedResetPasswordTokenEvent message)
        {
            _userService.SetResetPasswordToken(message.UserId, message.UniqueToken);
        }

        public void Handle(User_PasswordResettedEvent message)
        {
            _userService.ChangePassword(message.UserId,message.NewPassword,message.ChangeDate);
            _userService.SetResetPasswordToken(message.UserId,string.Empty);
    //was replaced with not qual (two update instead one)

            //var query = Query.EQ("_id", message.UserId);

            //var update = Update.Set(x => x.LastPasswordChangedDate, message.ChangeDate)
            //                   .Set(x => x.Password, message.NewPassword)
            //                   .Set(x => x.ResetPasswordToken, String.Empty);

            //_userService.Update(query, update);
        }

        public void Handle(User_Realestate_AddedEvent message)
        {
            var doc = new RealestateDocument
            {
                Id = message.Id,
                Name = message.Name,
                AmountInCents = message.AmountInCents,
                IsIncludedInWorth = false
            };
            _userService.AddRealestate(message.UserId, doc);
        }

        public void Handle(User_Realestate_IncludeInWorthEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.UserId), Query.EQ("Realestates._id", message.Id));
            var update = Update.Set("Realestates.$.IsIncludedInWorth", message.IsIncludedInWorth);
            _userService.Update(query, update);
        }

        public void Handle(User_Realestate_DeletedEvent message)
        {
            _userService.RemoveRealestate(message.UserId, message.Id);
        }


        public void Handle(User_WindwosLiveSignedupEvent message)
        {
            var doc = new IdentityDocument {Identity = message.Identifier};
            _userService.AddIdentity(message.UserId, doc);
        }

        public void Handle(User_FacebookSignedupEvent message)
        {
            var doc = new IdentityDocument { Identity = message.Identifier };
            _userService.AddIdentity(message.UserId, doc);
        }

        public void Handle(User_GoogleSignedupEvent message)
        {
            var doc = new IdentityDocument { Identity = message.Identifier };
            _userService.AddIdentity(message.UserId, doc);
        }

        public void Handle(User_TwitterSignedupEvent message)
        {
            var doc = new IdentityDocument { Identity = message.Identifier };
            _userService.AddIdentity(message.UserId, doc);
        }

        public void Handle(User_AddedFacebookAccountEvent message)
        {
            var identity = new IdentityDocument {Identity = message.Identifier, Provider = JanrainProviderType.Facebook};
            _userService.AddIdentity(message.UserId, identity);
        }

        public void Handle(User_AddedGoogleAccountEvent message)
        {          
            var identity = new IdentityDocument {Identity = message.Identifier, Provider = JanrainProviderType.Google};
            _userService.AddIdentity(message.UserId, identity);
        }

        public void Handle(User_AddedTwitterAccountEvent message)
        {
            var identity = new IdentityDocument {Identity = message.Identifier, Provider = JanrainProviderType.Twitter};
            _userService.AddIdentity(message.UserId, identity);
        }

        public void Handle(User_AddedWindowsLiveAccountEvent message)
        {
            var identity = new IdentityDocument
                               {Identity = message.Identifier, Provider = JanrainProviderType.WindowsLive};
            _userService.AddIdentity(message.UserId, identity);
        }


        public void Handle(User_AddedYodleeAccountEvent message)
        {
            var doc = new YodleeUserInfoDocument
                          {
                              EmailAddress = message.EmailAddress,
                              LastLoginTime = message.LastLoginTime,
                              LoginCount = message.LoginCount,
                              LoginName = message.LoginName,
                              Password = message.Password,
                              PasswordChangedOn = message.PasswordChangedOn,
                              PasswordExpiryDays = message.PasswordExpiryDays,
                              PasswordExpiryNotificationDays = message.PasswordExpiryNotificationDays,
                              PasswordRecovered = message.PasswordRecovered,
                              UserType = message.UserType
                          };
            _userService.SetYodleeUserInfo(message.UserId,doc);
        }

        public void Handle(User_Notification_UpdatedEvent message)
        {
            _userService.UpdateNotification(message);
        }

        public void Handle(User_Email_AddedEvent message)
        {
            _userService.AddEmail(message.UserId, message.Email);
        }

        public void Handle(User_Email_RemovedEvent message)
        {
            _userService.RemoveEmail(message.UserId, message.Email);
        }

        public void Handle(User_Phone_AddedEvent message)
        {
            _userService.AddPhone(message.UserId, message.Phone);
        }

        public void Handle(User_Phone_RemovedEvent message)
        {
            _userService.RemovePhone(message.UserId,message.Phone);
        }

        public void Handle(User_Subscription_CreatedEvent message)
        {
            var subscription = new SubscriptionDocument
                                   {
                                       Id = message.SubscriptionId,
                                       CreditIdentityId = message.CreditIdentityId
                                   };
            _userService.AddSubscription(message.UserId,subscription);

        }

        public void Handle(User_Subscription_SubscribedEvent message)
        {
            _userService.UpdateSubscribtion(message);
        }

        public void Handle(User_Subscription_DeletedEvent message)
        {
            _userService.RemoveSubscription(message.UserId, message.SubscriptionId,message.CreditIdentityId);
        }

        public void Handle(User_Subscription_ReceivedWebhookEvent message)
        {
            if (message.WebhookEvent == "payment_success" || message.WebhookEvent == "payment_failure")
            {
                var doc = new BillingDocument
                              {
                                  AmountInCents =
                                      long.Parse(message.WebhookParams["payload[transaction][amount_in_cents]"]),
                                  BillDate = DateTime.Parse(message.WebhookParams["payload[transaction][created_at]"]),
                                  //TODO: During subscribtion to chargify when user add new credit identity
                                  //TODO: we should send credit identity id to chargify and change logic in chargify controller (savewebhook and SubscribeUser)
                                  SubscriptionId = message.SubscriptionId,
                                  MaskedCreditCardNumber =
                                      message.WebhookParams["payload[subscription][credit_card][masked_card_number]"],
                                  ProductDescription =
                                      message.WebhookParams["payload[subscription][product][description]"],
                                  ProductId = message.WebhookParams["payload[subscription][product][id]"],
                                  Status =
                                      bool.Parse(message.WebhookParams["payload[transaction][success]"])
                                          ? "Success"
                                          : "Failure"
                              };
                _userService.AddBilling(message.UserId, doc);
            }

            if (message.WebhookEvent == "payment_failure")
            {
                var user = _userService.GetByChargifySystemId(message.SubscriptionId);
                var subscription = user.Subscriptions.Single(x => x.Id == message.SubscriptionId);

                var msg = new User_Subscription_DeletedEvent
                              {
                                  CancelMessage = message.WebhookEvent,
                                  CreditIdentityId = subscription.CreditIdentityId,
                                  SubscriptionId = message.SubscriptionId,
                                  UserId = user.Id
                              };

                _eventService.Send(msg);
            }

            //payment_failure
            //payment_success
            //
            //payload[subscription][product][description]
            //payload[subscription][product][id]

            //Following two params only for the payload payment
            //Payment amount:  	
            //true in case of success: payload[transaction][success]
            //We need this to identify next billing date:  
        }

        private static List<NotificationConfigDocument> GetDefaultNotifications()
        {
            return new List<NotificationConfigDocument>
                       {
                           new NotificationConfigDocument {Type = EmailTypeEnum.LowBalance, BorderValue = 10},
                           new NotificationConfigDocument {Type = EmailTypeEnum.LargePurchases, BorderValue = 1000},
                           //new NotificationConfigDocument {Type = EmailTypeEnum.BillReminder, BorderValue = 5},
                           new NotificationConfigDocument {Type = EmailTypeEnum.AvailableCredit, BorderValue = 500},
                           new NotificationConfigDocument {Type = EmailTypeEnum.UnusualSpending, BorderValue = 500},
                           new NotificationConfigDocument {Type = EmailTypeEnum.OverBudget},
                       };
        }

        public void Handle(User_SecuritySettingsUpdatedEvent message)
        {
            _userService.SetSecuritySettings(message.UserId, message.EnableAdminAccess, message.EnableIntuitLogging);
        }

        public void Handle(User_AutoUpdateDateSetEvent message)
        {
            _userService.SetLastAutoUpdateDate(message.UserId, message.Date);
        }

        public void Handle(User_ZipCode_ChangedEvent message)
        {
            var query = Query.EQ("_id", message.UserId);
            var update = Update<UserDocument>.Set(x => x.ZipCode, message.ZipCode);
            _userService.Update(query,update);
        }

        public void Handle(User_MerchantInfo_UpdatedEvent message)
        {
            UpdateUser(message.UserId, x => x.MerchantInfo = message.MerchantInfo);
        }

        public void Handle(User_Merchant_BillingInfo_UpdatedEvent message)
        {
            UpdateUser(message.UserId, x => x.BillingInfo = message.BillingInfo);
        }


        private void UpdateUser(string id, Action<UserDocument> updater)
        {
            var user = _userService.GetById(id);
            if (user != null)
            {
                updater(user);
                _userService.Save(user);
            }
        }
    }
}