using System;
using System.Collections.Generic;
using System.Linq;

namespace mPower.Domain.Membership.User.Data
{
    public class RealestateRawData
    {
        public Region Region { get; set; }
        public Zestimate Zestimate { get; set; }
        public List<LocalRealEstateRegion> LocalRealEstates { get; set; }
        public uint Zpid { get; set; }
        public string FIPSCounty { get; set; }
        public string UseCode { get; set; }
        public string TaxAssessment { get; set; }
        public string TaxAssessmentYear { get; set; }
        public string YearBuilt { get; set; }
        public string LotSizeSqFt { get; set; }
        public string FinishedSqFt { get; set; }
        public string Bathrooms { get; set; }
        public string Bedrooms { get; set; }
        public string TotalRooms { get; set; }
        public Links Links { get; set; }
        public Address Address { get; set; }
        public string LastSoldDate { get; set; }
        public Amount LastSoldPrice { get; set; }

        public static RealestateRawData FromZillowObject(GetZestimateApi.DetailedProperty rawObject)
        {
            if (rawObject == null) { return null; }
            return new RealestateRawData
            {
                Region = Region.FromZillowObject(rawObject.regions),
                Zestimate = Zestimate.FromZillowObject(rawObject.zestimate),
                LocalRealEstates = rawObject.localRealEstate == null ? null : rawObject.localRealEstate.Select(LocalRealEstateRegion.FromZillowObject).ToList(),
                Zpid = rawObject.zpid,
                FIPSCounty = rawObject.FIPScounty,
                UseCode = rawObject.useCode,
                TaxAssessment = rawObject.taxAssessment,
                TaxAssessmentYear = rawObject.taxAssessmentYear,
                YearBuilt = rawObject.yearBuilt,
                LotSizeSqFt = rawObject.lotSizeSqFt,
                FinishedSqFt = rawObject.finishedSqFt,
                Bathrooms = rawObject.bathrooms,
                TotalRooms = rawObject.totalRooms,
                Links = Links.FromZillowObject(rawObject.links),
                Address = Address.FromZillowObject(rawObject.address),
                LastSoldDate = rawObject.lastSoldDate,
                LastSoldPrice = Amount.FromZillowObject(rawObject.lastSoldPrice),
            };
        }
    }

    public class Region
    {
        public string CityId { get; set; }
        public string CountyId { get; set; }
        public string StateId { get; set; }
        public string ZipCodeId { get; set; }

        public static Region FromZillowObject(GetZestimateApi.Regions rawObject)
        {
            if (rawObject == null) { return null; }
            return new Region
            {
                CityId = rawObject.cityid,
                CountyId = rawObject.countyid,
                StateId = rawObject.stateid,
                ZipCodeId = rawObject.zipcodeid,
            };
        }
    }

    public class Zestimate
    {
        public string LastUpdated { get; set; }
        public string Percentile { get; set; }
        public Amount Amount { get; set; }
        public AmountOptional OneWeekChange { get; set; }
        public AmountOptional ValueChange { get; set; }
        public ZestimateValuationRange ValuationRange { get; set; }

        public static Zestimate FromZillowObject(GetZestimateApi.Zestimate rawObject)
        {
            if (rawObject == null) { return null; }
            return new Zestimate
            {
                LastUpdated = rawObject.lastupdated,
                Percentile = rawObject.percentile,
                Amount = Amount.FromZillowObject(rawObject.amount),
                OneWeekChange = AmountOptional.FromZillowObject(rawObject.oneWeekChange),
                ValueChange = AmountOptional.FromZillowObject(rawObject.valueChange),
                ValuationRange = ZestimateValuationRange.FromZillowObject(rawObject.valuationRange),
            };
        }
    }

    public class Amount
    {
        public Currency Сurrency { get; set; }
        public string Value { get; set; }

        public static Amount FromZillowObject(GetZestimateApi.Amount rawObject)
        {
            if (rawObject == null) { return null; }
            return new Amount
            {
                Сurrency = CurrencyFromZillowCurrency(rawObject.currency),
                Value = rawObject.Value,
            };
        }

        public static Currency CurrencyFromZillowCurrency(GetZestimateApi.Currency rawObject)
        {
            switch (rawObject)
            {
                case GetZestimateApi.Currency.USD:
                    return Currency.USD;

                default:
                    throw new NotImplementedException();
            }
        }
    }

