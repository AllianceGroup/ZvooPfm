using System;
using System.Collections.Generic;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Application.Enums;
using mPower.Framework;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate
{
    public class AffiliateAR : MpowerAggregateRoot
    {
        public AffiliateAR(string affiliateId, string name, ICommandMetadata metadata)
        {
            SetCommandMetadata(metadata);
            Apply(new Affiliate_CreatedEvent
            {
                Id = affiliateId,
                Name = name
            });
        }

        public void SynchronizeChargifyProducts(List<ChargifyProductData> products)
        {
            Apply(new Affiliate_SynchronizedChargifyProductsEvent
            {
                AffiliateId = _id,
                Products = products
            });
        }

        public void Update(AffiliateData data)
        {
            Apply(new Affiliate_UpdatedEvent
            {
                ApplicationId = data.ApplicationId,
                ApplicationName = data.ApplicationName,
                ChargifyApiKey = data.ChargifyApiKey,
                ChargifySharedKey = data.ChargifySharedKey,
                ChargifyUrl = data.ChargifyUrl,
                ContactPhoneNumber = data.ContactPhoneNumber,
                DisplayName = data.DisplayName,
                EmailSuffix = data.EmailSuffix,
                LegalName = data.LegalName,
                MembershipApiKey = data.MembershipApiKey,
                SmtpCredentialsEmail = data.SmtpCredentialsEmail,
                SmtpCredentialsUserName = data.SmtpCredentialsUserName,
                SmtpCredentialsPassword = data.SmtpCredentialsPassword,
                SmtpEnableSsl = data.SmtpEnableSsl,
                SmtpHost = data.SmtpHost,
                SmtpPort = data.SmtpPort,
                UrlPaths = data.UrlPaths,
                AssemblyName = data.AssemblyName,
                JanrainAppApiKey = data.JanrainAppApiKey,
                JanrainAppUrl = data.JanrainAppUrl,
                BfmEnabled = data.BfmEnabled,
                PfmEnabled = data.PfmEnabled,
                CreditAppEnabled = data.CreditAppEnabled,
                SavingsEnabled = data.SavingsEnabled,
                MarketingEnabled = data.MarketingEnabled,
                Address = data.Address,
                SignUpProductId = data.SignUpProductId,
                SignupProductIdWithTrial = data.SignupProductIdWithTrial,
                AdditionalCreditIdentityProduct = data.AdditionalCreditIdentityProduct,
                UpdateDate = data.UpdateDate,
                PublicUrl = data.PublicUrl,
            });
        }

        #region Email

        #region Template

        public void AddEmailTemplate(EmailTemplateData data)
        {
            Apply(new Affiliate_Email_Template_AddedEvent
            {
                Id = data.Id,
                AffiliateId = _id,
                Name = data.Name,
                Html = data.Html,
                IsDefault = data.IsDefault,
                CreationDate = data.CreationDate,
                Status = data.Status,
            });
        }

        public void UpdateEmailTemplate(string id, string name, string html, TemplateStatusEnum status)
        {
            Apply(new Affiliate_Email_Template_ChangedEvent
                      {
                          Id = id,
                          AffiliateId = _id,
                          Name = name,
                          Html = html,
                          Status = status,
                      });
        }

        public void DeleteEmailTemplate(string id)
        {
            Apply(new Affiliate_Email_Template_DeletedEvent
                      {
                          Id = id,
                          AffiliateId = _id,
                      });
        }

        #endregion

        #region FAQ

        public void AddFaq(FaqData data)
        {
            Apply(new Affiliate_Faq_AddedEvent
            {
                Id = data.Id,
                AffiliateId = _id,
                Name = data.Name,
                Html = data.Html,
                IsActive = data.IsActive,
                CreationDate = data.CreationDate,
            });
        }

        public void UpdateFaq(string id, string name, string html, bool isActive)
        {
            Apply(new Affiliate_Faq_UpdatedEvent
            {
                Id = id,
                AffiliateId = _id,
                Name = name,
                Html = html,
                IsActive = isActive,
            });
        }

        public void DeleteFaq(string id)
        {
            Apply(new Affiliate_Faq_DeletedEvent
            {
                Id = id,
                AffiliateId = _id,
            });
        }

        #endregion

        #region Segments

        public void AddSegment(SegmentData data)
        {
            Apply(new Affiliate_Segment_AddedEvent
            {
                Id = data.Id,
                AffiliateId = data.AffiliateId,
                AgeRangeTo = data.AgeRangeTo,
                AgeRangeFrom = data.AgeRangeFrom,
                CustomDateRangeEnd = data.CustomDateRangeEnd,
                CustomDateRangeStart = data.CustomDateRangeStart,
                DateRange = data.DateRange,
                Gender = data.Gender,
                Name = data.Name,
                Options = data.Options,
                MerchantSelections = data.MerchantSelections,
                SpendingCategories = data.SpendingCategories,
                State = data.State,
                ZipCodes = data.ZipCodes,
                CreatedDate = data.LastModifiedDate,
                MatchedUsers = data.MatchedUsers,
                Past30DaysGrowthInPct = data.Past30DaysGrowthInPct,
                Past60DaysGrowthInPct = data.Past60DaysGrowthInPct,
                Past90DaysGrowthInPct = data.Past90DaysGrowthInPct,
            });
        }

        public void UpdateSegment(SegmentData data)
        {
            Apply(new Affiliate_Segment_ChangedEvent
            {
                Id = data.Id,
                AffiliateId = data.AffiliateId,
                AgeRangeTo = data.AgeRangeTo,
                AgeRangeFrom = data.AgeRangeFrom,
                CustomDateRangeEnd = data.CustomDateRangeEnd,
                CustomDateRangeStart = data.CustomDateRangeStart,
                DateRange = data.DateRange,
                Gender = data.Gender,
                Name = data.Name,
                Options = data.Options,
                MerchantSelections = data.MerchantSelections,
                SpendingCategories = data.SpendingCategories,
                State = data.State,
                ZipCodes = data.ZipCodes,
                UpdatedDate = data.LastModifiedDate,
                MatchedUsers = data.MatchedUsers,
                Past30DaysGrowthInPct = data.Past30DaysGrowthInPct,
                Past60DaysGrowthInPct = data.Past60DaysGrowthInPct,
                Past90DaysGrowthInPct = data.Past90DaysGrowthInPct,
            });
        }

        public void ReestimateSegments(List<SegmentData> reestimatedSegments)
        {
            Apply(new Affiliate_Segments_ReestimatedEvent
            {
                AffiliateId = _id,
                ReestimatedSegments = reestimatedSegments,
            });
        }

        public void DeleteSegment(string id, string affiliateId)
        {
            Apply(new Affiliate_Segment_DeletedEvent
            {
                AffiliateId = affiliateId,
                Id = id
            });
        }

        #endregion

        #region Content

        public void AddEmailContent(EmailContentData data)
        {
            Apply(new Affiliate_Email_Content_AddedEvent
            {
                Id = data.Id,
                TemplateId = data.TemplateId,
                AffiliateId = _id,
                Name = data.Name,
                Subject = data.Subject,
                Html = data.Html,
                IsDefaultForEmailType = data.IsDefaultForEmailType,
                CreationDate = data.CreationDate,
                Status = data.Status,
            });
        }

        public void UpdateEmailContent(EmailContentData data)
        {
            Apply(new Affiliate_Email_Content_ChangedEvent
            {
                Id = data.Id,
                TemplateId = data.TemplateId,
                AffiliateId = _id,
                Name = data.Name,
                Subject = data.Subject,
                Html = data.Html,
                Status = data.Status,
            });
        }

        public void DeleteEmailContent(string id)
        {
            Apply(new Affiliate_Email_Content_DeletedEvent
            {
                Id = id,
                AffiliateId = _id,
            });
        }

        #endregion

        #region Notification

        public void AddNotificationTypeEmail(NotificationTypeEmailData data)
        {
            Apply(new Affiliate_NotificationTypeEmail_AddedEvent
            {
                AffiliateId = _id,
                Name = data.Name,
                EmailType = data.EmailType,
                EmailContentId = data.EmailContentId,
                Status = data.Status,
            });
        }

        public void UpdateNotificationTypeEmail(NotificationTypeEmailData data)
        {
            Apply(new Affiliate_NotificationTypeEmail_ChangedEvent
            {
                AffiliateId = _id,
                EmailType = data.EmailType,
                EmailContentId = data.EmailContentId,
                Status = data.Status,
            });
        }

        #endregion

        #endregion

        #region Campaigns

        public void CreateCampaign(string campaignId, string segmentId, CampaignOfferData offer)
        {
            Apply(new Affiliate_Campaign_CreatedEvent
            {
                AffiliateId = _id,
                CampaignId = campaignId,
                SegmentId = segmentId,
                Offer = offer,
            });
        }

        public void UpdateCampaignOffer(string campaignId, CampaignOfferData offer)
        {
            Apply(new Affiliate_Campaign_Offer_UpdatedEvent
            {
                AffiliateId = _id,
                CampaignId = campaignId,
                Offer = offer,
            });
        }

        public void UpdateCampaignSettings(string campaignId, CampaignSettingsData settings)
        {
            Apply(new Affiliate_Campaign_Settings_UpdatedEvent
            {
                AffiliateId = _id,
                CampaignId = campaignId,
                Settings = settings,
            });
        }

        public void ShowOffers(string userId, Dictionary<string, List<string>> shownAffiliateOffers)
        {
            Apply(new Affiliate_Offers_ShownToUserEvent
            {
                UserId = userId,
                UserAffiliateId = _id,
                ShownAffiliateOffers = shownAffiliateOffers,
            });
        }

        public void AcceptOffer(string userId, string offerAffiliateId, string offerId, DateTime date)
        {
            Apply(new Affiliate_Offer_AcceptedByUserEvent
            {
                UserAffiliateId = _id,
                UserId = userId,
                OfferAffiliateId = offerAffiliateId,
                OfferId = offerId,
                Date = date,
            });
        }

        #endregion

        public void Delete()
        {
            Apply(new Affiliate_DeleteEvent { Id = _id });
        }

        public AffiliateAR() { }

        #region Object Reconstruction

        protected void On(Affiliate_CreatedEvent created)
        {
            _id = created.Id;
        }

        #endregion
    }
}
