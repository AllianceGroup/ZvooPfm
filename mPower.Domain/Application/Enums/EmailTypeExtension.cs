using System;
using System.Collections.Generic;
using mPower.Framework.Utils.Extensions;

namespace mPower.Domain.Application.Enums
{
    public static class EmailTypeExtension
    {
        private static readonly Dictionary<EmailTypeEnum, EmailTypeData> TypesData;

        static EmailTypeExtension()
        {
            var amountBorders = new Dictionary<int, string>
            {
                {200, "$200"},
                {500, "$500"},
                {1000, "$1000"},
                {2000, "$2000"},
                {5000, "$5000"},
            };
            var lowBalanceBorders = new Dictionary<int, string>
            {
                {10, "$10"},
                {20, "$20"},
                {50, "$50"},
                {100, "$100"},
                {200, "$200"},
                {500, "$500"},
                {1000, "$1000"},
                {2000, "$2000"},
            };
            var largePurchaseBorders = new Dictionary<int, string>
            {
                {25, "$25"},
                {50, "$50"},
                {100, "$100"},
                {200, "$200"},
                {500, "$500"},
                {1000, "$1000"},
                {2000, "$2000"},
                {5000, "$5000"},
            };
            var days = new Dictionary<int, string>
            {
                {1, "1 day"},
                {3, "3 days"},
                {5, "5 days"},
                {7, "7 days"},
                {14, "14 days"},
            };

            TypesData = new Dictionary<EmailTypeEnum, EmailTypeData>
            {
                {EmailTypeEnum.LowBalance, new EmailTypeData(EmailTypeEnum.LowBalance.GetDescription(), "If any of my accounts drop below this balance:", lowBalanceBorders)},
                {EmailTypeEnum.LargePurchases, new EmailTypeData(EmailTypeEnum.LargePurchases.GetDescription(), "If a purchase costs more than this:", largePurchaseBorders)},
                {EmailTypeEnum.BillReminder, new EmailTypeData(EmailTypeEnum.BillReminder.GetDescription(), "How many days in advance should we remind you?", days)},
                {EmailTypeEnum.AvailableCredit, new EmailTypeData(EmailTypeEnum.AvailableCredit.GetDescription(), "If my available credit falls below:", amountBorders)},
                {EmailTypeEnum.UnusualSpending, new EmailTypeData(EmailTypeEnum.UnusualSpending.GetDescription(), "If my spending in a category is above average by:", amountBorders)},
                {EmailTypeEnum.OverBudget, new EmailTypeData(EmailTypeEnum.OverBudget.GetDescription(), "If I exceed any of my budgets during the month", null)},
            };
        }

        public static EmailTypeData GetData(this EmailTypeEnum type)
         {
            if (TypesData == null || !TypesData.ContainsKey(type))
            {
                throw new NotImplementedException();
            }
            return TypesData[type];
         }
    }

    public class EmailTypeData
    {
        public string Description { get; private set; }
        public string Prefix { get; private set; }
        public Dictionary<int, string> AvailableBorders { get; private set; }

        public EmailTypeData(string desc, string prefix, Dictionary<int, string> values)
        {
            Description = desc;
            Prefix = prefix;
            AvailableBorders = values;
        }
    }
}