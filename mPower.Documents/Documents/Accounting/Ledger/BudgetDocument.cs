using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Accounting.Ledger
{
    public enum BudgetDocumentType
    {
        Default,

    }

    public class ChildBudgetDocument
    {
        public ChildBudgetDocument()
        {
        }

        public string AccountId { get; set; }

        public string ParentAccountId { get; set; }

        public string AccountName { get; set; }

        public AccountTypeEnum AccountType { get; set; }

        public long SpentAmount { get; set; }
    }

    public class BudgetDocument
    {
        public BudgetDocument()
        {
            SubBudgets = new List<ChildBudgetDocument>();
        }

        [BsonId]
        public string Id { get; set; }

        public string LedgerId { get; set; }

        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public AccountTypeEnum AccountType { get; set; }

        public string ParentId { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public long BudgetAmount { get; set; }

        public long SpentAmount { get; set; }

        public long SpentAmountWithSubAccounts
        {
            get { return SpentAmount + SubBudgets.Sum(x => x.SpentAmount); }
        }

        public List<ChildBudgetDocument> SubBudgets { get; set; }

        public int YearPlusMonth
        {
            get { return Year * 12 + Month; }
            set { }
        }

        public int Persent
        {
            get
            {
                int persent = 0;

                if (BudgetAmount != 0)
                {
                    persent = (int)((decimal)((decimal)SpentAmountWithSubAccounts / (decimal)BudgetAmount) * 100);
                    if (persent > 100)
                        persent = 100;
                    else if (persent < 0)
                        persent = 0;
                }
               
                return persent;
            }
        }
    }
}
