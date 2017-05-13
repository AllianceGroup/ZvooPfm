using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Default.Areas.Administration.Models;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices;
using mPower.Documents.Documents.Affiliate;
using mPower.Domain.Application.Enums;
using mPower.Framework.Mvc;
using mPower.Framework.Utils;

namespace Default.Factories.ViewModels
{
    class CampaignDto
    {
        public string AffiliateName { get; set; }
        public CampaignDocument Data { get; set; }
    }

    public class OfferViewModelFactory : 
        IObjectFactory<MyOffersFilter, MyOffersListingModel>,
        IObjectFactory<NetworkOffersFilter, NetworkOffersListingModel>
    {
        private readonly AffiliateDocumentService _affiliates;
        private readonly UploadUtil _uploadUtil;

        public OfferViewModelFactory(AffiliateDocumentService affiliates, UploadUtil uploadUtil)
        {
            _affiliates = affiliates;
            _uploadUtil = uploadUtil;
        }

        public MyOffersListingModel Load(MyOffersFilter filter)
        {
            var result = new MyOffersListingModel {Filter = filter, Offers = new List<OfferListItemShortModel>()};
            
            var affiliate = _affiliates.GetById(filter.AffiliateId);
            if (affiliate != null)
            {
                var campaigns = affiliate.Campaigns.Where(x => x.Offer != null);
                
                if (!string.IsNullOrEmpty(filter.SeachQuery))
                {
                    campaigns = campaigns.Where(x => x.Offer.Name.ToLowerInvariant().Contains(filter.SeachQuery.ToLowerInvariant()));
                }
                
                if (filter.OfferType.HasValue)
                {
                    campaigns = campaigns.Where(x => x.Offer.OfferType == filter.OfferType.Value);
                }
                
                if (filter.Status.HasValue)
                {
                    campaigns = campaigns.Where(x => x.GetCurrentStatus() == filter.Status.Value);
                }

                result.Offers = campaigns.Select(ToShortModel).ToList();
            }

            return result;
        }

        public NetworkOffersListingModel Load(NetworkOffersFilter filter)
        {
            var result = new NetworkOffersListingModel {Filter = filter, Offers = new List<OfferListItemModel>()};

            var searchAffiliatesQuery = Query.And(
                Query<AffiliateDocument>.NE(aff => aff.ApplicationId, filter.AffiliateId),
                Query<AffiliateDocument>.ElemMatch(aff => aff.Campaigns, qb => qb.And(qb.EQ(cmp => cmp.Settings.IsPublic, true), qb.EQ(cmp => cmp.Status, OfferStatusEnum.Active)))
            );
            var affiliates = _affiliates.GetByQuery(searchAffiliatesQuery);
            var networkOffers = affiliates.SelectMany(aff => aff.Campaigns.Where(cmp => cmp.IsPublic && cmp.Status == OfferStatusEnum.Active && cmp.Offer != null).Select(x => ToDto(x, aff)));
            if (!string.IsNullOrEmpty(filter.SeachQuery))
            {
                networkOffers = networkOffers.Where(x => x.Data.Offer.Name.ToLowerInvariant().Contains(filter.SeachQuery.ToLowerInvariant()));
            }
            if (!string.IsNullOrEmpty(filter.Merchat))
            {
                networkOffers = networkOffers.Where(x => x.Data.MatchesMerchantName(filter.Merchat));
            }
            if (filter.OfferType.HasValue)
            {
                networkOffers = networkOffers.Where(x => x.Data.Offer.OfferType == filter.OfferType.Value);
            }

            if (!string.IsNullOrEmpty(filter.Category))
            {
                networkOffers = networkOffers.Where(x => x.Data.MatchesCategory(filter.Category));
            }

            var networkOffersList = networkOffers.ToList();
            result.Offers = networkOffersList.Select(ToModel).ToList();
            result.Filter.Merchants = networkOffersList
                .Select(x => x.Data.Merchant).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x)
                .Select(x => new SelectListItem {Text = x, Value = x}).ToList();

            return result;
        }

        private static OfferListItemShortModel ToShortModel(CampaignDocument doc)
        {
            return new OfferListItemShortModel
            {
                CampaignId = doc.Id,
                Name = doc.Offer.Name,
                Public = doc.IsPublic ? "public" : "local",
                Merchant = doc.Merchant,
                Status = doc.GetCurrentStatus(),
                Delivery = GetDelivery(doc.Offer.OfferType),
            };
        }

        private static CampaignDto ToDto(CampaignDocument doc, AffiliateDocument affiliate)
        {
            return new CampaignDto
            {
                AffiliateName = affiliate.ApplicationName,
                Data = doc,
            };
        }

        private OfferListItemModel ToModel(CampaignDto doc)
        {
            return new OfferListItemModel
            {
                CampaignId = doc.Data.Id,
                Headline = doc.Data.Offer.Headline,
                Type = doc.Data.Offer.OfferType,
                Merchant = doc.Data.Merchant,
                Category = doc.Data.Category,
                Body = doc.Data.Offer.Body,
                ExpirationDate = doc.Data.Offer.ExpirationDate,
                OfferValueInCents = doc.Data.Offer.OfferValueInCents,
                OfferValueInPerc = doc.Data.Offer.OfferValueInCents,
                CrossAffiliateRedeemCostInCents = doc.Data.Settings.CrossAffiliateRedeemCostInCents ?? 0,
                LogoPath = _uploadUtil.GetCampaigLogoUrl(doc.AffiliateName, doc.Data.Id, doc.Data.Offer.Logo, OfferImageSizeEnum.NetworkOffers),
                RedeemedCount = doc.Data.Statistic.AcceptedByUsers.Count,
                RedeemedMax = doc.Data.Settings.MaxPurchases,
            };
        }

        private static string GetDelivery(OfferTypeEnum offerType)
        {
            switch (offerType)
            {
                case OfferTypeEnum.InlineTransaction:
                    return "inline trans.";

                default:
                    return offerType.ToString().ToLower();
            }
        }
    }
}