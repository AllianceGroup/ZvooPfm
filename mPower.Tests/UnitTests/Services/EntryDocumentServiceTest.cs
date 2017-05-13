using System.Collections.Generic;
using NUnit.Framework;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Documents.DocumentServices.Accounting;

namespace mPower.Tests.UnitTests.Services
{
    public class EntryDocumentServiceTest : BaseServiceTest
    {
        private EntryDocumentService _entryDocumentService;

        public override void Setup()
        {
            base.Setup();
            _entryDocumentService = _container.GetInstance<EntryDocumentService>();
        }

        public override IEnumerable<object> Given()
        {
            yield return new EntryDocument()
            {
                AccountId = "1",
                LedgerId = "1",
                AccountLabel = AccountLabelEnum.Expense,
                AccountName = "1",
                AccountType = AccountTypeEnum.Expense,
                BookedDate = CurrentDate,
                BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                CreditAmountInCents = 100,
                DebitAmountInCents = 0,
                FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(-100, true),
                Imported = true,
                Memo = "Memo",
                OffsetAccountId = "2",
                OffsetAccountName = "2",
                Payee = "Payee",
                TransactionId = _id,
                Id = _id + "e0"
            };

            yield return new EntryDocument()
            {
                AccountId = "2",
                LedgerId = "1",
                AccountLabel = AccountLabelEnum.Bank,
                AccountName = "2",
                AccountType = AccountTypeEnum.Asset,
                BookedDate = CurrentDate,
                BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                CreditAmountInCents = 0,
                DebitAmountInCents = 100,
                FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(100, true),
                Imported = true,
                Memo = "Memo",
                OffsetAccountId = "1",
                OffsetAccountName = "1",
                Payee = "Payee",
                TransactionId = _id,
                Id = _id + "e1"
            };

            yield return new EntryDocument()
            {
                AccountId = "1",
                LedgerId = "2",
                AccountLabel = AccountLabelEnum.Expense,
                AccountName = "adf",
                AccountType = AccountTypeEnum.Expense,
                BookedDate = CurrentDate,
                BookedDateString = CurrentDate.ToString("MM-dd-yyyy"),
                CreditAmountInCents = 100,
                DebitAmountInCents = 0,
                FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(-100, true),
                Imported = true,
                Memo = "Memo",
                OffsetAccountId = "2",
                OffsetAccountName = "2",
                Payee = "Payee",
                TransactionId = _id,
                Id = _id + "e3"
            };

        }

        [Test]
        public void service_corretly_search_entries_by_accountId()
        {
            var entries = _entryDocumentService.GetByFilter(new EntryFilter() { AccountId = "1" });

            Assert.AreEqual(2, entries.Count);
        }

        [Test]
        public void service_corretly_search_entries_by_ledgerId()
        {
            var entries = _entryDocumentService.GetByFilter(new EntryFilter() { LedgerId = "1" });

            Assert.AreEqual(2, entries.Count);
        }

        [Test]
        public void service_corretly_search_entries_by_ledger_and_account_id()
        {
            var entries = _entryDocumentService.GetByFilter(new EntryFilter() { LedgerId = "1", AccountId = "1" });

            Assert.AreEqual(1, entries.Count);
        }

        [Test]
        public void service_corretly_search_entries_by_account_ids()
        {
            var entries = _entryDocumentService.GetByFilter(new EntryFilter() { AccountIds = new List<string>(){"1", "2"}});

            Assert.AreEqual(3, entries.Count);
        }
    }
}
