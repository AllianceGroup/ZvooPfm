using Default.Areas.Administration.Models;
using mPower.Domain.Application.Enums;
using mPower.Framework.Utils;
using mPower.Framework.Utils.Extensions;
using System.Linq;
using System.Web.Mvc;

namespace Default.Validations
{
    public class CampaignValidator
    {
        private readonly UploadUtil _uploadUtil;

        public CampaignValidator(UploadUtil uploadUtil)
        {
            _uploadUtil = uploadUtil;
        }

        public void Validate(CampaignSettingsModel model, ModelStateDictionary modelState)
        {
            if (model.StartDate.HasValue && model.EndDate.HasValue && model.EndDate.Value < model.StartDate.Value)
            {
                modelState.AddModelError("EndDate", string.Format("End Date should be greater than Start Date."));
            }

            if (model.IsPublic && !model.CrossAffiliateRedeemCostInDollars.HasValue)
            {
                modelState.AddModelError("CrossAffiliateRedeemCostInDollars", "For public offers amount that you a willing to pay is required.");
            }

            if ((model.NotifyPerPurchase || model.NotifyWhenPurchasesNumberExceeded || model.SendPurchaseSummaryReport) && string.IsNullOrEmpty(model.Email))
            {
                modelState.AddModelError("Email", "Email field is required.");
            }

            if (model.NotifyWhenPurchasesNumberExceeded && (!model.NotificationPurchasesCountBorderValue.HasValue || model.NotificationPurchasesCountBorderValue.Value < 0))
            {
                modelState.AddModelError("NotificationPurchasesCountBorderValue", string.Format("Non-negative value is requered for 'notify me when purchases exceed'."));
            }

            if (model.OfferType == OfferTypeEnum.InlineTransaction)
            {
                var anyMerchantSpecified = model.Merchants.Any(x => !string.IsNullOrEmpty(x));
                var categorySpecified = !string.IsNullOrEmpty(model.Category);
                if (!(anyMerchantSpecified || categorySpecified))
                {
                    modelState.AddModelError("Merchants[0]", "For offers with type '" + OfferTypeEnum.InlineTransaction.GetDescription().ToLowerInvariant() + "' any merchant or spent category is required.");
                }
                if (anyMerchantSpecified && categorySpecified)
                {
                    modelState.AddModelError("Merchants[0]", "Specify either a merchants' list or a spending category.");
                }
            }
        }

        public void Validate(CampaignOfferModel model, ModelStateDictionary modelState)
        {
            if (model.LogoFile != null && !_uploadUtil.IsImage(model.LogoFile))
            {
                modelState.AddModelError("Logo", "Only images are acceptable.");
            }

            if (model.OfferType != OfferTypeEnum.Sms && string.IsNullOrEmpty(model.Headline))
            {
                modelState.AddModelError("Headline", "For offers which type is not '" + OfferTypeEnum.Sms.GetDescription().ToLowerInvariant() + "' field 'Headline' is required.");
            }

            if (!model.OfferValueInPerc.HasValue && !model.OfferValueInDollars.HasValue)
            {
                modelState.AddModelError("OfferValueInPerc", "Offer value in percents or in dollars is required.");
            }
        }
    }
}