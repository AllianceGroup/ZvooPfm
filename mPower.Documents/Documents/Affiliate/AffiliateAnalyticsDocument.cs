using MongoDB.Bson.Serialization.Attributes;
using mPower.Documents.Documents.Membership;
using System.Collections.Generic;

namespace mPower.Documents.Documents.Affiliate
{
    public class AffiliateAnalyticsDocument
    {
        [BsonId]
        public string Id { get; set; }
        public SpentStatiscticData BalanceAdjustmentStatisctics { get; set; }
        public Dictionary<string, SpentStatiscticData> UserIncomeStatisctics { get; set; }
        public Dictionary<string, SpentStatiscticData> AvailableCashStatisctics { get; set; }
        public SpentStatiscticData AvailableCreditStatisctics { get; set; }
        public Dictionary<string, SpentStatiscticData> UserDebtStatisctics { get; set; }
        
        public long TotalMoneyManagedInCents { get; set; }
        public long AvgUserAnnualIncomeInCents { get; set; }
        public long AvailableCashInCents { get; set; }
        public long AvailableCreditInCents { get; set; }
        public long TotalUserDebtInCents { get; set; }

        public AffiliateAnalyticsDocument()
        {
            BalanceAdjustmentStatisctics = new SpentStatiscticData();
            UserIncomeStatisctics = new Dictionary<string, SpentStatiscticData>();
            AvailableCashStatisctics = new Dictionary<string, SpentStatiscticData>();
            AvailableCreditStatisctics = new SpentStatiscticData();
            UserDebtStatisctics = new Dictionary<string, SpentStatiscticData>();
        }
    }
}