    public enum Currency
    {
        USD,
    }

    public class AmountOptional
    {
        public Currency Currency;
        public bool CurrencySpecified { get; set; }
        public string Duration { get; set; }
        public bool Deprecated;
        public bool DeprecatedSpecified { get; set; }
        public string Value { get; set; }

        public static AmountOptional FromZillowObject(GetZestimateApi.AmountOptional rawObject)
        {
            if (rawObject == null) { return null; }
            return new AmountOptional
            {
                Currency = Amount.CurrencyFromZillowCurrency(rawObject.currency),
                CurrencySpecified = rawObject.currencySpecified,
                Duration = rawObject.duration,
                Deprecated = rawObject.deprecated,
                DeprecatedSpecified = rawObject.deprecatedSpecified,
                Value = rawObject.Value,
            };
        }
    }

    public class ZestimateValuationRange
    {
        public Amount High { get; set; }
        public Amount Low { get; set; }

        public static ZestimateValuationRange FromZillowObject(GetZestimateApi.ZestimateValuationRange rawObject)
        {
            if (rawObject == null) { return null; }
            return new ZestimateValuationRange
            {
                High = Amount.FromZillowObject(rawObject.high),
                Low = Amount.FromZillowObject(rawObject.low),
            };
        }
    }

    public class LocalRealEstateRegion
    {
        public string ZindexValue { get; set; }
        public string ZindexOneYearChange { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public uint Id { get; set; }
        public LocalRealEstateRegionLinks Links { get; set; }

        public static LocalRealEstateRegion FromZillowObject(GetZestimateApi.LocalRealEstateRegion rawObject)
        {
            if (rawObject == null) { return null; }
            return new LocalRealEstateRegion
            {
                ZindexValue = rawObject.zindexValue,
                ZindexOneYearChange = rawObject.zindexOneYearChange,
                Name = rawObject.name,
                Type = rawObject.type,
                Id = rawObject.id,
                Links = LocalRealEstateRegionLinks.FromZillowObject(rawObject.links),
            };
        }
    }

    public class LocalRealEstateRegionLinks
    {
        public string Overview { get; set; }
        public string ForSale { get; set; }
        public string ForSaleByOwner { get; set; }

        public static LocalRealEstateRegionLinks FromZillowObject(GetZestimateApi.LocalRealEstateRegionLinks rawObject)
        {
            if (rawObject == null) { return null; }
            return new LocalRealEstateRegionLinks
            {
                Overview = rawObject.overview,
                ForSale = rawObject.forSale,
                ForSaleByOwner = rawObject.forSaleByOwner,
            };
        }
    }

    public class Links
    {
        public string HomeDetails { get; set; }
        public string GraphSandData { get; set; }
        public string MapThisHome { get; set; }
        public string MyEstimator { get; set; }
        public DeprecatedType MyZestimator { get; set; }
        public string Comparables { get; set; }

        public static Links FromZillowObject(GetZestimateApi.Links rawObject)
        {
            if (rawObject == null) { return null; }
            return new Links
            {
                HomeDetails = rawObject.homedetails,
                GraphSandData = rawObject.graphsanddata,
                MapThisHome = rawObject.mapthishome,
                MyEstimator = rawObject.myestimator,
                MyZestimator = DeprecatedType.FromZillowObject(rawObject.myzestimator),
                Comparables = rawObject.comparables,
            };
        }
    }

    public class DeprecatedType
    {
        public string Value { get; set; }
        public bool Deprecated { get; set; }

        public static DeprecatedType FromZillowObject(GetZestimateApi.DeprecatedType rawObject)
        {
            if (rawObject == null) { return null; }
            return new DeprecatedType
            {
                Value = rawObject.Value,
                Deprecated = rawObject.deprecated,
            };
        }
    }

    public class Address
    {
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public static Address FromZillowObject(GetZestimateApi.Address rawObject)
        {
            if (rawObject == null) { return null; }
            return new Address
            {
                Street = rawObject.street,
                ZipCode = rawObject.zipcode,
                City = rawObject.city,
                State = rawObject.state,
                Latitude = rawObject.latitude,
                Longitude = rawObject.longitude,
            };
        }
    }
}