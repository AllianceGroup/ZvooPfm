using System.Collections.Generic;
using NUnit.Framework;
using Paralect.Domain;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting.Enums;

namespace mPower.Tests.UnitTests.Domain.Accounting.Ledger.AccountTests
{
    public class accounts_archiving_removing_test : LedgerTest
    {
        public override IEnumerable<IEvent> Given()
        {
            yield return Ledger_Created();
        }

        public override IEnumerable<ICommand> When()
        {
            yield return Ledger_Account_Create("45", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
            yield return Ledger_Account_Create("4534", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
            yield return Ledger_Account_Remove("45");
            yield return Ledger_Account_Archive("4534", "outdated");
        }

        public override IEnumerable<IEvent> Expected()
        {
            yield return Ledger_Account_Added("45", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
            yield return Ledger_Account_Added("4534", AccountTypeEnum.Expense, AccountLabelEnum.Expense);
            yield return Ledger_Account_Removed("45");
            yield return Ledger_Account_Archived("4534", "outdated");
        }

        [Ignore]
        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var _lederDocumentService = GetInstance<LedgerDocumentService>();

                var ledgerDoc = _lederDocumentService.GetById(_id);
                Assert.AreEqual(1, ledgerDoc.Accounts.Count);

                var account = ledgerDoc.Accounts[0];

                Assert.AreEqual("4534", account.Id);
                Assert.AreEqual(true, account.Archived);
                Assert.AreEqual("outdated", account.ReasonToArchive);
                Assert.AreEqual(AccountTypeEnum.Expense, account.TypeEnum);
                Assert.AreEqual(AccountLabelEnum.Expense, account.LabelEnum);

                // mobile API asserts
                //var _mobileAccountDocumentService = GetInstance<MobileAccountDocumentService>();

                //var mobileAccounts = _mobileAccountDocumentService.GetByFilter(new MobileAccountFilter {LedgerId = _id});
                //Assert.AreEqual(1, mobileAccounts.Count);

                //var mobileAccount = mobileAccounts[0];

                //Assert.AreEqual("4534", mobileAccount.Id);
                //Assert.AreEqual(true, mobileAccount.Archived);
                //Assert.AreEqual("outdated", mobileAccount.ReasonToArchive);
                //Assert.AreEqual(AccountTypeEnum.Expense, mobileAccount.Type);
                //Assert.AreEqual(AccountLabelEnum.Expense, mobileAccount.Label);
            });
        }
    }
}
