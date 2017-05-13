using mPower.Domain.Application.Enums;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Data;
using mPower.Domain.Membership.User.Events;
using mPower.Framework;
using System;
using System.Collections.Generic;

namespace mPower.Domain.Membership.User
{
    public class UserAR : MpowerAggregateRoot
    {
        public PermissionCollection Permissions = new PermissionCollection();

        public UserAR(UserData data)
        {
            SetCommandMetadata(data.Metadata);

            Apply(new User_CreatedEvent
            {
                Email = data.Email,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Password = data.PasswordHash,
                UserId = data.UserId,
                UserName = data.UserName,
                CreateDate = data.CreateDate,
                IsActive = data.IsActive,
                ApplicationId = data.ApplicationId,
                ZipCode = data.ZipCode,
                BirthDate = data.BirthDate,
                Gender = data.Gender,
                ReferralCode = data.ReferralCode,
            });
        }

        public void Delete()
        {
            Apply(new User_DeletedEvent
            {
                UserId = _id
            });
        }

        public void ChangePassword(string password, DateTime changeDate)
        {
            Apply(new User_PasswordChangedEvent
            {
                NewPassword = password,
                UserId = _id,
                ChangeDate = changeDate
            });
        }

        public void ResetPassword(string password, DateTime changeDate)
        {
            Apply(new User_PasswordResettedEvent
            {
                NewPassword = password,
                UserId = _id,
                ChangeDate = changeDate
            });
        }

        public void UpdateResetPasswordToken(string token)
        {
            Apply(new User_UpdatedResetPasswordTokenEvent
            {
                UniqueToken = token,
                UserId = _id
            });
        }

        public void Update(string firstName, string lastName, string email, string zipCode, DateTime? birthDate, GenderEnum? gender)
        {
            Apply(new User_UpdatedEvent
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                UserId = _id,
                ZipCode = zipCode,
                BirthDate = birthDate,
                Gender = gender,
            });
        }

        public void LogIn(DateTime date, string authToken, string affiliateName, string affiliateId, string userEmail, string userName)
        {
            Apply(new User_LoggedInEvent
            {
                Date = date,
                UserId = _id,
                AuthToken = authToken,
                AffiliateName = affiliateName,
                UserEmail = userEmail,
                UserName = userName,
                AffiliateId = affiliateId
            });
        }

        public void LogInMobile(DateTime date, string accessToken, string affiliateName, string affiliateId, string userEmail, string userName)
        {
            Apply(new User_MobileLoggedInEvent
            {
                Date = date,
                UserId = _id,
                AccessToken = accessToken,
                AffiliateName = affiliateName,
                UserEmail = userEmail,
                UserName = userName,
                AffiliateId = affiliateId
            });
        }

        public void Activate(bool isAdmin)
        {
            Apply(new User_ActivatedEvent
            {
                UserId = _id,
                IsAdmin = isAdmin
            });
        }

        public void Deacivate(bool isAdmin)
        {
            Apply(new User_DeactivatedEvent
            {
                UserId = _id,
                IsAdmin = isAdmin
            });
        }

        public void UpdateSecurityLevel(SecurityLevelEnum securityLevel)
        {
            Apply(new User_UpdatedSecurityLevelEvent
            {
                UserId = _id,
                SecurityLevel = securityLevel,
            });
        }

        public void UpdateSecurityQuestion(string question, string answer)
        {
            Apply(new User_UpdatedSecurityQuestionEvent
            {
                Answer = answer,
                Question = question,
                UserId = _id
            });
        }

        public void SetZipCode(string zipCode)
        {
            Apply(new User_ZipCode_ChangedEvent
                      {
                          UserId = _id,
                          ZipCode = zipCode
                      });
        }

        public void AddPermission(UserPermissionEnum permission)
        {
            if (!Permissions.HasPermission(permission))
            {
                Apply(new User_PermissionAddedEvent
                {
                    Permission = permission,
                    UserId = _id
                });
            }
        }

        public void RemovePermission(UserPermissionEnum permission)
        {
            if (Permissions.HasPermission(permission))
            {
                Apply(new User_PermissionRemovedEvent
                {
                    Permission = permission,
                    UserId = _id
                });
            }
        }

        #region Additional Information

        public void AddEmail(string email)
        {
            Apply(new User_Email_AddedEvent
            {
                UserId = _id,
                Email = email,
            });
        }

        public void RemoveEmail(string email)
        {
            Apply(new User_Email_RemovedEvent
            {
                UserId = _id,
                Email = email,
            });
        }

        public void AddPhone(string phone)
        {
            Apply(new User_Phone_AddedEvent
            {
                UserId = _id,
                Phone = phone,
            });
        }

        public void RemovePhone(string phone)
        {
            Apply(new User_Phone_RemovedEvent
            {
                UserId = _id,
                Phone = phone,
            });
        }

        #endregion

