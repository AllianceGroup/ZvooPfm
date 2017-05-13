using System;
using System.Linq;
using Default.Factories.Commands;
using Default.ViewModel.TransactionsController;
using NUnit.Framework;
using mPower.Documents;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Environment;
using mPower.Tests.MoqObjects.DocumentServices;

namespace mPower.Tests.UnitTests.Web.Factories.Commands
{
    [TestFixture]
    public class Transaction_CreateCommandFactoryTest: BaseWebTest
    {
        private LedgerDocumentServiceMock _ledgerService;


        [SetUp]
        public void SetupTest()
        {
            _ledgerService = MockFactory.Create<LedgerDocumentServiceMock>().AddFindOne().AddGetById();
            Factory = new Transaction_CreateCommandFactory(_ledgerService.Object, new MongoObjectIdGenerator());
        }

        protected Transaction_CreateCommandFactory Factory { get; set; }

        [Test]
        public void factory_build_correct_command_for_filter_with_empty_entries_list()
        {
            var account = _ledgerService.FakeLedger.Accounts.First();
            var command = Factory.Load(new TransactionDto()
                             {
                                 LedgerId = _ledgerService.FakeLedger.Id,
                                 BaseEntryAccountId = account.Id,
                                 BaseEntryAccountType = account.TypeEnum
                             });
            Assert.NotNull(command);
            Assert.IsEmpty(command.Entries);
        }

        [Test]
        public void factory_returns_command_even_account_width_specified_id_does_not_exist()
        {
            var baseEntryAccountId = "inccorect account ID";
            var baseEntryAccountType = AccountTypeEnum.Asset;
            var command = Factory.Load(new TransactionDto()
            {
                LedgerId = _ledgerService.FakeLedger.Id,
                BaseEntryAccountId = baseEntryAccountId,
                BaseEntryAccountType = baseEntryAccountType
            });
            Assert.NotNull(command);
            Assert.AreEqual(command.BaseEntryAccountType, baseEntryAccountType);
            Assert.AreEqual(command.BaseEntryAccountId, baseEntryAccountId);
        }

        [Test]
        public void general_properties_are_filled()
        {
            var account = _ledgerService.FakeLedger.Accounts.First();
            var input = new TransactionDto();

            input.Imported = true;
            input.LedgerId = _ledgerService.FakeLedger.Id;
            input.ReferenceNumber = "20";
            input.TransactionId = "1";
            input.Type = TransactionType.Invoice;
            input.ImportedTransactionId = "1";
            input.BaseEntryAccountId = account.Id;
            input.BaseEntryAccountType = account.TypeEnum;

            var command = Factory.Load(input);

            Assert.AreEqual(input.Imported,command.Imported);
            Assert.AreEqual(input.LedgerId, command.LedgerId);
            Assert.AreEqual(input.ReferenceNumber, command.ReferenceNumber);
            Assert.AreEqual(input.TransactionId, command.TransactionId);
            Assert.AreEqual(input.Type, command.Type);
            Assert.AreEqual(input.ImportedTransactionId, command.ImportedTransactionId);
            Assert.AreEqual(input.BaseEntryAccountId, command.BaseEntryAccountId);
            Assert.AreEqual(input.BaseEntryAccountType, command.BaseEntryAccountType);
        }

        [Test]
        public void execution_with_zero_amount_throws_exception()
        {
            var model = new AddStandardTransactionViewModel()
                            {
                                AmountInDollars = "0"
                            };
            Assert.Throws<Exception>(()=> 
                Factory.Load(model)
                );

        }

        [Test]
        public void factory_replaces_min_date_with_current_date()
        {
            var account = _ledgerService.FakeLedger.Accounts.First();
            var model = new AddStandardTransactionViewModel()
            {
                AmountInDollars = "10000",
                AccountId = account.Id,
                LedgerId = _ledgerService.FakeLedger.Id,
                BookedDate = DateTime.MinValue.ToShortDateString(),
                OffSetAccountId = "2"
            };
            DateTime start = DateTime.Now;
            var command = Factory.Load(model);
            DateTime stop = DateTime.Now;
            foreach (var entry in command.Entries)
            {
                Assert.GreaterOrEqual(entry.BookedDate, start);
                Assert.LessOrEqual(entry.BookedDate, stop);
            }
        }

        [Test]
        public void factory_adds_time_to_valid_booked_date()
        {
            var account = _ledgerService.FakeLedger.Accounts.First();
            var model = new AddStandardTransactionViewModel()
            {
                AmountInDollars = "10000",
                AccountId = account.Id,
                LedgerId = _ledgerService.FakeLedger.Id,
                BookedDate = "01/01/2012",
                OffSetAccountId = "2"
            };
            DateTime start = DateTime.Now;
            var command = Factory.Load(model);
            DateTime stop = DateTime.Now;
            foreach (var entry in command.Entries)
            {
                Assert.LessOrEqual(start.Hour, entry.BookedDate.Hour);
                Assert.LessOrEqual(start.Minute, entry.BookedDate.Minute);
                Assert.LessOrEqual(start.Second, entry.BookedDate.Second);

                Assert.GreaterOrEqual(stop.Hour, entry.BookedDate.Hour);
                Assert.GreaterOrEqual(stop.Minute, entry.BookedDate.Minute);
                Assert.GreaterOrEqual(stop.Second, entry.BookedDate.Second);
            }
        }     
        
    }
}