using System.Collections.Generic;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Tests.Environment;
using NUnit.Framework;
using Paralect.Domain;

namespace mPower.Tests.UnitTests.EventHandlers.Ledger
{
    public class Ledger_Account_AddedEventTest : BaseHandlerTest
    {
        public override IEnumerable<object> Given()
        {
            yield return new LedgerDocument
            {
                Id = _id,
            };
        }

        public override IEnumerable<IEvent> When()
        {
            yield return new Ledger_Account_AddedEvent
            {
                LedgerId = _id,
                AccountId = "a1",
                AccountTypeEnum = AccountingFormatter.AccountLabelToType(AccountLabelEnum.CreditCard),
                Name = "Test account",
                AccountLabelEnum = AccountLabelEnum.CreditCard,
                Imported = true,
                Aggregated = true,
                ContentServiceId = 1,
                YodleeItemAccountId = "1",
                Number = "1234",
                Description = "Account for testing creation event",
                ParentAccountId = "a0",
                InterestRatePerc = 0.12f,
                MinMonthPaymentInCents = 50000,
                CreditLimitInCents = 1000000,
                InstitutionName = "Test institution",
                IntuitAccountNumber = "ian1",
            };
        }

        public override IEnumerable<object> Expected()
        {
            yield return new LedgerDocument
            {
                Id = _id,
                Accounts = new List<AccountDocument>
                {
                    new AccountDocument
                    {
                        Id = "a1",
                        TypeEnum = AccountingFormatter.AccountLabelToType(AccountLabelEnum.CreditCard),
                        Name = "Test account",
                        LabelEnum = AccountLabelEnum.CreditCard,
                        Imported = true,
                        Aggregated = true,
                        IntuitInstitutionId = 1,
                        IntuitAccountId = 1,
                        Number = "1234",
                        Description = "Account for testing creation event",
                        ParentAccountId = "a0",
                        InterestRatePerc = 0.12f,
                        MinMonthPaymentInCents = 50000,
                        CreditLimitInCents = 1000000,
                        InstitutionName = "Test institution",
                        IntuitAccountNumber = "ian1",
                    },
                },
            };
        }

        [Test]
        public void Test()
        {
            Dispatch(ignoreList: IgnoreList.Create("Created", "DateLastAggregated"));
        }
    }
}