        #region Social Signup

        public void GoogleSignup(GoogleSignupData data)
        {
            Apply(new User_GoogleSignedupEvent
            {
                DisplayName = data.DisplayName,
                Email = data.Email,
                FamilyName = data.FamilyName,
                FormattedName = data.FormattedName,
                GivenName = data.GivenName,
                GoogleUserId = data.GoogleUserId,
                Identifier = data.Identifier,
                PreferredUsername = data.PreferredUsername,
                ProfileUrl = data.ProfileUrl,
                UserId = _id,
                VerifiedEmail = data.VerifiedEmail
            });
        }

        public void FacebookSignup(FacebookSignupData data)
        {
            Apply(new User_FacebookSignedupEvent
            {
                DisplayName = data.DisplayName,
                Email = data.Email,
                FamilyName = data.FamilyName,
                FormattedName = data.FormattedName,
                GivenName = data.GivenName,
                Identifier = data.Identifier,
                PreferredUsername = data.PreferredUsername,
                ProfileUrl = data.ProfileUrl,
                UserId = _id,
                VerifiedEmail = data.VerifiedEmail,
                Gender = data.Gender,
                PhotoUrl = data.PhotoUrl
            });
        }

        public void TwitterSignup(TwitterSignupData data)
        {
            Apply(new User_TwitterSignedupEvent
            {
                DisplayName = data.DisplayName,
                FormattedName = data.FormattedName,
                Identifier = data.Identifier,
                PreferredUsername = data.PreferredUsername,
                ProfileUrl = data.ProfileUrl,
                UserId = _id,
                PhotoUrl = data.PhotoUrl
            });
        }

        public void WindowsLiveSignup(WindowsLiveSignupData data)
        {
            Apply(new User_WindwosLiveSignedupEvent
            {
                DisplayName = data.DisplayName,
                FormattedName = data.FormattedName,
                Identifier = data.Identifier,
                PreferredUsername = data.PreferredUsername,
                ProfileUrl = data.ProfileUrl,
                UserId = _id,
                Email = data.Email,
                FamilyName = data.FamilyName,
                GivenName = data.GivenName
            });
        }

        public void AddGoogleAccount(GoogleSignupData data)
        {
            Apply(new User_AddedGoogleAccountEvent
            {
                DisplayName = data.DisplayName,
                Email = data.Email,
                FamilyName = data.FamilyName,
                FormattedName = data.FormattedName,
                GivenName = data.GivenName,
                GoogleUserId = data.GoogleUserId,
                Identifier = data.Identifier,
                PreferredUsername = data.PreferredUsername,
                ProfileUrl = data.ProfileUrl,
                UserId = _id,
                VerifiedEmail = data.VerifiedEmail
            });
        }

        public void AddYodleeAccount(YodleeSignUpData message)
        {
            Apply(new User_AddedYodleeAccountEvent
            {
                UserId = _id,
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

            });
        }

        public void AddFacebookAccount(FacebookSignupData data)
        {
            Apply(new User_AddedFacebookAccountEvent
            {
                DisplayName = data.DisplayName,
                Email = data.Email,
                FamilyName = data.FamilyName,
                FormattedName = data.FormattedName,
                GivenName = data.GivenName,
                Identifier = data.Identifier,
                PreferredUsername = data.PreferredUsername,
                ProfileUrl = data.ProfileUrl,
                UserId = _id,
                VerifiedEmail = data.VerifiedEmail,
                Gender = data.Gender,
                PhotoUrl = data.PhotoUrl
            });
        }

        public void AddTwitterAccount(TwitterSignupData data)
        {
            Apply(new User_AddedTwitterAccountEvent
            {
                DisplayName = data.DisplayName,
                FormattedName = data.FormattedName,
                Identifier = data.Identifier,
                PreferredUsername = data.PreferredUsername,
                ProfileUrl = data.ProfileUrl,
                UserId = _id,
                PhotoUrl = data.PhotoUrl
            });
        }

        

        public void AddWindowsLiveAccount(WindowsLiveSignupData data)
        {
            Apply(new User_AddedWindowsLiveAccountEvent
            {
                DisplayName = data.DisplayName,
                FormattedName = data.FormattedName,
                Identifier = data.Identifier,
                PreferredUsername = data.PreferredUsername,
                ProfileUrl = data.ProfileUrl,
                UserId = _id,
                Email = data.Email,
                FamilyName = data.FamilyName,
                GivenName = data.GivenName
            });
        }

        #endregion

        #region Notifications

        public void UpdateNotification(NotificationData data)
        {
            Apply(new User_Notification_UpdatedEvent
                      {
                          UserId = _id,
                          Type = data.Type,
                          SendEmail = data.SendEmail,
                          SendText = data.SendText,
                          BorderValue = data.BorderValue,
                      });
        }

        #endregion

        #region Subscriptions

