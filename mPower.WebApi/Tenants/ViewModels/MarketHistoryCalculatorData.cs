using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mPower.WebApi.Tenants.ViewModels
{
    public class MarketHistoryCalculatorData
    {
       public MarketHistoryCalculatorData()
        {
            ListofData = new List<ListofMarketHistoryData>();
            SummaryData = new List<SummaryOfMarketHistoryData>();
        }

        public int FirstYear { get; set; }
        public int LastYear { get; set; }
        public int PresentValue { get; set; }
        public int AnnualPayment { get; set; }

        public List<ListofMarketHistoryData> ListofData { get; set; }
        public List<SummaryOfMarketHistoryData> SummaryData { get; set; }
    }

    public class ListofMarketHistoryData
    {
       // public string _id { get; set; }
        public string Year { get; set; }
        public double DJI { get; set; }
        public double SPWithoutDividends { get; set; }
        public double SPWithDividends { get; set; }
    }

    public class SummaryOfMarketHistoryData
    {
        public string Year { get; set; }
        public string DJI { get; set; }
        public string SPWithoutDividends { get; set; }
        public string SPWithDividends { get; set; }
    }
}
