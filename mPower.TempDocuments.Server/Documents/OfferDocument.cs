using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Framework.Geo;

namespace mPower.TempDocuments.Server.Documents
{
    public class OfferDocument
    {
        private string _merchant;

        [BsonId]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Terms { get; set; }
        public string CategoryName { get; set; }
        public string Brand { get; set; }
        public string Merchant
      
        {
            get { return _merchant; }
            set { _merchant = value; MerchantIndex = new StringValueIndex(value);}
        }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long AmountInCents { get; set; }
        public double Discount { get; set; }
        public OfferTypeEnum OfferType { get; set; }
        public ExpressionTypeEnum ExpressionType { get; set; }
        public List<string>Images { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public bool Declined { get; set; }
        public string Url { get; set; }
        public string MerchantLogoImage { get; set; }
        public Location GeoLocation { get; set; }
        public string MerchantDescription { get; set; }
        public string ChannelId { get; set; }
        public string DataId { get; set; }
        public List<RedeemDocument> Redeems { get; set; }
        public string Exclusions { get; set; }
        public string MemberId { get; set; }
        public StringValueIndex MerchantIndex { get; set; }

        public string GetFormatedAward()
        {
            switch (ExpressionType)
            {
                case ExpressionTypeEnum.Percent:
                    return String.Format("{0}%", Discount);
                case ExpressionTypeEnum.Amount:
                    return (AmountInCents / 100m).ToString("C0");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public class StringValueIndex
    {
        public string OriginalValue { get; set; }
        public string ValueInLower { get; set; }
        public string First5 { get; set; }
        public string[] Words { get; set; }

        public StringValueIndex(string originalValue)
        {
            OriginalValue = originalValue;
            ValueInLower = originalValue.ToLower();
            Words = ValueInLower.Split(' ');
            First5 = new string(ValueInLower.Take(5).ToArray());
        }
    }

    public class RedeemDocument
    {
        public string Id { get; set; }

        public RedeemMethod Method { get; set; }

        public string Instruction { get; set; }

        public string Code { get; set; }

        public string CouponName { get; set; }
    }

    public enum RedeemMethod
    {
        Print,
        Online,
        Phone,
        Digital
    }

    public enum ExpressionTypeEnum
    {
        Percent = 0,
        Amount = 1
    }

    public enum OfferTypeEnum
    {
        Discount = 0,
        Reward = 1
    }
}