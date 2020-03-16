using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents
{
    public class MarketHistoryDocument
    { 
 
        public string _id { get; set; }

        public string Year { get; set; }
        public double DJI { get; set; }
        public double SPWithoutDividends { get; set; }
        public double SPWithDividends { get; set; }
    }
}
