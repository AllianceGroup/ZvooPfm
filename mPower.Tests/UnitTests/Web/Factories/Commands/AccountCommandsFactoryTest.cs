using System;
using System.Globalization;
using System.Linq;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Framework.Environment;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Web.Factories.Commands
{
    public class AccountCommandsFactoryTest : BaseWebTest
    {
        private IIdGenerator _idGenerator;
        protected AccountsService AccountsService { get; set; }

        [SetUp]
        public void TestSetup()
        {
            _idGenerator = _container.GetInstance<IIdGenerator>();
            AccountsService = new AccountsService(_idGenerator);
        }

        [Test]
        public void CreatePersonalBaseAccounts_sets_ledger_for_all_commands()
        {
            var id = Guid.NewGuid().ToString();
            var commands = AccountsService.CreatePersonalBaseAccounts(id);
            foreach (var command in commands)
            {
                var type = command.GetType();
                var property = type.GetProperty("LedgerId");
                Assert.IsTrue(id.Equals(property.GetValue(command, null)));
            }
        }

        [Test]
        public void CreateBusinessBaseAccounts_sets_ledger_for_all_commands()
        {
            var id = Guid.NewGuid().ToString();
            var commands = AccountsService.CreateBusinessBaseAccounts(id);
            foreach (var command in commands)
            {
                var type = command.GetType();
                var property = type.GetProperty("LedgerId");
                Assert.IsTrue(id.Equals(property.GetValue(command, null)));
            }
        }

        [Test]
        public void CreateBusinessCommonAccounts_sets_ledger_and_account_data_for_all_commands()
        {
            var id = Guid.NewGuid().ToString();
            var commands = AccountsService.CreateBusinessCommonAccounts(id);
            foreach (var command in commands)
            {
                Assert.AreEqual(id, command.LedgerId);
                Assert.AreEqual(AccountLabelEnum.Expense, command.AccountLabelEnum);
                Assert.AreEqual(AccountTypeEnum.Expense, command.AccountTypeEnum);
            }
        }

        [Test]
        public void CreatePersonalCommonAccounts_sets_ledger_and_account_data_for_all_commands()
        {
            var id = Guid.NewGuid().ToString();
            var commands = AccountsService.CreatePersonalCommonAccounts(id);
            foreach (var command in commands)
            {
                Assert.AreEqual(id, command.LedgerId);
                Assert.AreEqual(AccountLabelEnum.Expense, command.AccountLabelEnum);
                Assert.AreEqual(AccountTypeEnum.Expense, command.AccountTypeEnum);
            }
        }

        [Test]
        public void CreateProductBasedBusinessAccounts_sets_ledger_for_all_commands()
        {
            var id = Guid.NewGuid().ToString();
            var commands = AccountsService.CreateProductBasedBusinessAccounts(id);
            foreach (var command in commands)
            {
                Assert.AreEqual(id, command.LedgerId);
            }
        }

        [Test]
        public void CreateServiceBasedBusinessAccounts_sets_ledger_for_all_commands()
        {
            var id = Guid.NewGuid().ToString();
            var commands = AccountsService.CreateServiceBasedBusinessAccounts(id);
            foreach (var command in commands)
            {
                Assert.AreEqual(id, command.LedgerId);
            }
        }

        [Test]
        public void CreateAccountCommands_without_opening_balance()
        {
            var random = new Random();
            var data = new AccountData
            {
                Name = "Test account",
                Description = "Test description",
                LabelEnum = AccountLabelEnum.Bank,
                TypeEnum = AccountingFormatter.AccountLabelToType(AccountLabelEnum.Bank),
                ParentAccountId = Guid.NewGuid().ToString(),
                Number = random.Next(1, 9999).ToString(CultureInfo.InvariantCulture),
                InterestRatePerc = (float) random.NextDouble(),
                MinMonthPaymentInCents = random.Next(1000, 100000),
                CreditLimitInCents = random.Next(1000, 100000),
            };
            var ledgerId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

            var commands = AccountsService.CreateAccountCommands(data.Name, userId, ledgerId, data.LabelEnum, data.ParentAccountId, 
                data.Description, data.Number, data.InterestRatePerc, AccountingFormatter.CentsToDollars(data.MinMonthPaymentInCents), 
                AccountingFormatter.CentsToDollars(data.CreditLimitInCents), 0).ToList();
            
            Assert.AreEqual(1, commands.Count);
            var createAccountCmd = commands[0] as Ledger_Account_CreateCommand;
            Assert.IsNotNull(createAccountCmd);
            Assert.IsNotNull(createAccountCmd.AccountId);
            Assert.AreEqual(data.Name, createAccountCmd.Name);
            Assert.AreEqual(data.Description, createAccountCmd.Description);
            Assert.AreEqual(ledgerId, createAccountCmd.LedgerId);
            Assert.AreEqual(data.LabelEnum, createAccountCmd.AccountLabelEnum);
            Assert.AreEqual(data.TypeEnum, createAccountCmd.AccountTypeEnum);
            Assert.AreEqual(data.ParentAccountId, createAccountCmd.ParentAccountId);
            Assert.IsFalse(createAccountCmd.Aggregated);
            Assert.IsFalse(createAccountCmd.Imported);
            Assert.AreEqual(data.Number, createAccountCmd.Number);
            Assert.AreEqual(data.InterestRatePerc, createAccountCmd.InterestRatePerc);
            Assert.AreEqual(data.MinMonthPaymentInCents, createAccountCmd.MinMonthPaymentInCents);
            Assert.AreEqual(data.CreditLimitInCents, createAccountCmd.CreditLimitInCents);
        }

        [Test]
        public void CreateAccountCommands_with_opening_balance()
        {
            var random = new Random();
            var data = new AccountData
            {
                Name = "Test account",
                Description = "Test description",
                LabelEnum = AccountLabelEnum.Bank,
                TypeEnum = AccountingFormatter.AccountLabelToType(AccountLabelEnum.Bank),
                ParentAccountId = Guid.NewGuid().ToString(),
                Number = random.Next(1, 9999).ToString(CultureInfo.InvariantCulture),
                InterestRatePerc = (float) random.NextDouble(),
                MinMonthPaymentInCents = random.Next(1000, 100000),
                CreditLimitInCents = random.Next(1000, 100000),
            };
            const long openingBalanceInСents = 1000;
            var ledgerId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

            var commands = AccountsService.CreateAccountCommands(data.Name, userId, ledgerId, data.LabelEnum, data.ParentAccountId, 
                data.Description, data.Number, data.InterestRatePerc, AccountingFormatter.CentsToDollars(data.MinMonthPaymentInCents), 
                AccountingFormatter.CentsToDollars(data.CreditLimitInCents), AccountingFormatter.CentsToDollars(openingBalanceInСents)).ToList();
            
            Assert.AreEqual(2, commands.Count);
            // won't check 'create account' command - it's already done in previous test
            var createAccountCmd = commands[0] as Ledger_Account_CreateCommand;
            var createTransCmd = commands[1] as Transaction_CreateCommand;
            Assert.IsNotNull(createAccountCmd);
            Assert.IsNotNull(createTransCmd);
            Assert.IsNotNull(createTransCmd.TransactionId);
            Assert.AreEqual(TransactionType.Deposit, createTransCmd.Type);
            Assert.AreEqual(ledgerId, createTransCmd.LedgerId);
            Assert.AreEqual(createAccountCmd.AccountId, createTransCmd.BaseEntryAccountId);
            Assert.AreEqual(data.TypeEnum, createTransCmd.BaseEntryAccountType);
            Assert.AreEqual(2, createTransCmd.Entries.Count);
            createTransCmd.Entries.ForEach(entry =>
            {
                Assert.Greater(entry.BookedDate, DateTime.Now.AddDays(-2));
                Assert.LessOrEqual(entry.BookedDate, DateTime.Now.AddDays(-1));
                Assert.AreEqual("Beginning Balance Adjustment", entry.Memo);
                Assert.AreEqual(ledgerId, entry.LedgerId);
                Assert.AreEqual(createTransCmd.TransactionId, entry.TransactionId);
                Assert.IsFalse(entry.TransactionImported);
            });

            var baseEntry = createTransCmd.Entries.Find(x => x.AccountId == createTransCmd.BaseEntryAccountId);
            Assert.IsNotNull(baseEntry);
            Assert.AreEqual(AccountingFormatter.CreditAmount(openingBalanceInСents, data.TypeEnum), baseEntry.CreditAmountInCents);
            Assert.AreEqual(AccountingFormatter.DebitAmount(openingBalanceInСents, data.TypeEnum), baseEntry.DebitAmountInCents);
            Assert.AreEqual(data.LabelEnum, baseEntry.AccountLabel);
            Assert.AreEqual(data.TypeEnum, baseEntry.AccountType);
            Assert.AreEqual(data.Name, baseEntry.AccountName);
            Assert.AreEqual(BaseAccounts.OpeningBalanceEquity, baseEntry.OffsetAccountId);
            Assert.AreEqual(BaseAccounts.OpeningBalanceEquity.GetDescription(), baseEntry.OffsetAccountName);

            var offsetEntry = createTransCmd.Entries.Find(x => x.AccountId == BaseAccounts.OpeningBalanceEquity);
            Assert.IsNotNull(offsetEntry);
            Assert.AreEqual(AccountingFormatter.DebitAmount(openingBalanceInСents, data.TypeEnum), offsetEntry.CreditAmountInCents);
            Assert.AreEqual(AccountingFormatter.CreditAmount(openingBalanceInСents, data.TypeEnum), offsetEntry.DebitAmountInCents);
            Assert.AreEqual(AccountLabelEnum.Equity, offsetEntry.AccountLabel);
            Assert.AreEqual(AccountTypeEnum.Equity, offsetEntry.AccountType);
            Assert.AreEqual(BaseAccounts.OpeningBalanceEquity.GetDescription(), offsetEntry.AccountName);
            Assert.AreEqual(createTransCmd.BaseEntryAccountId, offsetEntry.OffsetAccountId);
            Assert.AreEqual(data.Name, offsetEntry.OffsetAccountName);
        }

        [Test]
        public void SetupPersonalLedger_test()
        {
            var userId = Guid.NewGuid().ToString();
            var ledgerId = Guid.NewGuid().ToString();
            var commands = AccountsService.SetupPersonalLedger(userId, ledgerId).ToList();

            var ledgerCreationCommand = commands.Where(x => x is Ledger_CreateCommand).ToList();
            Assert.AreEqual(1, ledgerCreationCommand.Count);
            Assert.AreEqual(ledgerId, ((Ledger_CreateCommand)ledgerCreationCommand[0]).LedgerId);

            var userAssignCommand = commands.Where(x => x is Ledger_User_AddCommand).ToList();
            Assert.AreEqual(1, userAssignCommand.Count);
            var cmd = (Ledger_User_AddCommand)userAssignCommand[0];
            Assert.AreEqual(ledgerId, cmd.LedgerId);
            Assert.AreEqual(userId, cmd.UserId);
        }
    }
}