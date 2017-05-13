using mPower.Domain.Accounting.Enums;
using System;
using System.Collections.Generic;

namespace Default.ViewModel.Areas.Shared
{
    public class Entry
    {
        public string TransactionId { get; set; }

        public string AccountId { get; set; }
        public string AccountName { get; set; }
        
        public string OffsetAccountName { get; set; }
        public string OffsetAccountId { get; set; }

        
        public bool IsBaseEntry { get; set; }
        public string Memo { get; set; }
        
        public DateTime BookedDate { get; set; }
        public string Payee { get; set; }
        public string FormattedAmountInDollars { get; set; }
        public AmountTypeEnum AmountType { get; set; }

        public List<InlineOffer> Offers { get; set; }


        public Entry()
        {
            Offers = new List<InlineOffer>();
        }
    }

    public class InlineOffer
    {
        public string AffiliateId { get; set; }
        public string CampaignId { get; set; }
        public string Headline { get; set; }
        public string Body { get; set; }
        public string LogoPath { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Terms { get; set; }
        public long OfferValueInCents { get; set; }
    }
}