        public void AddSubscription(string subscriptionId, string creditIdentityId)
        {
            Apply(new User_Subscription_CreatedEvent
            {
                SubscriptionId = subscriptionId,
                UserId = _id,
                CreditIdentityId = creditIdentityId
            });
        }

        public void ActivateSubscription(ProductData productData, CreditCardData creditCardData, CustomerData customerData, string subscriptionId, int chargifySubscriptionId)
        {
            Apply(new User_Subscription_SubscribedEvent
            {
                ProductName = productData.Name,
                ProductPriceInCents = productData.PriceInCents,
                ProductHandle = productData.Handle,

                BillingAddress = creditCardData.BillingAddress,
                BillingCity = creditCardData.BillingCity,
                BillingCountry = creditCardData.BillingCountry,
                BillingState = creditCardData.BillingState,
                BillingZip = creditCardData.BillingZip,
                CVV = creditCardData.CVV,
                ExpirationMonth = creditCardData.ExpirationMonth,
                ExpirationYear = creditCardData.ExpirationYear,
                FirstNameCC = creditCardData.FirstName,
                LastNameCC = creditCardData.LastName,
                FullNumber = creditCardData.FullNumber,

                Email = customerData.Email,
                FirstName = customerData.FirstName,
                LastName = customerData.LastName,
                Organization = customerData.Organization,
                UserId = _id,
                SubscriptionId = subscriptionId,
                ChargifySubscriptionId = chargifySubscriptionId
            });
        }

        public void DeleteSubscription(string cancelMessage, string subscriptionId, string creditIdentityId)
        {
            Apply(new User_Subscription_DeletedEvent
            {
                CancelMessage = cancelMessage,
                SubscriptionId = subscriptionId,
                UserId = _id,
                CreditIdentityId = creditIdentityId
            });
        }

        #endregion

        #region Realestates

        public void AddRealestate(RealestateData data)
        {
            Apply(new User_Realestate_AddedEvent
                      {
                          Id = data.Id,
                          UserId = _id,
                          Name = data.Name,
                          AmountInCents = data.AmountInCents,
                          RawData = data.RawData,
                      });
        }

        public void DeleteRealestate(string realestateId)
        {
            Apply(new User_Realestate_DeletedEvent
                      {
                          Id = realestateId,
                          UserId = _id,
                      });
        }

        public void IncludeInWorthRealestate(string userId, string realestateId, bool isIncludedInWorth)
        {
            Apply(new User_Realestate_IncludeInWorthEvent
            {
                Id = realestateId,
                UserId = userId,
                IsIncludedInWorth = isIncludedInWorth
            });
        }

        #endregion

        #region Merchants

        public void SetMerchantInfo(MerchantData data)
        {
            Apply(new User_MerchantInfo_UpdatedEvent {UserId = _id, MerchantInfo = data});
        }
        
        public void SetMerchantBillingInfo(BillingData data)
        {
            Apply(new User_Merchant_BillingInfo_UpdatedEvent {UserId = _id, BillingInfo = data});
        }

        #endregion

        public void ReceiveWebHook(string subscriptionId,string customerId, string chargifyWebhookId, DateTime date, Dictionary<string, string> webhookParams, string webhookEvent)
        {
            Apply(new User_Subscription_ReceivedWebhookEvent
            {
                SubscriptionId = subscriptionId,
                ChargifyWebhookId = chargifyWebhookId,
                CustomerId = customerId,
                Date = date,
                WebhookEvent = webhookEvent,
                WebhookParams = webhookParams,
                Id = null,
                UserId = _id
            });
        }

        public void SetGoalsLinkedAccount(string ledgerId, string accountId)
        {
            Apply(new User_GoalsLinkedAccount_SetEvent
            {
                UserId = _id,
                LedgerId = ledgerId,
                AccountId = accountId,
            });
        }

        public void UpdateSecuritySettings(bool enableAdminAccess, bool intuitLoggingEnabled)
        {
            Apply(new User_SecuritySettingsUpdatedEvent
                      {
                          UserId = _id,
                          EnableAdminAccess = enableAdminAccess,
                          EnableIntuitLogging = intuitLoggingEnabled
                      });
        }

        public void SetAutoUpdateDate(DateTime date)
        {
            Apply(new User_AutoUpdateDateSetEvent
                      {
                          UserId = _id,
                          Date = date
                      });
        }

        /// <summary>
        /// For object reconstraction
        /// </summary>
        public UserAR() { }

        #region Object Reconstruction

        protected void On(User_CreatedEvent created)
        {
            _id = created.UserId;
        }

        protected void On(User_PermissionAddedEvent permissionAdded)
        {
            Permissions.Add(permissionAdded.Permission);
        }

        protected void On(User_PermissionRemovedEvent permissionRemoved)
        {
            Permissions.Remove(permissionRemoved.Permission);
        }

        #endregion
    }
}
