using System.Collections.Generic;
using System.Linq;
using Default.Factories.Commands;
using mPower.Documents;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Tests.MoqObjects.DocumentServices;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Web.Factories.Commands
{
    public class Transaction_ModifyCommandFactoryTest : BaseWebTest
    {
        private LedgerDocumentServiceMock _ledgerServiceMock;
        private TransactionDocumentServiceMock _transactionsServiceMock;
        private Transaction_ModifyCommandFactory _factory;

        [SetUp]
        public void TestSetup()
        {
            _ledgerServiceMock = MockFactory.Create<LedgerDocumentServiceMock>().AddGetById();

            _transactionsServiceMock = MockFactory.Create<TransactionDocumentServiceMock>();

            _factory = new Transaction_ModifyCommandFactory(_ledgerServiceMock.Object, _transactionsServiceMock.Object);
        }

        [Test]
        public void if_dto_contains_base_account_id_it_should_be_mapped_to_command()
        {
            var ledger = _ledgerServiceMock.FakeLedger;

            var command = _factory.Load(GetTransactionModifyCommandDto());

            var account = ledger.Accounts.Single(x => x.Id == "6");

            Assert.AreEqual(command.BaseEntryAccountId, account.Id);
            Assert.AreEqual(command.BaseEntryAccountType, account.TypeEnum);
        }

        [Test]
        public void two_entries_offsset_accounts_mapped_correctly()
        {
            var ledger = _ledgerServiceMock.FakeLedger;

            var dto = GetTransactionModifyCommandDto();

            var command = _factory.Load(dto);

            Assert.AreEqual(dto.Entries[1].AccountId, command.Entries[0].OffsetAccountId);
            var account = ledger.Accounts.Single(x => x.Id == dto.Entries[1].AccountId);
            Assert.AreEqual(account.Name, command.Entries[0].OffsetAccountName);

            Assert.AreEqual(dto.Entries[0].AccountId, command.Entries[1].OffsetAccountId);
            account = ledger.Accounts.Single(x => x.Id == dto.Entries[0].AccountId);
            Assert.AreEqual(account.Name, command.Entries[1].OffsetAccountName);
        }

        [Test]
        public void three_or_more_entries_offsset_accounts_should_be_split_word()
        {
            var dto = GetTransactionModifyCommandDto();
            dto.Entries.Add(new EntryData());

            var command = _factory.Load(dto);

            Assert.AreEqual("Split", command.Entries[0].OffsetAccountId);
            Assert.AreEqual("Split", command.Entries[0].OffsetAccountName);

            Assert.AreEqual("Split", command.Entries[1].OffsetAccountId);
            Assert.AreEqual("Split", command.Entries[1].OffsetAccountName);
        }

        private TransactionModifyDto GetTransactionModifyCommandDto()
        {
            var ledger = _ledgerServiceMock.FakeLedger;

            var dto = new TransactionModifyDto()
            {
                LedgerId = ledger.Id,
                TransactionId = "1",
                BaseEntryAccountId = "6",
                Imported = false,
                ReferenceNumber = "4",
                Type = TransactionType.BankTransfer,
                Entries = new List<EntryData>()
                {
                    new EntryData()
                        {
                            AccountId = "1",
                            BookedDate = CurrentDate,
                            CreditAmountInCents = 100,
                            DebitAmountInCents = 0,
                            Memo = "memo",
                            Payee = "payee"
                        },
                    new EntryData()
                        {
                            AccountId = "5",
                            BookedDate = CurrentDate,
                            CreditAmountInCents = 0,
                            DebitAmountInCents = 100,
                            Memo = "memo 2",
                            Payee = "payee 2"
                        }
                }
            };

            return dto;
        }
    }
}
