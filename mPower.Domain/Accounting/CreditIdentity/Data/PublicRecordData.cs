using System;
using System.Collections.Generic;

namespace mPower.Domain.Accounting.CreditIdentity.Data
{

    //this is coming from Trans union api as dynamic Item
    //We need to declare all possible dinamic types in order to allow deserialize them
    //In Global.asax

    #region dynamic items

    public class TaxLien
    {
        public int amount { get; set; }
        public bool amountSpecified { get; set; }
        public DateTime dateReleased { get; set; }
        public bool dateReleasedSpecified { get; set; }
    }

    public class Bankruptcy
    {
        public int assetAmount { get; set; }
        public bool assetAmountSpecified { get; set; }
        public DateTime dateResolved { get; set; }
        public bool dateResolvedSpecified { get; set; }
        public int exemptAmount { get; set; }
        public bool exemptAmountSpecified { get; set; }
        public int liabilityAmount { get; set; }
        public bool liabilityAmountSpecified { get; set; }
    }

    public class LegalItem
    {
        public int actionAmount { get; set; }
        public bool actionAmountSpecified { get; set; }
        public DateTime dateSatisfied { get; set; }
        public bool dateSatisfiedSpecified { get; set; }
        public string plaintiff { get; set; }
    }

    #endregion

    public class PublicRecordData
    {
        public DateTime DateFiled { get; set; }
        public DateTime DateVerified { get; set; }
        public DateTime DateExpires { get; set; }

        public string ClassificationDescription { get; set; }
        public string ClassificationSymbol { get; set; }
        public int ClassificationRank { get; set; }
        public string CourtName { get; set; }
        public string DesignatorDescription { get; set; }
        public string Bureau { get; set; }
        public string IndustryCodeDescription { get; set; }
        public string IndustryCodeSymbol { get; set; }
        public int IndustryRank { get; set; }
        public string ReferenceNumber { get; set; }
        public List<string> CustomRemarks { get; set; }
        public string SubscriberCode { get; set; }
        public string Status { get; set; }

        public string Type { get; set; }
        public dynamic Item { get; set; }
    }
}