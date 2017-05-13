using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NLog;
using mPower.Framework;
using mPower.Framework.Extensions;
using mPower.Framework.Geo;
using mPower.OfferingsSystem.Data;
using mPower.TempDocuments.Server.DocumentServices;
using mPower.TempDocuments.Server.Documents;

namespace mPower.OfferingsSystem
{
    public class OfferDocumentsBuilder : BaseDocumentBuilder, IOfferDocumentsBuilder
    {
        private readonly AccessDataRepsitory _repsitory;
        private readonly IMerchantDocumentService _merchantDocumentService;
        private readonly Logger _logger = MPowerLogManager.CurrentLogger;

        public OfferDocumentsBuilder(AccessDataRepsitory repsitory, IMerchantDocumentService merchantDocumentService)
        {
            _repsitory = repsitory;
            _merchantDocumentService = merchantDocumentService;
        }

        public IEnumerable<OfferDocument> GetAll(PackageInfo packageInfo)
        {
            var offers = _repsitory.GetOffers();
            var categories = _repsitory.GetCategories().ToList().Where(x=> x.CategoryId.HasValue()).Distinct(x=> x.CategoryId).ToDictionary(x => x.CategoryId, v => v);
            var emptyCategory = new Category();
            var subscriptions = _repsitory.GetSubscriptions().Select(x => x.OfferId).ToList();
            var redeems = _repsitory.GetRedeems().Where(x=> x.RedeemId != null).Distinct(x=> x.RedeemId).ToDictionary(x => x.RedeemId);
            foreach (var offer in offers)
            {
                OfferDocument doc = null;
                try
                {
                    var merchantDocument = _merchantDocumentService.GetById(offer.LocationId);
                    if (merchantDocument == null)
                    {
                        continue;
                    }
                    var currentRedeems = offer.RedeemIdentifiers.Split(',').Select(redeemId => redeems[redeemId]);
                    var offerType = (OfferTypeEnum)Enum.Parse(typeof(OfferTypeEnum), offer.OfferType, true);
                    var expressionType = (ExpressionTypeEnum)Enum.Parse(typeof(ExpressionTypeEnum), offer.ExpressionType, true);
                    var category = GetValueOrDefault(categories, offer.CategoryId, emptyCategory);
                    var isActive = subscriptions.Contains(offer.OfferId);
                    doc = new OfferDocument
                                     {
                                         Id = offer.OfferId,
                                         AmountInCents = expressionType == ExpressionTypeEnum.Amount ? Convert.ToInt64(decimal.Parse(offer.Award) * 100) : 0,
                                         Discount = expressionType == ExpressionTypeEnum.Percent ? Convert.ToDouble(offer.Award) : 0,
                                         CategoryName = category.Name,
                                         Merchant = merchantDocument.Name,
                                         MerchantLogoImage = merchantDocument.LocationLogo ?? merchantDocument.BrandLogo,
                                         MerchantDescription = merchantDocument.LocationDescription,
                                         Description = ParseString(offer.Disclaimer),
                                         Title = ParseString(offer.Desciption),
                                         GeoLocation = ParseLocation(merchantDocument.Latitude, merchantDocument.Longitude),
                                         OfferType = offerType,
                                         ExpressionType = expressionType,
                                         Images = ParseNullable(offer.OfferPhotoNames, (s) => s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),new List<string>()),
                                         StartDate = ParseNullable(offer.StartDate, (s) => DateTime.ParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture), DateTime.MinValue),
                                         EndDate = ParseNullable(offer.EndDate, (s) => DateTime.ParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture), DateTime.MinValue),
                                         Phone = ParseString(merchantDocument.Phone),
                                         Terms = ParseString(offer.Terms),
                                         Street = merchantDocument.Street,
                                         City = ParseString(merchantDocument.City),
                                         Country = ParseString(merchantDocument.Country),
                                         Url = ParseString(merchantDocument.LocationUrl),
                                         PostalCode = ParseString(merchantDocument.PostalCode),
                                         State = ParseString(merchantDocument.State),
                                         DataId = ParseString(offer.OfferDataId),
                                         Redeems = currentRedeems.Select(MapRedeem).ToList(),
                                         Declined = !isActive,
                                         Exclusions = ParseExclusions(offer.DayExclusions,offer.MonthExclusions,offer.DateExclusions),
                                     };
                }
                catch (Exception)
                {
                    //_logger.ErrorException("Couldn't parse CSV offer record to OfferDocument. Offer Record: " + offer.OfferId, ex);
                }
                if (doc != null)
                {
                    yield return doc;
                }
            }
        }

        private Location ParseLocation(double? latitude, double? longitude)
        {
            return new Location(latitude.Value,longitude.Value);
        }

        private string ParseExclusions(string dayExclusions, string monthExclusions, string dateExclusions)
        {
            var day = ParseString(dayExclusions);
            var month = ParseString(monthExclusions);
            var date = ParseString(dateExclusions);
            return string.Join(",", day, month, date);
        }

        private RedeemDocument MapRedeem(Redeem redeem)
        {
            return new RedeemDocument
                       {
                           Id = redeem.RedeemId,
                           Method = ParseRedeemMethod(redeem.Method),
                           Instruction = ParseString(redeem.Instruction),
                           Code = ParseString(redeem.Code),
                           CouponName = ParseString(redeem.CouponName)
                       };
        }

        public RedeemMethod ParseRedeemMethod(string method)
        {
            switch (method)
            {
                case "INSTORE_PRINT_COUPON":
                    return RedeemMethod.Print;
                case "ONLINE_LINK":
                    return RedeemMethod.Online;
                case "PHONE_VOICE":
                    return RedeemMethod.Phone;
                case "INSTORE_DIGITAL_COUPON":
                    return RedeemMethod.Digital;
            }
            throw new ArgumentOutOfRangeException("method", "Invalid Redeem Method");
        }
    }
    public class BaseDocumentBuilder
    {
        protected static T ParseNullable<T>(string value, Func<string, T> parse, T nullValue)
        {
            return value == "NULL" || string.IsNullOrEmpty(value) ? nullValue : parse(value);
        }

        protected static string ParseString(string value)
        {
            return ParseNullable(value, (s) => s, null);
        }

        protected static TValue GetValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dictionary,TKey key, TValue defValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defValue;
        }
    }
}