using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;

namespace mPower.Tests.UnitTests.EventHandlers.Ledger
{
    public class Ledger_DeletedEventTest : BaseHandlerTest
    {
        public override IEnumerable<object> Given()
        {
            yield return new BudgetDocument
            {
                Id = "5",
                AccountId = "5",
                AccountName = "5",
                AccountType = AccountTypeEnum.Expense,
                LedgerId = "1",
                Month = CurrentDate.Month,
                Year = CurrentDate.Year,
                BudgetAmount = 20000,
                SpentAmount = 5,
            };
        }

        public override IEnumerable<IEvent> When()
        {
            yield return new Ledger_DeletedEvent
            {
                LedgerId = "1",
            };
        }

        public override IEnumerable<object> Expected()
        {
            yield break;
        }

        public override IEnumerable<object> ShouldBeDeleted()
        {
            yield return new BudgetDocument
            {
                Id = "5",
            };
        }

        [Test]
        public void Test()
        {
            Dispatch();
        }
    }
}