using System;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class MonthlyPayStatusDocument
    {
        public DateTime Date;
        public string Status;
        public bool Changed;
    }
}
