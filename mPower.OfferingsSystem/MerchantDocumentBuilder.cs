using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using mPower.Framework;
using mPower.Framework.Extensions;
using mPower.OfferingsSystem.Data;
using mPower.TempDocuments.Server.Documents;
using mPower.OfferingsSystem;

namespace mPower.OfferingsSystem
{
    public class MerchantDocumentBuilder : BaseDocumentBuilder, IMerchantDocumentBuilder
    {
        private readonly AccessDataRepsitory _repsitory;
        private readonly Logger _logger = MPowerLogManager.CurrentLogger;

        public MerchantDocumentBuilder(AccessDataRepsitory repsitory)
        {
            _repsitory = repsitory;
        }

        public IEnumerable<MerchantDocument> GetAll()
        {
            var brands = _repsitory.GetBrands().ToList().Where(x=> x.BrandId.HasValue()).Distinct(x=> x.BrandId).ToDictionary(k => k.BrandId, v => v);      
            var emptyBrand = new Brand();
            var merchants = _repsitory.GetMerchants();
            foreach (var merchant in merchants)
            {
                MerchantDocument doc = null;
                try
                {
                    var brand = GetValueOrDefault(brands, merchant.BrandId, emptyBrand);
                    doc = new MerchantDocument
                    {
                        Id = merchant.LocationId,
                        Name = ParseString(merchant.LocationName),
                        LocationLogo = ParseString(merchant.LocationLogo),
                        Latitude = merchant.Latitude,
                        Longitude = merchant.Longitude,
                        Keywords = ParseString(merchant.Keywords),
                        City = ParseString(merchant.City),
                        Country = ParseString(merchant.Country),
                        LocationDescription = ParseString(merchant.LocationDescription) ?? ParseString(brand.Description),
                        LocationPhotos = ParseString(merchant.LocationPhotos),
                        LocationUrl = ParseString(merchant.LocationUrl),
                        Phone = ParseString(merchant.Phone),
                        PostalCode = ParseString(merchant.PostalCode),
                        ServiceArea = ParseString(merchant.ServiceArea),
                        State = ParseString(merchant.State),
                        Street =
                            String.Format("{0} {1}", ParseString(merchant.StreetLine1),
                                          ParseString(merchant.StreetLine2)),
                        BrandName = ParseString(brand.Name),
                        BrandLogo = ParseString(brand.LogoName)
                    };
                }
                catch (Exception)
                {
                    //_logger.ErrorException("Couldn't parse CSV record to MerchantDocument. Merchant Record: " + merchant.LocationId, ex);
                }
                if (doc!= null)
                {
                    yield return doc;
                }
            }
        }
    }

    public interface IMerchantDocumentBuilder
    {
        IEnumerable<MerchantDocument> GetAll();
    }
}