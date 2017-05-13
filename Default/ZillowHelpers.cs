using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.RealestateController;
using mPower.Domain.Membership.User.Data;
using mPower.Framework;
using Zillow.Net;

namespace Default
{
    public class ZillowHelpers
    {
        private readonly ZillowService _zillowService;

        public ZillowHelpers(MPowerSettings mPowerSettings)
        {
            _zillowService = new ZillowService();
            _zillowService.Initialize(mPowerSettings.ZillowWebServiceId);
        }

        public List<SearchResultsItem> Search(string address, string zip)
        {
            var properties = _zillowService.GetSearchResults(address, zip).Take(4).ToList();
            return properties.Select(Map).ToList();
        }

        public PropertyModel GetZestimate(uint zillowId)
        {
            var model = new PropertyModel();
            var property = _zillowService.GetZestimate(zillowId);
            if (property != null)
            {
                if (property.localRealEstate.Count > 0)
                {
                    var estate = property.localRealEstate.First();
                    model.Name = estate.name;
                }
                decimal amountInDollars;
                if (decimal.TryParse(property.zestimate.amount.Value, out amountInDollars))
                {
                    model.Value = amountInDollars;
                }
                model.ZillowId = zillowId;
            }
            return model;
        }

        public RealestateRawData GetRealestateRawData(uint zillowId)
        {
            var property = _zillowService.GetZestimate(zillowId);
            return RealestateRawData.FromZillowObject(property);
        }

        private static SearchResultsItem Map(SearchResultsApi.SimpleProperty property)
        {
            var rawAddress = property.address;
            var address = $"{rawAddress.street}, {rawAddress.city}, {rawAddress.state} {rawAddress.zipcode}";
            var descr =
                $"{NanIfEmpty(property.bedrooms)} BR / {NanIfEmpty(property.bathrooms)} BA / {NanIfEmpty(property.FIPScounty)} / {NanIfEmpty(property.finishedSqFt)} SQ. FT.";

            return new SearchResultsItem
            {
                Id = property.zpid,
                Address = address,
                Description = descr,
            };
        }

        private static string NanIfEmpty(string value)
        {
            return string.IsNullOrEmpty(value) ? "n/a" : value;
        }
    }
}
