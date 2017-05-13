using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting.Enums;

namespace mPower.Tests.UnitTests.Domain.Accounting.Transaction
{
    public class statistic_transaction_deletion_test : TransactionTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield return Transaction_Created("transaction")
                .AddEntry("Checking", 0, 50, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Split", "Split")
                .AddEntry("Car Loan", 0, 50, CurrentDate, AccountTypeEnum.Liability, AccountLabelEnum.OtherCurrentLiability, "Split", "Split")
                .AddEntry("Checking", 100, 0, CurrentDate, AccountTypeEnum.Asset, AccountLabelEnum.Bank, "Split", "Split");
            yield return Transaction_Deleted("transaction");
        }

        public override IEnumerable<ICommand> When()
        {
            return new List<ICommand>();
        }

        public override IEnumerable<IEvent> Expected()
        {
            return new List<IEvent>();
        }

        [Test]
        public void Test()
        {
            DispatchEvents(() =>
            {
                var statisticService = GetInstance<TransactionsStatisticDocumentService>();

                var filter = new TransactionsStatisticFilter { LedgerId = _id, Year = CurrentDate.Year, Month = CurrentDate.Month };
                var statDocuments = statisticService.GetByFilter(filter);

                Assert.AreEqual(2, statDocuments.Count);

                var assets = statDocuments.Where(sd => sd.AccountType == AccountTypeEnum.Asset).ToList();
                Assert.AreEqual(1, assets.Count);
                Assert.AreEqual(0, assets[0].DebitAmountInCents);
                Assert.AreEqual(0, assets[0].CreditAmountInCents);

                var liabilities = statDocuments.Where(sd => sd.AccountType == AccountTypeEnum.Liability).ToList();
                Assert.AreEqual(1, liabilities.Count);
                Assert.AreEqual(0, liabilities[0].DebitAmountInCents);
                Assert.AreEqual(0, liabilities[0].CreditAmountInCents);
            });
        }
    }
}