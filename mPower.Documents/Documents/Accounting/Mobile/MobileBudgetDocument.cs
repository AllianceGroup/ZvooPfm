using System.Collections.Generic;

namespace mPower.Documents.Documents.Accounting.Mobile
{
    public class MobileBudgetDocument
    {
        public MobileBudgetDocument()
        {
            SubBudgets = new List<MobileChildBudgetDocument>();
        }

        public string Id { get; set; }

        public string LedgerId { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public string AccountName { get; set; }

        public string BudgetAmount { get; set; }

        public string SpentAmount { get; set; }

        public List<MobileChildBudgetDocument> SubBudgets { get; set; }
    }

    public class MobileChildBudgetDocument
    {
        public string AccountName { get; set; }

        public string SpentAmount { get; set; }
    }
}
