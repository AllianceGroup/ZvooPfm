using System;
using System.Web.Routing;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Services;

namespace mPower.Domain.Accounting
{
    public class TransactionClientFilter
    {
        public TransactionClientFilter()
        {
            page = 1;
            itemsPerPage = 10;
            s = 0;
        }

        public TransactionClientFilter(string ledgerId, string applicationName, RequestContext requestContext):this()
        {
            this.ledgerId = ledgerId;
            this.applicationName = applicationName;
            this.RequestContext = requestContext;
        }

        public static TransactionClientFilter Empty(string ledgerId, string applicationName, RequestContext requestContext)
        {
            return new TransactionClientFilter(ledgerId, applicationName, requestContext);
        }

        public string request { get; set; }
        public int page { get; set; }
        public int itemsPerPage { get; set; }
        public string accountId { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string mode { get; set; }
        public string type { get; set; }
        public string ledgerId { get; set; }
        public string affiliateId { get; set; }
        public string applicationName { get; set; }
        public RequestContext RequestContext { get; set; }
        public string UserId { get; set; }

        public PagingInfo Paging { get; set; }

        //s = 1 means include sub accounts transactions
        public int s { get; set; }

        public bool IncludeSubAccounts
        {
            get { return s == 1; }
        }


        private AccountLabelEnum? _parsedLabel;
        public AccountLabelEnum? ParsedLabel
        {
            get
            {
                if (_parsedLabel == null)
                    _parsedLabel = ParseAccountLabel();

                return _parsedLabel;
            }
        }

        public AccountTypeEnum? ParsedBudgetType
        {
            get
            {
                var intType = 0;
                int.TryParse(type, out intType);

                if (intType == 0)
                    return null;

                var accountType = (AccountTypeEnum) intType;

                //only for budgets account types
                if (accountType == AccountTypeEnum.Income || accountType == AccountTypeEnum.Expense)
                    return accountType;

                return null;
            }
        }

        private AccountLabelEnum? ParseAccountLabel()
        {
            AccountLabelEnum? label = null;
            if (mode == "all")
            {
                label = null;
            }
            else if (!String.IsNullOrEmpty(mode))
            {
                AccountLabelEnum modeResult;
                Enum.TryParse(mode, true, out modeResult);
                label = modeResult;
            }

            return label;
        }
    }
}