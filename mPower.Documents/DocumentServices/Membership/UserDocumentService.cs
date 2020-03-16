using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Documents.Enums;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Events;
using mPower.Framework;
using mPower.Framework.Mongo;
using mPower.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoUpdate = MongoDB.Driver.Builders.Update;

namespace mPower.Documents.DocumentServices.Membership
{
    public class UserDocumentService : BaseDocumentService<UserDocument, UserFilter>
    {
        public UserDocumentService(MongoRead mongo)
            : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.Users; }
        }

        protected override IMongoSortBy BuildSortExpression(UserFilter filter)
        {
            return SortBy.Descending("CreateDate");
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(UserFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.UserName))
            {
                yield return Query.EQ("UserName", filter.UserName);
            }
            else if (!string.IsNullOrEmpty(filter.UserNameOrEmail))
            {
                yield return Query.Or(Query.EQ("UserName", filter.UserNameOrEmail), Query.EQ("Email", filter.UserNameOrEmail));
            }

            if (!string.IsNullOrEmpty(filter.Id))
            {
                yield return Query.EQ("_id", filter.Id);
            }

            if (!string.IsNullOrEmpty(filter.PasswordHash))
            {
                yield return Query.EQ("Password", filter.PasswordHash);
            }

            if (!string.IsNullOrEmpty(filter.AuthToken))
            {
                yield return Query.EQ("AuthToken", filter.AuthToken);
            }

            if (!string.IsNullOrEmpty(filter.Email))
            {
                yield return Query.EQ("Email", filter.Email);
            }

            if (!string.IsNullOrEmpty(filter.ResetPasswordToken))
            {
                yield return Query.EQ("ResetPasswordToken", filter.ResetPasswordToken);
            }

            if (!string.IsNullOrEmpty(filter.SocialIdentity))
            {
                yield return Query.EQ("Identities.Identity", filter.SocialIdentity);
            }

            if (!string.IsNullOrEmpty(filter.MobileAccessToken))
            {
                yield return Query.EQ("MobileAccessToken", filter.MobileAccessToken);
            }

            if (!string.IsNullOrEmpty(filter.AffiliateId))
            {
                yield return Query.EQ("ApplicationId", filter.AffiliateId);
            }
            if (filter.IsCreatedByAgent)
            {
                yield return Query.EQ("IsCreatedByAgent", filter.IsCreatedByAgent);
            }
            if (!string.IsNullOrEmpty(filter.CreatedBy))
            {
                yield return Query.EQ("CreatedBy", filter.CreatedBy);
            }

            if (filter.SubscriptionId != null)
            {
                yield return Query.EQ("Subscriptions.ChargifySubscriptionId", filter.SubscriptionId.Value);
            }

            if (!string.IsNullOrEmpty(filter.SearchKey))
            {
                var regexp = BsonRegularExpression.Create(filter.SearchKey, "i");

                yield return
                    Query.Or(
                    Query.Matches("FullNameLowerCase", regexp),
                    Query.Matches("UserName", regexp),
                    Query.Matches("ReferralCode", regexp));
            }
        }

        public UserDocument GetUserByUserName(string userName)
        {
            return GetByFilter(new UserFilter { UserName = userName }).FirstOrDefault();
        }

        public UserDocument Login(string userNameOrEmail, string passwordHash)
        {
            return GetByFilter(new UserFilter { UserNameOrEmail = userNameOrEmail, PasswordHash = passwordHash }).FirstOrDefault();
        }

        public UserDocument LoginByAuthToken(string authToken)
        {
            return GetByFilter(new UserFilter { AuthToken = authToken }).FirstOrDefault();
        }

        public UserDocument LoginByUserIdAndPassword(string userId, string passwordHash)
        {
            return GetByFilter(new UserFilter { Id = userId, PasswordHash = passwordHash }).FirstOrDefault();
        }

        public UserDocument GetUserByEmail(string email)
        {
            return GetByFilter(new UserFilter { Email = email }).FirstOrDefault();
        }

        public UserDocument GetUserByResetPasswordToken(string token)
        {
            return GetByFilter(new UserFilter { ResetPasswordToken = token }).FirstOrDefault();
        }

        public UserDocument GetUserBySocialIdentifier(string identity)
        {
            return GetByFilter(new UserFilter { SocialIdentity = identity }).FirstOrDefault();
        }

        public UserDocument GetUserByMobileAccessToken(string accessToken)
        {
            return GetByFilter(new UserFilter { MobileAccessToken = accessToken }).FirstOrDefault();
        }
        

        public UserDocument GetByChargifySystemId(string systemId)
        {
            var query = Query.EQ("Subscriptions._id", systemId);

           return GetByQuery(query).FirstOrDefault();
        }

        public void SetGoalsLinkedAccount(string userId, string ledgerId, string accountId)
        {
            var query = Query.EQ("_id", userId);
            var update = Update<UserDocument>.Set(x => x.GoalsLinkedAccount.LedgerId, ledgerId)
                .Set(x => x.GoalsLinkedAccount.AccountId, accountId);
            Update(query, update);
        }

        public void RemoveGoalsLinkedAccount(string ledgerId)
        {

            var query = Query.EQ("GoalsLinkedAccount.LedgerId", ledgerId);
            var update = Update<UserDocument>.Set(x => x.GoalsLinkedAccount.LedgerId, null)
                .Set(x => x.GoalsLinkedAccount.AccountId, null);
            Update(query, update);
        }

        public void RemoveGoalsLinkedAccount(string ledgerId, string accountId)
        {
            var query = Query.And(Query.EQ("GoalsLinkedAccount.LedgerId", ledgerId),
                Query.EQ("GoalsLinkedAccount.AccountId", accountId));
            var update = Update<UserDocument>.Set(x => x.GoalsLinkedAccount.LedgerId, null)
                .Set(x => x.GoalsLinkedAccount.AccountId, null);
            Update(query, update);
        }

        public UserDocument GetBySubscriptionId(int subscriptionId)
        {
            var query = Query.EQ("Subscriptions.ChargifySubscriptionId", subscriptionId);

            return GetByQuery(query).FirstOrDefault();
        }

        public void SetActivation(string userId, bool active)
        {
            var query = Query.EQ("_id", userId);
            var update = Update<UserDocument>.Set(x => x.IsActive, active);
            Update(query, update);
        }

        public void ChangePassword(string userId, string newPassword, DateTime changeDate)
        {
            var query = Query.EQ("_id", userId);
            var update = Update<UserDocument>
                .Set(x => x.LastPasswordChangedDate, changeDate)
                .Set(x => x.Password, newPassword);
            Update(query, update);
        }

        public void UpdateUser(User_UpdatedEvent message)
        {
            var query = Query.EQ("_id", message.UserId);
            var update = Update<UserDocument>
                .Set(x => x.Email, message.Email)
                .Set(x => x.FirstName, message.FirstName)
                .Set(x => x.LastName, message.LastName)
                .Set(x => x.ZipCode, message.ZipCode)
                .Set(x => x.BirthDate, message.BirthDate.HasValue ? message.BirthDate.Value.Date : (DateTime?)null)
                .Set(x => x.Gender, message.Gender)
                .Set(x => x.IsAgent, message.IsAgent);

            Update(query, update);
        }

        public void SetSecurityLevel(string userId, SecurityLevelEnum securityLevel)
        {
            var query = Query.EQ("_id", userId);
            var update = Update<UserDocument>.Set(x => x.SecurityLevel, securityLevel);
            Update(query, update);
        }

        public void AddPermissions(string userId, UserPermissionEnum permission)
        {
            var query = Query.EQ("_id", userId);
            var update = MongoUpdate
                .AddToSet("Permissions", permission);
            Update(query, update);
        }

        public void SetSequrityQuestion(string userId, string question, string answer)
        {
            var query = Query.EQ("_id", userId);
            var update = Update<UserDocument>
                .Set(x => x.PasswordAnswer, answer)
                .Set(x => x.PasswordQuestion, question);
            Update(query, update);
        }

        public void RemovePermissions(string userId, UserPermissionEnum permission)
        {
            var query = Query.EQ("_id", userId);
            var update = MongoUpdate
                .Pull("Permissions", permission);
            Update(query, update);
        }

        public void SetResetPasswordToken(string userId, string uniqueToken)
        {
            var query = Query.EQ("_id", userId);
            var update = Update<UserDocument>
                .Set(x => x.ResetPasswordToken, uniqueToken);
            Update(query, update);
        }

        public void AddRealestate(string userId, RealestateDocument doc)
        {
            var query = Query.EQ("_id", userId);
            var update = MongoUpdate.Push("Realestates", doc.ToBsonDocument());
            Update(query, update);
        }

        public void RemoveRealestate(string userId, string realestateId)
        {
            var query = Query.EQ("_id", userId);
            var innerQuery = Query.EQ("_id", realestateId);
            var update = MongoUpdate.Pull("Realestates", innerQuery);
            Update(query, update);
        }

        public void AddIdentity(string userId, IdentityDocument doc)
        {
            var query = Query.EQ("_id", userId);
            var update = MongoUpdate.Push("Identities", doc.ToBsonDocument());
            Update(query, update);
        }

        public void SetYodleeUserInfo(string userId, YodleeUserInfoDocument doc)
        {
            var query = Query.EQ("_id", userId);
            var update = Update<UserDocument>.Set(x => x.YodleeUserInfo, doc);
            Update(query, update);
        }

        public void UpdateNotification(User_Notification_UpdatedEvent message)
        {
            UpdateBuilder update = null;
            if (message.SendEmail.HasValue)
            {
                update = MongoUpdate.Set("Notifications.$.SendEmail", message.SendEmail);
            }
            if (message.SendText.HasValue)
            {
                update = update == null
                             ? MongoUpdate.Set("Notifications.$.SendText", message.SendText)
                             : update.Set("Notifications.$.SendText", message.SendText);
            }
            if (message.BorderValue.HasValue)
            {
                update = update == null
                             ? MongoUpdate.Set("Notifications.$.BorderValue", message.BorderValue)
                             : update.Set("Notifications.$.BorderValue", message.BorderValue);
            }
            if (update != null)
            {
                var query = Query.And(Query.EQ("_id", message.UserId), Query.EQ("Notifications._id", message.Type));
                Update(query, update);
            }
        }

        public void SetLastAutoUpdateDate(string userId, DateTime date)
        {
            var query = Query.EQ("_id", userId);
            var update = Update<UserDocument>.Set(x => x.LastAutoUpdateDate, date);
            Update(query, update);
        }

        public void IncreaseAggregatedAccountsCounter(string userId)
        {
            var query = Query.EQ("_id", userId);
            var update = MongoUpdate.Inc("AccountsAggregated", 1);
            Update(query, update);
        }

        public void SetSecuritySettings(string userId, bool enableAdminAccess, bool enableAggregationLogging, bool enableAgentAccess)
        {
            var query = Query.EQ("_id", userId);
            var update = Update<UserDocument>
                .Set(x => x.Settings.EnableAdminAccess, enableAdminAccess)
                .Set(x => x.Settings.EnableAgentAccess, enableAgentAccess)
                .Set(x => x.Settings.EnableIntuitLogging, enableAggregationLogging);
            Update(query, update);
        }

        public void AddBilling(string userId, BillingDocument doc)
        {
            var query = Query.And(Query.EQ("_id", userId));
            var update = MongoUpdate.Push("BillingsList", doc.ToBsonDocument());
            Update(query, update);
        }

        public void AddSubscription(string userId, SubscriptionDocument subscription)
        {
            var query = Query.EQ("_id", userId);
            var update = MongoUpdate.AddToSet("Subscriptions", subscription.ToBsonDocument())
                               .AddToSet("Permissions", UserPermissionEnum.ViewPfm);
            Update(query, update);
        }

        public void UpdateSubscribtion(User_Subscription_SubscribedEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.UserId), Query.EQ("Subscriptions._id", message.SubscriptionId));
            var update = MongoUpdate
            .Set("Subscriptions.$.ProductName", BsonValue.Create(message.ProductName) ?? BsonNull.Value)
            .Set("Subscriptions.$.ProductPriceInCents", message.ProductPriceInCents)
            .Set("Subscriptions.$.ProductHandle", BsonValue.Create(message.ProductHandle) ?? BsonNull.Value)
            .Set("Subscriptions.$.BillingAddress", BsonValue.Create(message.BillingAddress) ?? BsonNull.Value)
            .Set("Subscriptions.$.BillingCity", BsonValue.Create(message.BillingCity) ?? BsonNull.Value)
            .Set("Subscriptions.$.BillingCountry", BsonValue.Create(message.BillingCountry) ?? BsonNull.Value)
            .Set("Subscriptions.$.BillingState", BsonValue.Create(message.BillingState) ?? BsonNull.Value)
            .Set("Subscriptions.$.BillingZip", BsonValue.Create(message.BillingZip) ?? BsonNull.Value)
            .Set("Subscriptions.$.CVV", BsonValue.Create(message.CVV) ?? BsonNull.Value)
            .Set("Subscriptions.$.Email", BsonValue.Create(message.Email) ?? BsonNull.Value)
            .Set("Subscriptions.$.ExpirationMonth", message.ExpirationMonth)
            .Set("Subscriptions.$.ExpirationYear", message.ExpirationYear)
            .Set("Subscriptions.$.FirstName", BsonValue.Create(message.FirstName) ?? BsonNull.Value)
            .Set("Subscriptions.$.FirstNameCC", BsonValue.Create(message.FirstNameCC) ?? BsonNull.Value)
            .Set("Subscriptions.$.FullNumber", BsonValue.Create(message.FullNumber) ?? BsonNull.Value)
            .Set("Subscriptions.$.LastName", BsonValue.Create(message.LastName) ?? BsonNull.Value)
            .Set("Subscriptions.$.LastNameCC", BsonValue.Create(message.LastNameCC) ?? BsonNull.Value)
            .Set("Subscriptions.$.Organization", BsonValue.Create(message.Organization) ?? BsonNull.Value)
            .Set("Subscriptions.$.Status", SubscriptionStatusEnum.Success)
            .Set("Subscriptions.$.ChargifySubscriptionId", message.ChargifySubscriptionId);
            Update(query, update);
        }

        public void AddEmail(string userId, string email)
        {
            var query = Query.EQ("_id", userId);
            var update = MongoUpdate.Push("AdditionalEmails", email);
            Update(query, update);
        }

        public void RemoveEmail(string userId, string email)
        {
            var query = Query.EQ("_id", userId);
            var update = MongoUpdate.Pull("AdditionalEmails", email);
            Update(query, update);
        }

        public void AddPhone(string userId, string phone)
        {
            var query = Query.EQ("_id", userId);
            var update = MongoUpdate.Push("Phones", phone);
            Update(query, update);
        }

        public void RemovePhone(string userId, string phone)
        {
            var query = Query.EQ("_id", userId);
            var update = MongoUpdate.Pull("Phones", phone);
            Update(query, update);
        }

        public void RemoveSubscription(string userId, string subscriptionId, string creditIdentityId)
        {
            var query = Query.And(Query.EQ("_id", userId), Query.EQ("Subscriptions._id", subscriptionId));
            var update = MongoUpdate.Set("Subscriptions.$.Status", SubscriptionStatusEnum.Canceled);
            Update(query, update);
            if (String.IsNullOrEmpty(creditIdentityId)) // remove permission only if user cancel main product
            {
                var update2 = Update<UserDocument>
                    .Pull(x => x.Permissions, UserPermissionEnum.ViewPfm)
                    .Set(x => x.IsActive, false);
                Update(query, update2);
            }
        }

        public void SetLastLogin(string userId, DateTime date, string authToken)
        {
            var query = Query.EQ("_id", userId);
            var update = Update<UserDocument>
                .Set(x => x.LastLoginDate, date)
                .Set(x => x.AuthToken, authToken);
            Update(query, update);
        }

        public void SetLastMobileLogin(string userId, DateTime date, string accessToken)
        {
            var query = Query.EQ("_id", userId);
            var update = Update<UserDocument>
                .Set(x => x.LastLoginDate, date)
                .Set(x => x.MobileAccessToken, accessToken);
            Update(query, update);
        }
    }
}
